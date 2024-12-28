using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day02 {
	public class Day02 : Solver {
		public Day02() : base(2019, 2) {
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

		private long RunCode() {
			for (int i = 0; i < intCode.Length; i += 4) {
				switch (intCode[i]) {
					case 1:
						SetVariable(i + 3, GetVariable(i + 1) + GetVariable(i + 2));
						break;
					case 2:
						SetVariable(i + 3, GetVariable(i + 1) * GetVariable(i + 2));
						break;
					case 99:
						return intCode[0];
				}
			}
			return intCode[0];
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
