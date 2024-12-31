using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day21 {
	public class Day21 : Solver {
		public Day21() : base(2019, 21) {
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

			long GetVariable(long pos, long mode) {
				if (mode == 0) return intCode[intCode[pos]];
				if (mode == 1) return intCode[pos];
				if (mode == 2) return intCode[intCode[pos] + relativeBase];
				throw new Oopsie("unknown ParamMode");
			}
			void SetVariable(long pos, long val, long mode) {
				if (mode == 0) intCode[intCode[pos]] = val;
				else if (mode == 1) intCode[intCode[pos]] = val;
				else if (mode == 2) intCode[intCode[pos] + relativeBase] = val;
				else throw new Oopsie("unknown ParamMode");
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

			long param1(long mode) => GetVariable(instructionPointer + 1, mode);
			long param2(long mode) => GetVariable(instructionPointer + 2, mode);

			List<long> output;
			char[] input;
			private void SetCommands(List<Command> commands, bool isFirstPart) {
				StringBuilder temp = new StringBuilder();
				foreach (var c in commands) {
					temp.Append(s1[c.a]).Append(' ');
					if (isFirstPart) temp.Append(s2[c.b]);
					else temp.Append(s2_part2[c.b]);
					temp.Append(' ').Append(s3[c.c]).Append('\n');
				}
				temp.Append(isFirstPart ? "WALK\n" : "RUN\n");
				input = temp.ToString().ToCharArray();
			}

			static string[] s1 = new string[] { "AND", "OR", "NOT" };
			static string[] s2 = new string[] { "A", "B", "C", "D", "T", "J" };
			static string[] s2_part2 = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "T", "J" };
			static string[] s3 = new string[] { "T", "J" };

			public long RunCode(List<Command> commands, bool isFirstPart) {
				SetCommands(commands, isFirstPart);
				output = new List<long>();
				int actInput = 0;
				while (instructionPointer < intCode.Length) {
					long cmdNum = intCode[instructionPointer];
					var opcode = (Opcode)(cmdNum % 100);
					if (opcode == 0) throw new Oopsie();
					long modes = cmdNum / 100;
					var aMode = modes % 10;
					modes /= 10;
					var bMode = modes % 10;
					modes /= 10;
					var cMode = modes % 10;
					switch (opcode) {
						case Opcode.ADD:
							SetVariable(instructionPointer + 3, param1(aMode) + param2(bMode), cMode);
							instructionPointer += 4;
							break;
						case Opcode.MUL:
							SetVariable(instructionPointer + 3,
								param1(aMode) * param2(bMode), cMode);
							instructionPointer += 4;
							break;
						case Opcode.IN:
							SetVariable(instructionPointer + 1, input[actInput], aMode);
							actInput++;
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							var c = param1(aMode);
							if (c == 'D') {
								return -1; //Didn't make it across, return early
							}
							output.Add(c);
							instructionPointer += 2;
							break;
						case Opcode.JIT:
							if (param1(aMode) != 0) instructionPointer = param2(bMode);
							else instructionPointer += 3;
							break;
						case Opcode.JIF:
							if (param1(aMode) == 0) instructionPointer = param2(bMode);
							else instructionPointer += 3;
							break;
						case Opcode.LT:
							SetVariable(instructionPointer + 3, param1(aMode) < param2(bMode) ? 1 : 0, cMode);
							instructionPointer += 4;
							break;
						case Opcode.EQ:
							SetVariable(instructionPointer + 3, param1(aMode) == param2(bMode) ? 1 : 0, cMode);
							instructionPointer += 4;
							break;
						case Opcode.RB:
							relativeBase += param1(aMode);
							instructionPointer += 2;
							break;
						case Opcode.HALT:
							return output.Last();
					}
				}
				return -1;
			}
		}


		private char?[,] tiles;

		List<Command> allCommands;
		private void SetAllCommands() {
			allCommands = new List<Command>();
			for (byte a = 0; a < 3; a++) {
				for (byte b = 0; b < (IsPart1 ? 6 : 11); b++) {
					for (byte c = 0; c < 2; c++) {
						//in 2nd part A and D is already used
						if (!IsPart1 && (b == 0 || b == 3)) continue;
						allCommands.Add(new Command(a, b, c));
					}
				}
			}
		}

		record struct Command(byte a, byte b, byte c);

		Command NOT_A_J = new Command(2, 0, 1);
		Command AND_D_J = new Command(0, 3, 1);

		Command SECOND_NOT_A_T = new Command(2, 0, 0);
		Command SECOND_OR_T_J = new Command(1, 9, 1);
		Command SECOND_AND_D_J = new Command(0, 3, 1);


		protected override long SolvePart1() {
			base.SolvePart1();
			if (IsShort) return -1;
			SetAllCommands();
			return Calc(true);
		}

		private long Calc(bool isFirstPart) {
			int commandsToAdd = 1;
			int threadCount = Environment.ProcessorCount / 2;
			Stopwatch sw = Stopwatch.StartNew();
			List<string> winner = new List<string>();
			while (true) {
				List<Task<long>> tasks = new List<Task<long>>();
				BlockingCollection<List<Command>> commandQueue = new BlockingCollection<List<Command>>();
				Task producerTask = Task.Factory.StartNew(() => {
					foreach (var combination in allCommands.Combinations(commandsToAdd)) {
						bool valid = true;
						HashSet<byte> bs = new HashSet<byte>();
						foreach (var b in combination) {
							if (bs.Contains(b.b)) {
								valid = false; //assume each register A,B,C etc can only appear once in a command set
								continue;
							}
							bs.Add(b.b);
						}
						if (!valid) continue;
						var commands = combination.ToList();
						commandQueue.Add(combination.ToList());
					}
					commandQueue.CompleteAdding();
				});

				Timer t = new Timer((o) => {
					if (commandQueue.Any()) Console.WriteLine($"Commands left: {commandQueue.Count}");
				}, null, 10000, 10000);

				for (int i = 0; i < threadCount; i++) {
					tasks.Add(Task.Factory.StartNew(() => {
						var compy = new IntCodeComputer(startCode);
						foreach (var commands in commandQueue.GetConsumingEnumerable()) {
							compy.Reset();
							var cmdsLocal = isFirstPart ?
								commands.Prepend(NOT_A_J).Append(AND_D_J).ToList() :
								commands.Append(SECOND_NOT_A_T).Append(SECOND_OR_T_J).Append(SECOND_AND_D_J).ToList();
							var r = compy.RunCode(cmdsLocal, isFirstPart);
							if (r > 0) return r;		
						}
						return -1;
					}));
				}
				Task.WaitAll(tasks.ToArray());
				long result = tasks.Max(t => t.Result);
				Console.WriteLine($"Tried: {(isFirstPart ? commandsToAdd + 2 : commandsToAdd + 3)} commands in {sw.ElapsedMilliseconds} ms.");
				if (result > 0) {
					foreach (var w in winner) Console.WriteLine(w);
					return result;
				}
				commandsToAdd++;
				if (commandsToAdd > 5) return -1;
			}
		}

		protected override long SolvePart2() {
			base.SolvePart2();
			if (IsShort) return -1;
			SetAllCommands();
			return Calc(false);
		}
		
	}
}

