using Helpers;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day05 {
	public class Day05 : Solver {
		public Day05() : base(2019, 5) {
		}

		int[] intCode;
		int[] startCode;

		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			IsShort = fileName.Contains("short");
			var cmds = File.ReadAllLines(fileName)[0].Split(',');
			startCode = new int[cmds.Length];
			intCode = new int[cmds.Length];
			for (int i = 0; i < cmds.Length; i++) {
				startCode[i] = int.Parse(cmds[i]);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			base.ReadInputPart2(fileName);
			ReadInputPart1(fileName);
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
			STOR = 3,
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
		List<int> output;
		int instructionPointer;
		int inputValue = 1;
		int param1(Command cmd) => GetVariable(instructionPointer + 1, cmd.aMode);
		int param2(Command cmd) => GetVariable(instructionPointer + 2, cmd.bMode);
		int param3(Command cmd) => GetVariable(instructionPointer + 3, cmd.cMode);
		private long RunCode() {
			if (IsShort) return -1;
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
					case Opcode.STOR:
						SetVariable(instructionPointer + 1, inputValue);
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

		protected override long SolvePart1() {
			Reset();
			inputValue = 1;
			return RunCode();
		}
		protected override long SolvePart2() {
			Reset();
			inputValue = 5;
			return RunCode();
		}
	}
}
