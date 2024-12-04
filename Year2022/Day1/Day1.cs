using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2022.Day1 {
	public class Day1 : Solver {
		public Day1() : base(1) {
		}
		List<long> input = new List<long>();
		protected override void ReadInputPart1(string fileName) {
			input = new();
			long r = 0;
			foreach (var l in File.ReadAllLines(fileName)) {
				if (string.IsNullOrWhiteSpace(l)) {
					input.Add(r);
					r = 0;
				} else r += long.Parse(l);
			}
			input.Add(r);
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			return input.Max();
		}

		protected override long SolvePart2() {
			return input.OrderByDescending(x => x).Take(3).Sum();
		}
	}
}
