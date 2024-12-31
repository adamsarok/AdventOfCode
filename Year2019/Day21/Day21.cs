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
			//record Command(Opcode opcode, ParamMode aMode, ParamMode bMode, ParamMode cMode);

			long param1(long mode) => GetVariable(instructionPointer + 1, mode);
			long param2(long mode) => GetVariable(instructionPointer + 2, mode);

			List<long> output;
			char[] input;
			private void SetCommands(List<string> commands) {
				input = string.Join("\n", commands).ToCharArray();
			}

			public long RunCode(List<string> commands) {
				SetCommands(commands);
				output = new List<long>();
				int actInput = 0;
				//instructionsRan = 0;
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
					//Console.WriteLine(instructionPointer);
					//instructionsRan++;
					//var cmd = ParseCommand(intCode[instructionPointer]);
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
							//return new string(output.ToArray());
							return output.Last();
					}
				}
				return -1;
			}
		}


		private char?[,] tiles;
		string[] s1 = new string[] { "AND", "OR", "NOT" };
		string[] s2 = new string[] { "B", "C", "T", "J" }; //A and D can be skipped as that is the last and first
		string[] s2_part2 = new string[] { "B", "C", "E", "F", "G", "H", "I", "T", "J" };
		string[] s3 = new string[] { "T", "J" };
		List<string> allCommands;
		private void SetAllCommands() {
			allCommands = new List<string>();
			foreach (var a in s1) {
				foreach (var b in s2) {
					foreach (var c in s3) {
						allCommands.Add($"{a} {b} {c}");
					}
				}
			}
		}
		private void SetAllCommandsPart2() {
			allCommands = new List<string>();
			foreach (var a in s1) {
				foreach (var b in s2_part2) {
					foreach (var c in s3) {
						allCommands.Add($"{a} {b} {c}");
					}
				}
			}
		}

		//1st: 18103 ms no result
		//2nd: 9159 ms no result
		//3rd: 8032 ms no result
		//4nd: 7563 ms no result
		//16 threads: 2000 ms no result
		//last try: 245 ms success!
		record struct Command(byte a, byte b, byte c);

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
				BlockingCollection<List<string>> commandQueue = new BlockingCollection<List<string>>();
				//foreach (var partition in allCommands.Combinations(commandsToAdd)) commandQueue.Enqueue(partition.ToList());
				Task producerTask = Task.Factory.StartNew(() => {
					foreach (var partition in allCommands.Combinations(commandsToAdd)) {
						commandQueue.Add(partition.ToList());
					}
					commandQueue.CompleteAdding();
				});


				for (int i = 0; i < threadCount; i++) {
					tasks.Add(Task.Factory.StartNew(() => {
						//List<string> commands;
						var compy = new IntCodeComputer(startCode);
						foreach (var commands in commandQueue.GetConsumingEnumerable()) {
							bool valid = true;
							for (int i = 0; i < commands.Count - 1; i++) if (commands[i + 1] == commands[i]) valid = false;
							if (!valid) continue;
							compy.Reset();
							List<string> cmdsLocal = isFirstPart ? commands.Prepend("NOT A J").Append($"AND D J\nWALK\n").ToList() : commands.Append("\nRUN\n").ToList();
							var r = compy.RunCode(cmdsLocal);
							if (r > 0) {
								winner = cmdsLocal;
								return r;
							}
						}
						return -1;
					}));
				}
				Task.WaitAll(tasks.ToArray());
				long result = tasks.Max(t => t.Result);
				Console.WriteLine($"Tried: {(isFirstPart ? commandsToAdd + 2 : commandsToAdd)} commands in {sw.ElapsedMilliseconds} ms.");
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
			SetAllCommandsPart2();
			return Calc(false);
		}
		
	}
}

