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


			long lastOut;
			public long RunCode(LVec pos) {
				bool isX = true;
				while (instructionPointer < intCode.Length) {
					var cmd = ParseCommand(intCode[instructionPointer]);
					//Console.WriteLine(cmd);
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
			//seems we always halt after getting 1 output?
			tiles = new char?[50, 50];
			long result = 0;
			var compy = new IntCodeComputer(startCode);
			for (int x = 0; x < tiles.GetLength(0); x++) {
				for (int y = 0; y < tiles.GetLength(1); y++) {
					var res = compy.RunCode(new LVec(x, y));
					if (res == 1) result++;
					tiles[x, y] = res == 1 ? '#' : '.';
					compy.Reset();
				}
			}
			//Debug();
			return result;
		}

		//11442 ms - should be much better
		protected override long SolvePart2() {
			base.SolvePart2();
			if (IsShort) return -1;
			var compy = new IntCodeComputer(startCode);
			int squareSize = 100;
			for (int y = 100; y < int.MaxValue; y++) {
				int beamCnt = 0;
				for (int x = 0; x < int.MaxValue; x++) {
					var res = compy.RunCode(new LVec(x, y));
					if (res == 1) beamCnt++;
					if (beamCnt > 0 && res == 0) break; //we are out of the beam
					if (beamCnt == squareSize) {
						//we need to draw a line up from 10 square from the 10th tractor beam square
						//if that is also a tractor beam square, we have a square of 10x10 fit inside the beam
						compy.Reset();
						var up = compy.RunCode(new LVec(x, y - (squareSize - 1)));
						if (up == 1) {
							var ul = new LVec(x - (squareSize - 1), y - (squareSize - 1));
							var ur = new LVec(x, y - (squareSize - 1));
							var dl = new LVec(x - (squareSize - 1), y);
							var dr = new LVec(x, y);
							CheckCube(ul, ur, dl, dr);
							return (ul.x * 10000) + ul.y;
						} else break;
					}
					compy.Reset();
				}
			}
			return -1;
		}

		private void CheckCube(LVec ul, LVec ur, LVec dl, LVec dr) {
			var compy = new IntCodeComputer(startCode);
			if (compy.RunCode(ul) != 1) throw new Oopsie($"{ul} outside of beam");
			compy.Reset();
			if (compy.RunCode(ur) != 1) throw new Oopsie($"{ur} outside of beam");
			compy.Reset();
			if (compy.RunCode(dl) != 1) throw new Oopsie($"{dl} outside of beam");
			compy.Reset();
			if (compy.RunCode(dr) != 1) throw new Oopsie($"{dr} outside of beam");
		}
	}

}
