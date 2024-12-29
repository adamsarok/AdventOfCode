using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day07 {
	public class Day07 : Solver {
		public Day07() : base(2019, 7) {
		}

		int[] startCode;

		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			IsShort = fileName.Contains("short");
			var cmds = File.ReadAllLines(fileName)[0].Split(',');
			startCode = new int[cmds.Length];
			for (int i = 0; i < cmds.Length; i++) {
				startCode[i] = int.Parse(cmds[i]);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			base.ReadInputPart2(fileName);
			ReadInputPart1(fileName);
		}

		class IntCodeComputer {
			int[] intCode;
			List<int> output;
			int instructionPointer;
			int[] startCode { get; }
			public IntCodeComputer(int[] startCode) {
				intCode = new int[startCode.Length];
				this.startCode = startCode;
				Reset();
			}

			int GetVariable(int pos, ParamMode mode) {
				if (mode == ParamMode.Position) return intCode[intCode[pos]];
				else return intCode[pos];
			}
			void SetVariable(int pos, int val) {
				intCode[intCode[pos]] = val;
			}

			void Reset() {
				instructionPointer = 0;
				output = new List<int>();
				for (int i = 0; i < startCode.Length; i++) intCode[i] = startCode[i];
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
				HALT = 99
			}
			enum ParamMode {
				Position = 0,
				Immediate = 1
			}
			record Command(Opcode opcode, ParamMode aMode, ParamMode bMode, ParamMode cMode);
			Command ParseCommand(int cmdNum) { //ABCDE - A=3rd par B=2nd par C=3rd par in the example BUT - I implemented with 1st par = A for simplicity
				var opcode = (Opcode)(cmdNum % 100);
				if (opcode == 0) throw new Oopsie();
				int modes = cmdNum / 100;
				var aMode = (ParamMode)(modes % 10);
				modes /= 10;
				var bMode = (ParamMode)(modes % 10);
				modes /= 10;
				var cMode = (ParamMode)(modes % 10);
				return new Command(opcode, aMode, bMode, cMode);
			}

			int param1(Command cmd) => GetVariable(instructionPointer + 1, cmd.aMode);
			int param2(Command cmd) => GetVariable(instructionPointer + 2, cmd.bMode);
			int param3(Command cmd) => GetVariable(instructionPointer + 3, cmd.cMode);
			public int RunCode(int phase, int input) {
				Reset();
				int inpCount = 0;
				int[] inputs = new int[] { phase, input };
				while (instructionPointer < intCode.Length) {
					var cmd = ParseCommand(intCode[instructionPointer]);
					switch (cmd.opcode) {
						case Opcode.ADD:
							SetVariable(instructionPointer + 3, param1(cmd) + param2(cmd));
							instructionPointer += 4;
							break;
						case Opcode.MUL:
							SetVariable(instructionPointer + 3,
								param1(cmd) * param2(cmd));
							instructionPointer += 4;
							break;
						case Opcode.IN:
							SetVariable(instructionPointer + 1, inputs[inpCount++]);
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							Output(param1(cmd));
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
							SetVariable(instructionPointer + 3, param1(cmd) < param2(cmd) ? 1 : 0);
							instructionPointer += 4;
							break;
						case Opcode.EQ:
							SetVariable(instructionPointer + 3, param1(cmd) == param2(cmd) ? 1 : 0);
							instructionPointer += 4;
							break;
						case Opcode.HALT:
							return output.Last();
					}
				}
				return -1;
			}

			private void Output(int v) {
				output.Add(v);
			}
		}

		class IntCodeComputerPart2 {
			int[] intCode;
			List<int> output;
			int instructionPointer;
			private readonly int id;
			private readonly int phase;
			int inpCount = 0;

			public IntCodeComputerPart2(int id, int[] startCode, int phase) {
				intCode = new int[startCode.Length];
				this.id = id;
				this.phase = phase;
				instructionPointer = 0;
				inpCount = 0;
				output = new List<int>();
				for (int i = 0; i < startCode.Length; i++) intCode[i] = startCode[i];
			}

			int GetVariable(int pos, ParamMode mode) {
				if (mode == ParamMode.Position) return intCode[intCode[pos]];
				else return intCode[pos];
			}
			void SetVariable(int pos, int val) {
				intCode[intCode[pos]] = val;
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
				HALT = 99
			}
			enum ParamMode {
				Position = 0,
				Immediate = 1
			}
			record Command(Opcode opcode, ParamMode aMode, ParamMode bMode, ParamMode cMode);
			Command ParseCommand(int cmdNum) { //ABCDE - A=3rd par B=2nd par C=3rd par in the example BUT - I implemented with 1st par = A for simplicity
				var opcode = (Opcode)(cmdNum % 100);
				if (opcode == 0) throw new Oopsie();
				int modes = cmdNum / 100;
				var aMode = (ParamMode)(modes % 10);
				modes /= 10;
				var bMode = (ParamMode)(modes % 10);
				modes /= 10;
				var cMode = (ParamMode)(modes % 10);
				return new Command(opcode, aMode, bMode, cMode);
			}

			int param1(Command cmd) => GetVariable(instructionPointer + 1, cmd.aMode);
			int param2(Command cmd) => GetVariable(instructionPointer + 2, cmd.bMode);
			int param3(Command cmd) => GetVariable(instructionPointer + 3, cmd.cMode);
			public enum ResultType { OUT, HALT, ERROR };
			public record Result(ResultType ResultType, int Value);
			public Result RunCode(int input) {
				//Console.WriteLine($"Computer {id} is (re)starting with input {(inpCount == 0 ? phase : input)} at instruction pointer {instructionPointer}");
				while (instructionPointer < intCode.Length) {
					var cmd = ParseCommand(intCode[instructionPointer]);
					switch (cmd.opcode) {
						case Opcode.ADD:
							SetVariable(instructionPointer + 3, param1(cmd) + param2(cmd));
							instructionPointer += 4;
							break;
						case Opcode.MUL:
							SetVariable(instructionPointer + 3,
								param1(cmd) * param2(cmd));
							instructionPointer += 4;
							break;
						case Opcode.IN:
							SetVariable(instructionPointer + 1, inpCount == 0 ? phase : input);
							inpCount++;
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							var val = param1(cmd);
							output.Add(val);
							instructionPointer += 2;
							return new Result(ResultType.OUT, val);
						case Opcode.JIT:
							if (param1(cmd) != 0) instructionPointer = param2(cmd);
							else instructionPointer += 3;
							break;
						case Opcode.JIF:
							if (param1(cmd) == 0) instructionPointer = param2(cmd);
							else instructionPointer += 3;
							break;
						case Opcode.LT:
							SetVariable(instructionPointer + 3, param1(cmd) < param2(cmd) ? 1 : 0);
							instructionPointer += 4;
							break;
						case Opcode.EQ:
							SetVariable(instructionPointer + 3, param1(cmd) == param2(cmd) ? 1 : 0);
							instructionPointer += 4;
							break;
						case Opcode.HALT:
							return new Result(ResultType.HALT, output.Last());
					}
				}
				return new Result(ResultType.ERROR, -1);
			}
		}
		protected override long SolvePart1() {
			if (IsShort) return -1;
			var amps = new int[] { 0, 1, 2, 3, 4 };
			var computers = Enumerable.Repeat(new IntCodeComputer(startCode), 5).ToList();
			int maxSignal = int.MinValue;
			foreach (var p in amps.Permute()) {
				var permutations = p.ToList();
				int output = 0;
				for (int i = 0; i < computers.Count; i++) {
					output = computers[i].RunCode(permutations[i], output);
				}
				maxSignal = Math.Max(output, maxSignal);
			}
			return maxSignal;
		}
		protected override long SolvePart2() {
			var amps = new int[] { 5, 6, 7, 8, 9 };
			int maxSignal = int.MinValue;
			foreach (var p in amps.Permute()) {
				var permutations = p.ToList();
				List<IntCodeComputerPart2> computers = new List<IntCodeComputerPart2>();
				int compid = 0;
				foreach (var perm in permutations) {
					computers.Add(new IntCodeComputerPart2(compid, startCode, permutations[compid]));
					compid++;
				}
				var output = new IntCodeComputerPart2.Result(IntCodeComputerPart2.ResultType.OUT, 0);
				while (output.ResultType == IntCodeComputerPart2.ResultType.OUT) {
					for (int i = 0; i < computers.Count; i++) {
						output = computers[i].RunCode(output.Value);
					}
				}
				maxSignal = Math.Max(output.Value, maxSignal);
			}
			return maxSignal;
		}
	}
}