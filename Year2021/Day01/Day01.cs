using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2021.Day01 {
	public class Day01 : Solver {
		private string[] input;

		public Day01() : base(2021, 1) {
		}
		protected override void ReadInputPart1(string fileName) {
			input = File.ReadAllLines(fileName);
		}

		protected override void ReadInputPart2(string fileName) {
			input = File.ReadAllLines(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			long last = long.MaxValue;
			foreach (var l in input) {
				long n = long.Parse(l);
				if (last < n) result++;
				last = n;
			}
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;
			long window = long.Parse(input[0]) 
				+ long.Parse(input[1])
				+ long.Parse(input[2]);
			for (int i = 3; i < input.Length; i++) {
				long nextWindow = window + long.Parse(input[i]) - long.Parse(input[i - 3]);
				if (window < nextWindow) result++;
				window = nextWindow;
			}
			return result;
		}
	}
}
