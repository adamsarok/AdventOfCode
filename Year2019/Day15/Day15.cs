using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers.Dijkstra;

namespace Year2019.Day15 {
	public class Day15 : Solver {
		public Day15() : base(2019, 15) {
		}
		private long[] startCode;
		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			IsShort = fileName.Contains("short");
			var cmds = File.ReadAllLines(fileName)[0].Split(',');
			startCode = new long[cmds.Length];
			for (int i = 0; i < cmds.Length; i++) {
				startCode[i] = long.Parse(cmds[i]);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			base.ReadInputPart2(fileName);
			ReadInputPart1(fileName);
		}

		class IntCodeComputer {
			long[] intCode;
			long relativeBase = 0;
			long instructionPointer;
			Dictionary<LVec, Tile> painted;
			private long[] startCode;


			public IntCodeComputer(long[] startCode) {
				intCode = new long[10000];
				instructionPointer = 0;
				pos = new LVec(0, 0);
				painted = new() { { pos, Tile.Start } };
				for (int i = 0; i < startCode.Length; i++) intCode[i] = startCode[i];
				SetDir(Direction.West);
			}

			long GetVariable(long pos, ParamMode mode) {
				switch (mode) {
					case ParamMode.Position:
						return intCode[intCode[pos]];
					case ParamMode.Immediate:
						return intCode[pos];
					case ParamMode.Relative:
						return intCode[intCode[pos] + relativeBase];
					default: throw new Oopsie("unknown ParamMode");
				}
			}
			void SetVariable(long pos, long val, ParamMode mode) {
				switch (mode) {
					case ParamMode.Position:
						intCode[intCode[pos]] = val;
						break;
					case ParamMode.Immediate:
						intCode[intCode[pos]] = val;
						break;
					case ParamMode.Relative:
						intCode[intCode[pos] + relativeBase] = val;
						break;
					default: throw new Oopsie("unknown ParamMode");
				}
			}

			enum StatusCode {
				Wall = 0,   //pos stays same
				Step = 1,   //pos changed to next step
				Finish = 2, //oxygen system reached
			}
			enum Opcode {
				ADD = 1,
				MUL = 2,
				IN = 3,
				OUT = 4,
				JIT = 5,
				JIF = 6,
				LT = 7,
				EQ = 8,
				RB = 9,
				HALT = 99
			}
			enum ParamMode {
				Position = 0,
				Immediate = 1,
				Relative = 2
			}
			record Command(Opcode opcode, ParamMode aMode, ParamMode bMode, ParamMode cMode);
			Command ParseCommand(long cmdNum) {
				var opcode = (Opcode)(cmdNum % 100);
				if (opcode == 0) throw new Oopsie();
				long modes = cmdNum / 100;
				var aMode = (ParamMode)(modes % 10);
				modes /= 10;
				var bMode = (ParamMode)(modes % 10);
				modes /= 10;
				var cMode = (ParamMode)(modes % 10);
				return new Command(opcode, aMode, bMode, cMode);
			}

			long param1(Command cmd) => GetVariable(instructionPointer + 1, cmd.aMode);
			long param2(Command cmd) => GetVariable(instructionPointer + 2, cmd.bMode);

			long x = 0, y = 0;
			long score = 0;
			LVec pos;
			LVec dirVec = new LVec(0, 0);
			Direction dir;
			private void SetDir(Direction dir) {
				this.dir = dir;
				switch (dir) {
					case Direction.North:
						dirVec = new LVec(0, -1);
						break;
					case Direction.South:
						dirVec = new LVec(0, 1);
						break;
					case Direction.West:
						dirVec = new LVec(-1, 0);
						break;
					case Direction.East:
						dirVec = new LVec(1, 0);
						break;
				}
			}

			enum Direction {
				North = 1, South = 2, West = 3, East = 4
			}

			Dictionary<int, Direction> dirsInOrder = new Dictionary<int, Direction> { { 0, Direction.West }, { 1, Direction.North }, { 2, Direction.East }, { 3, Direction.South } };

			enum RelativeDir { Left, Right }
			void Turn(RelativeDir turn) {
				int next = dirsInOrder.First(x => x.Value == dir).Key + (turn == RelativeDir.Left ? -1 : 1);
				if (next < 0) next = 3;
				else if (next > 3) next = 0;
				var nextDir = dirsInOrder[next];
				SetDir(nextDir);
			}

