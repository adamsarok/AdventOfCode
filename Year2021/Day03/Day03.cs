using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2021.Day03 {
	public class Day03 : Solver {
		private string[] input;

		public Day03() : base(2021, 3) {
		}

		protected override void ReadInputPart1(string fileName) {
			input = File.ReadAllLines(fileName);
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long gamma = 0, epsilon = 0;
			long pow = input[0].Length - 1;
			for (int x = 0; x < input[0].Length; x++) {
				int ones = 0;
				for (int y = 0; y < input.Length; y++) {
					if (input[y][x] == '1') ones++;
				}
				if (ones > input.Length / 2) {
					gamma += (long)Math.Pow(2, pow);
				} else {
					epsilon += (long)Math.Pow(2, pow);
				}
				pow--;
			}
			return gamma * epsilon;
		}

		protected override long SolvePart2() {
			//TODO: thimk
			return 0;
		}
	}
}
