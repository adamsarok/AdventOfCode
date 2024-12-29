using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day13 {
	public class Day13 : Solver {
		public Day13() : base(2019, 13) {
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
			const char PADDLE = '-';
			const char BALL = 'O';
			char[] blocks = new char[] {
				' ',
				'#', //wall
				'X', //destroyable block
				PADDLE,
				BALL, //ball
			};
			Dictionary<LVec, char> painted;
			private long[] startCode;


			public IntCodeComputer(long[] startCode) {
				intCode = new long[10000];
				instructionPointer = 0;
				painted = new();
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
			void SetPainted(LVec pos, char c) {
				if (painted.ContainsKey(pos)) painted[pos] = c;
				else painted.Add(pos, c);
				if (c == BALL) ballPos = pos;
				else if (c == PADDLE) paddlePos = pos;
			}
			long x = 0, y = 0;
			long score = 0;
			LVec joystick = new LVec(0, 0);
			LVec? ballPos, paddlePos;
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
							if (intCode[0] == 2) MatchBall();
							SetVariable(instructionPointer + 1, joystick.x, cmd.aMode);
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							if (outCnt == 0) {
								x = param1(cmd);
								outCnt++;
							} else if (outCnt == 1) {
								y = param1(cmd);
								outCnt++;
							} else {
								if (x > 0) SetPainted(new LVec(x, y), blocks[param1(cmd)]);
								else score = param1(cmd);
								outCnt = 0;
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
							//Print();
							if (intCode[0] == 2) return score;
							return painted.Where(x => x.Value == 'X').Count();
					}
				}
				return -1;
			}

			//I will keep this for posterity's sake - I overcomplicated this and it does not work
			//private void Predict() {
			//	//for each tick, we have to predict where the ball would intersect the paddle to keep the ball in game
			//	//tick == Opcode.IN
			//	if (ballVec == null || ballVec.y <= 0) {
			//		joystick = new LVec(0, 0);
			//		interceptPos = null;
			//		return; //lets try first to move the paddle only if the ball is coming downward, otherwise this gets complicated
			//	}
			//	
			//	//we are too slow and cant catch the ball this way - in reality we only need to match ball.x with paddle.x always
			//	var interceptTicks = paddlePos.y - ballPos.y;
			//	interceptPos = ballPos + (ballVec * interceptTicks);
			//	if (interceptPos.x < paddlePos.x) joystick = new LVec(-1, 0);
			//	else if (interceptPos.x > paddlePos.x) joystick = new LVec(1, 0);
			//	else joystick = new LVec(0, 0);
			//}

			private void MatchBall() {
				if (ballPos.x < paddlePos.x) joystick = new LVec(-1, 0);
				else if (ballPos.x > paddlePos.x) joystick = new LVec(1, 0);
				else joystick = new LVec(0, 0);
			}

			private void Print() {
				Console.Clear();
				for (long y = painted.Min(x => x.Key.y); y <= painted.Max(x => x.Key.y); y++) {
					for (long x = painted.Min(x => x.Key.x); x <= painted.Max(x => x.Key.x); x++) {
						char block;
						var vec = new LVec(x, y);
						if (!painted.TryGetValue(new LVec(x, y), out block)) block = ' ';
						Console.Write(block);
					}
					Console.WriteLine();
				}
				Console.WriteLine($"Score: {score}");
				Thread.Sleep(1);
			}
		}

		protected override long SolvePart1() {
			if (IsShort) return -1;
			var compy = new IntCodeComputer(startCode);
			return compy.RunCode();
		}

		protected override long SolvePart2() {
			if (IsShort) return -1;
			startCode[0] = 2;
			var compy = new IntCodeComputer(startCode);
			//this is such a cool idea
			return compy.RunCode();
		}
	}
}