			public long RunCode(bool part1) {
				int outCnt = 0;
				while (instructionPointer < intCode.Length) {
					var cmd = ParseCommand(intCode[instructionPointer]);
					switch (cmd.opcode) {
						case Opcode.ADD:
							SetVariable(instructionPointer + 3, param1(cmd) + param2(cmd), cmd.cMode);
							instructionPointer += 4;
							break;
						case Opcode.MUL:
							SetVariable(instructionPointer + 3,
								param1(cmd) * param2(cmd), cmd.cMode);
							instructionPointer += 4;
							break;
						case Opcode.IN:
							SetVariable(instructionPointer + 1, (int)dir, cmd.aMode);
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							//Print();
							switch ((StatusCode)param1(cmd)) {
								case StatusCode.Wall: //turn right until we can go forward
									var wall = pos + dirVec;
									if (!painted.ContainsKey(wall)) painted.Add(wall, Tile.Wall);
									Turn(RelativeDir.Right);
									break;
								case StatusCode.Step: //try turn left - always hug wall
									pos += dirVec;
									if (pos == new LVec(0, 0)) {
										var path2 = GetPath(); //part2 - oxygen starts from the end -> longest path from finish
										for (int x = 0; x < path2.GetLength(0); x++) {
											for (int y = 0; y < path2.GetLength(1); y++) {
												if (path2[x, y] == Tile.Finish) path2[x,y] = Tile.Start;
												else if (path2[x, y] == Tile.Start) path2[x, y] = Tile.Finish;
											}
										}
										var f = new Dijkstra();
										var cost = f.GetCost(path2, CostType.LongestPath);
										return cost;
									}
									if (!painted.ContainsKey(pos)) painted.Add(pos, Tile.Path);
									Turn(RelativeDir.Left);
									break;
								case StatusCode.Finish:
									pos += dirVec;
									if (!painted.ContainsKey(pos)) painted.Add(pos, Tile.Finish);
									if (part1) {
										var path = GetPath();
										var d = new Dijkstra();
										var cost = d.GetCost(path);
										return cost;
									}
									break;
							}
							instructionPointer += 2;
							break;
						case Opcode.JIT:
							if (param1(cmd) != 0) instructionPointer = param2(cmd);
							else instructionPointer += 3;
							break;
						case Opcode.JIF:
							if (param1(cmd) == 0) instructionPointer = param2(cmd);
							else instructionPointer += 3;
							break;
						case Opcode.LT:
							SetVariable(instructionPointer + 3, param1(cmd) < param2(cmd) ? 1 : 0, cmd.cMode);
							instructionPointer += 4;
							break;
						case Opcode.EQ:
							SetVariable(instructionPointer + 3, param1(cmd) == param2(cmd) ? 1 : 0, cmd.cMode);
							instructionPointer += 4;
							break;
						case Opcode.RB:
							relativeBase += param1(cmd);
							instructionPointer += 2;
							break;
						case Opcode.HALT:
							return -1; //we dont get here
					}
				}
				return -1;
			}

			private Tile[,] GetPath() {
				if (!painted.Any()) return null;
				//our Vec data structure can have negative coordinates we need to offset so this fits into an array 
				long yMin = painted.Min(x => x.Key.y);
				long yMax = painted.Max(x => x.Key.y);
				long xMin = painted.Min(x => x.Key.x);
				long xMax = painted.Max(x => x.Key.x);
				Tile[,] result = new Tile[xMax - xMin + 1, yMax - yMin + 1];
				long xOffset = 0 - xMin;
				long yOffset = 0 - yMin;
				for (long y = yMin; y <= yMax; y++) {
					for (long x = xMin; x <= xMax; x++) {
						Tile t;
						if (!painted.TryGetValue(new LVec(x, y), out t)) t = Tile.Wall;
						result[x + xOffset, y + yOffset] = t;
					}
				}
				return result;
			}
		}

		protected override long SolvePart1() {
			if (IsShort) return -1;
			var compy = new IntCodeComputer(startCode);
			return compy.RunCode(true);
		}

		protected override long SolvePart2() {
			if (IsShort) return -1;
			var compy = new IntCodeComputer(startCode);
			return compy.RunCode(false);
		}
	}
}
