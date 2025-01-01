using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day01 {
	public class Day01 : Solver {
		public Day01() : base(2019, 1) {
		}
		List<long> input;
		protected override void ReadInputPart1(string fileName) {
			input = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				input.Add(long.Parse(l));
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var i in input) result += (i / 3) - 2;
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;
			foreach (var i in input) {
				long acc = 0;
				var r = AddDiv(i, ref acc);
				result += r;
			}
			return result;
		}

		private long AddDiv(long input, ref long acc) {
			var r = Math.Max(0, (input / 3) - 2);
			acc += r;
			if (r <= 0) return acc;
			else return AddDiv(r, ref acc);
		}
	}
}
