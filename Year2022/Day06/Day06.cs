using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2022.Day06 {
	public class Day06 : Solver {
		public Day06() : base(2022, 6) {
		}
		string input;
		protected override void ReadInputPart1(string fileName) {
			input = File.ReadAllLines(fileName)[0];
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			List<char> window = input.Substring(0, 4).ToCharArray().ToList();
			for (int i = 4; i < input.Length; i++) {
				if (window.Distinct().Count() == 4) return i;
				window.RemoveAt(0);
				window.Add(input[i]);
			}
			return 0;
		}

		protected override long SolvePart2() {
			List<char> window = input.Substring(0, 14).ToCharArray().ToList();
			for (int i = 14; i < input.Length; i++) {
				if (window.Distinct().Count() == 14) return i;
				window.RemoveAt(0);
				window.Add(input[i]);
			}
			return 0;
		}
	}
}
