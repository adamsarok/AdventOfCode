using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day19 {
	public class Day19 : Solver {
		public Day19() : base(2019, 19) {
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
			long relativeBase;
			long instructionPointer;
			private long[] startCode;


			public IntCodeComputer(long[] startCode) {
				intCode = new long[10000];
				this.startCode = startCode;
				Reset();
			}

			public void Reset() {
				instructionPointer = 0;
				relativeBase = 0;
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


			long lastOut;
			public long RunCode(LVec pos) {
				bool isX = true;
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
							SetVariable(instructionPointer + 1, isX ? pos.x : pos.y, cmd.aMode);
							isX = !isX;
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							lastOut = param1(cmd);
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
							return lastOut;
					}
				}
				return -1;
			}
		}


		private char?[,] tiles;
		

		protected override long SolvePart1() {
			base.SolvePart1();
			if (IsShort) return -1;
			tiles = new char?[50, 50];
			long result = 0;
			var compy = new IntCodeComputer(startCode);
			for (int x = 0; x < tiles.GetLength(0); x++) {
				for (int y = 0; y < tiles.GetLength(1); y++) {
					var res = compy.RunCode(new LVec(x, y));
					if (res == 1) result++;
					tiles[x, y] = res == 1 ? '#' : '.';
					compy.Reset(); //seems we always halt after getting 1 output
				}
			}
			//Debug();
			return result;
		}

		//11442 ms - should be much better
		//9900 ms - better
		//75 ms - yep
		protected override long SolvePart2() {
			base.SolvePart2();
			if (IsShort) return -1;
			var compy = new IntCodeComputer(startCode);
			int squareSize = 100;
			int lastBeam = 1;
			for (int y = 100; y < int.MaxValue; y++) {
				for (int x = lastBeam - 1; x < int.MaxValue; x++) {
					var res = compy.RunCode(new LVec(x, y));
					compy.Reset();
					if (res == 1) { //if up + 99, right + 99 is in beam we have a 100 square
						lastBeam = x;
						var up = compy.RunCode(new LVec(x + (squareSize - 1), y - (squareSize - 1)));
						compy.Reset();
						if (up == 1) return (x * 10000) + (y - (squareSize - 1));
						else break;
					}
				}
			}
			return -1;
		}
	}
}
