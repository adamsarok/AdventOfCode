using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Helpers.Dijkstra;

namespace Year2019.Day17 {
	public class Day17 : Solver {
		public Day17() : base(2019, 17) {
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
			Dictionary<LVec, char> tiles;
			private long[] startCode;

			public IntCodeComputer(long[] startCode) {
				intCode = new long[10000];
				this.startCode = startCode;
				Reset();
			}

			private void Reset() {
				instructionPointer = 0;
				tiles = new();
				for (int i = 0; i < startCode.Length; i++) intCode[i] = startCode[i];
				pos = new LVec(0, 0);
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

			public long RunCode(bool part1) {
				int part2InpCnt = 0;
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
							if (!part1 && part2input.Any() && part2InpCnt < part2input.Count) {
								SetVariable(instructionPointer + 1, part2input[part2InpCnt++], cmd.aMode);
							} else {
								SetVariable(instructionPointer + 1, 0, cmd.aMode);
							}
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							var val = param1(cmd);
							switch (val) {
								case 10:
									pos = new LVec(0, pos.y + 1);
									break;
								default: 
									tiles.Add(pos, (char)val);
									pos += new LVec(1, 0);
									break;
							}
							lastOut = val;
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
							path = GetPath();
							if (part1) return CountIntersections(path);
							if (part2input.Any()) return lastOut;
							//Debug(path);
							Traverse(path);
							Reset();
							break;
					}
				}
				return -1;
			}

			long lastOut;

			LVec start;
			private void Traverse(char[,] path) {
				List<string> pathInput = new List<string>();
				pos = start;
				path[pos.x,pos.y] = 'D';
				while (TurnToClosestScaff(pathInput)) {
					int len = 0;
					while (GetTile(pos + facing) == '#') {
						pos += facing;
						path[pos.x, pos.y] = 'D';
						len++;
					}
					pathInput.Add(len.ToString());
					//Debug(path);
				}
				//Debug(path);
				Compress(pathInput);
			}

			List<long> part2input = new List<long>();
			private void Compress(List<string> commands) {
				Dictionary<string, List<int>> repetitions = new Dictionary<string, List<int>>();
				int windowLength = 2;
				List<string> window = new List<string>();
				while (windowLength < 10) {
					for (int i = 0; i < commands.Count; i++) {
						if (window.Count < windowLength) window.Add(commands[i]);
						else {
							var key = string.Join(",", window);
							if (repetitions.ContainsKey(key)) repetitions[key].Add(i - 1);
							else repetitions.Add(key, new List<int>() { i - 1 });
							window = [.. window[1..], commands[i]];
						}
					}
					windowLength++;
				}
				string resultOrig = string.Join(",", commands);
				List<string> compressionKeys = new List<string>() { "A", "B", "C" };
				var validKeys = repetitions.Where(x => x.Value.Count > 1 && x.Key.Length <= 20).Select(x => x.Key);
				HashSet<string> bestKeys = new HashSet<string>();
				foreach (var combination in validKeys.Combinations(3)) {
					int actKey = 0;
					string result = resultOrig;
					HashSet<string> unique = new HashSet<string>();
					bool ok = true;
					foreach (var key in combination) {
						if (unique.Contains(key) || !result.Contains(key)) {
							ok = false;
							break;
						}
						unique.Add(key); //key is not found in the result this is a suboptimal key choice
						result = result.Replace(key, compressionKeys[actKey]);
						actKey++;
					}
					if (ok && result.Length <= 20) {
						part2input = new List<long>();
						part2input.AddRange(result.ToCharArray().Select(x => (long)x));
						part2input.Add(10);
						foreach (var key in combination) {
							part2input.AddRange(key.ToCharArray().Select(x => (long)x));
							part2input.Add(10);
						}
						part2input.Add((long)'n');
						part2input.Add(10);
						return;
					}
				}
				throw new Oopsie($"Failed to find 20 length compressed string.");
			}
			private char GetTile(LVec pos) {
				if (tiles.TryGetValue(pos, out char c)) return c;
				return ' ';
			}
			private bool TurnToClosestScaff(List<string> path) {
				if (GetTile(pos + facing) == '#') return true;
				if (GetTile(pos + facing.RotateLeft()) == '#') {
					facing = facing.RotateLeft();
					path.Add("L");
					return true;
				}
				if (GetTile(pos + facing.RotateRight()) == '#') {
					facing = facing.RotateRight();
					path.Add("R");
					return true;
				}
				return false;
			}

			private long CountIntersections(char[,] path) {
				long result = 0;
				for (long y = 1; y < path.GetLength(1) - 1; y++) {
					for (long x = 1; x < path.GetLength(0) - 1; x++) {
						if (path[x, y] == '#' && path[x - 1, y] == '#' && path[x + 1, y] == '#' && path[x, y - 1] == '#' && path[x, y + 1] == '#') {
							result += x * y;
							path[x, y] = 'O';
						}
					}
				}
				//Debug(path);
				return result;
			}

			private void Debug(char[,] path) {
				Console.Clear();
				for (long y = 0; y < path.GetLength(1); y++) {
					for (long x = 0; x < path.GetLength(0); x++) {
						Console.Write(path[x,y]);
					}
					Console.WriteLine();
				}
				//Thread.Sleep(1000);
			}

			private LVec pos;
			private LVec facing;
			private char[,] path;

			private char[,] GetPath() {
				if (!tiles.Any()) return null;
				long yMax = tiles.Max(x => x.Key.y);
				long xMax = tiles.Max(x => x.Key.x);
				char[,] result = new char[xMax + 1, yMax + 1];
				for (long y = 0; y <= yMax; y++) {
					for (long x = 0; x <= xMax; x++) {
						char c;
						if (!tiles.TryGetValue(new LVec(x, y), out c)) c = ' ';
						result[x, y] = c;
						switch (c) {
							case '^':
								start = new LVec(x, y);
								facing = new LVec(0, -1);
								break;
							case 'v':
								start = new LVec(x, y);
								facing = new LVec(0, 1);
								break;
							case '<':
								start = new LVec(x, y);
								facing = new LVec(-1, 0);
								break;
							case '>':
								start = new LVec(x, y);
								facing = new LVec(1, 0);
								break;
						}
					}
				}
				return result;
			}
		}

		protected override long SolvePart1() {
			base.SolvePart1();
			if (IsShort) return -1;
			var compy = new IntCodeComputer(startCode);
			return compy.RunCode(IsPart1);
		}

		protected override long SolvePart2() {
			base.SolvePart2();
			if (IsShort) return -1;
			startCode[0] = 2;
			var compy = new IntCodeComputer(startCode);
			return compy.RunCode(IsPart1);
		}
	}
}
