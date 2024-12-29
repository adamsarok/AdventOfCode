using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day11 {
	public class Day11 : Solver {
		public Day11() : base(2019, 11) {
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
			List<long> output;
			long instructionPointer;


			Vec pos, facing;
			public enum Painted { Black, White }
			Dictionary<Vec, Painted> painted;
			private long[] startCode;


			public IntCodeComputer(long[] startCode, Painted startOn) {
				intCode = new long[10000];
				instructionPointer = 0;
				output = new List<long>();
				pos = new Vec(0, 0);
				facing = new Vec(0, -1);
				painted = new Dictionary<Vec, Painted>() {
					{ pos, startOn },
				};
				for (int i = 0; i < startCode.Length; i++) intCode[i] = startCode[i];
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
			Painted GetPainted() {
				Painted color;
				if (painted.TryGetValue(pos, out color)) return color;
				return Painted.Black;
			}
			void SetPainted(Painted color) {
				if (painted.ContainsKey(pos)) painted[pos] = color;
				else painted.Add(pos, color);
			}
			enum Dir {
				Left = 0, Right = 1
			}
			Dictionary<int, Vec> facings = new Dictionary<int, Vec> {
				{ 0, new Vec(0, -1) }, //clockwise = right
				{ 1, new Vec(1, 0) },
				{ 2, new Vec(0, 1) },
				{ 3, new Vec(-1, 0) },
			};
			void Turn(Dir dir) {
				int next = facings.Where(x => x.Value == facing).First().Key;
				next += dir == Dir.Right ? 1 : -1;
				if (next > 3) facing = facings[0];
				else if (next < 0) facing = facings[3];
				else facing = facings[next];
			}
			public long RunCode() {
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
							SetVariable(instructionPointer + 1, (long)GetPainted(), cmd.aMode);
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							if (outCnt % 2 == 0) {
								SetPainted((Painted)param1(cmd));
							} else {
								Turn((Dir)param1(cmd));
								pos += facing;
							}
							outCnt++;
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
							Print();
							return painted.Count();
					}
				}
				return -1;
			}

			private void Print() {
				for (int y = painted.Min(x => x.Key.y); y <= painted.Max(x => x.Key.y); y++) {
					for (int x = painted.Min(x => x.Key.x); x <= painted.Max(x => x.Key.x); x++) {
						Painted color;
						if (!painted.TryGetValue(new Vec(x, y), out color)) color = Painted.Black;
						Console.Write(color == Painted.Black ? "." : "#");
					}
					Console.WriteLine();
				}
			}
		}

		protected override long SolvePart1() {
			if (IsShort) return -1;
			var compy = new IntCodeComputer(startCode, IntCodeComputer.Painted.Black);
			return compy.RunCode();
		}

		protected override long SolvePart2() {
			if (IsShort) return -1;
			var compy = new IntCodeComputer(startCode, IntCodeComputer.Painted.White);
			return compy.RunCode();
		}
	}
}
