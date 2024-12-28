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
		int[] input;

		protected override void ReadInputPart1(string fileName) {
			IsShort = fileName.Contains("short");
			var cmds = File.ReadAllLines(fileName)[0].Split(',');
			input = new int[cmds.Length];
			intCode = new int[cmds.Length];
			for (int i = 0; i < cmds.Length; i++) {
				input[i] = int.Parse(cmds[i]);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		int GetVariable(int pos) {
			return intCode[intCode[pos]];
		}
		void SetVariable(int pos, int val) {
			intCode[intCode[pos]] = val;
		}

		void Reset() {
			for (int i = 0; i < input.Length; i++) intCode[i] = input[i];
		}

		protected override long SolvePart1() {
			Reset();
			if (!IsShort) SetInput(12, 2);
			return RunCode();
		}
		//enum ParameterMode { //enum.parse works with reflection lets not slow this down yet
		//	PositionMode = 0,
		//	ImmediateMode = 1
		//};
		//enum Operation {
		//	ADD = 1,
		//	MUL = 2,
		//	STOR = 3,
		//	OUT = 4,
		//	HALT = 99
		//}
		record Command(int opcode, int aMode, int bMode, int cMode);
		Command ParseCommand(int cmdNum) {
			int opcode = cmdNum % 100;
			int modes = cmdNum / 100;
			int aMode = modes % 10;
			modes /= 10;
			int bMode = modes % 10;
			modes /= 10;
			int cMode = modes % 10;
			return new Command(opcode, aMode, bMode, cMode);
		}

		private long RunCode() {
			for (int i = 0; i < intCode.Length; i += 4) {
				switch (intCode[i]) {
					case 1:
						SetVariable(i + 3, GetVariable(i + 1) + GetVariable(i + 2));
						break;
					case 2:
						SetVariable(i + 3, GetVariable(i + 1) * GetVariable(i + 2));
						break;
					case 3:
						SetVariable(i + 1, GetVariable(i + 2));
						break;
					case 4:
						Output(GetVariable(i + 1));
						break;
					case 99:
						return intCode[0];
				}
			}
			return intCode[0];
		}

		private void Output(int v) {
			throw new NotImplementedException();
		}

		private void SetInput(int a, int b) {
			intCode[1] = a;
			intCode[2] = b;
		}

		protected override long SolvePart2() {
			if (IsShort) return -1;
			for (int a = 0; a <= 99; a++) {
				for (int b = 0; b <= 99; b++) {
					Reset();
					SetInput(a, b);
					var result = RunCode();
					if (result == 19690720) return 100 * a + b;
				}
			}
			return -1;
		}
	}
}
