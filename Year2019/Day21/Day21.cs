using Helpers;
using System;
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

			List<char> output;
			char[] input;
			private void SetCommands(List<string> commands) {
				input = (string.Join("\n", commands) + "\nWALK\n").ToCharArray();
			}

			public string RunCode(List<string> commands, out long instructionsRan) {
				SetCommands(commands);
				output = new List<char>();
				int actInput = 0;
				instructionsRan = 0;
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
					instructionsRan++;
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
							output.Add((char)param1(aMode));
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
							return new string(output.ToArray());
					}
				}
				return "-1";
			}
		}


		private char?[,] tiles;
		string[] s1 = new string[] { "AND", "OR", "NOT" };
		string[] s2 = new string[] { "A", "B", "C", "T", "J" };
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

		//To get to 5 commands:
		//1st: 18103 ms
		//2nd: 9159 ms
		//3rd: 8032 ms
		//4nd: 7563 ms

		protected override long SolvePart1() {
			base.SolvePart1();
			if (IsShort) return -1;
			var compy = new IntCodeComputer(startCode);
			string output = "Didn't make it across";
			SetAllCommands();
			int actComm = 0;
			List<string> commandsStart = new List<string>() {
				"NOT A J"
			};
			List<string> commands;
			int commandsToAdd = 1;
			long maxRunLength = 0;
			Stopwatch sw = Stopwatch.StartNew();
			while (output.Contains("Didn't") || output.Contains("?")) { //I am really not figuring this out right now
				foreach (var combination in allCommands.Combinations(commandsToAdd)) {
					var list = combination.ToList();
					bool valid = true;
					for (int i = 0; i < list.Count - 1; i++) if (list[i + 1] == list[i]) valid = false;
					if (!valid) continue;
					commands = new List<string>(commandsStart);
					commands.AddRange(combination);
					commands.Add("AND D J"); //this is surely the last command?
					long instructionsRan = 0;
					compy.Reset();
					var r = compy.RunCode(commands, out instructionsRan);
					maxRunLength = Math.Max(maxRunLength, instructionsRan);
					//Console.WriteLine($"Tried: {string.Join(",", combination)} - went {instructionsRan} long");
					//foreach (var l in r.Split('\n')) {
					//	Console.WriteLine(l);
					//}
				}
				Console.WriteLine($"Tried: {commandsToAdd + 2} commands in {sw.ElapsedMilliseconds} ms. Went {maxRunLength} long");
				commandsToAdd++;
				if (commandsToAdd > 4) {
					return -1;
				}
			}
			return -1;
		}

		//11442 ms - should be much better
		//9900 ms - better
		//75 ms - yep
		protected override long SolvePart2() {
			base.SolvePart2();
			if (IsShort) return -1;

			return -1;
		}
	}
}

