using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day04 {
	public class Day04 : Solver {
		public Day04() : base(2019, 4) {
		}
		int from, to;
		protected override void ReadInputPart1(string fileName) {
			var input = File.ReadAllLines(fileName)[0];
			from = int.Parse(input.Split('-')[0]);
			to = int.Parse(input.Split('-')[1]);
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			for (int i = from; i < to; i++) {
				if (CheckPass(i.ToString())) {
					//Console.WriteLine(i);
					result++;
				}
			}
			return result;
		}

		private bool CheckPass(string v) {
			bool hasSame = false;
			char last = v[0];
			for (int i = 1; i < v.Length; i++) {
				var next = v[i];
				if (last > next) return false;
				else if (last == next) hasSame = true;
				last = next;
			}
			return hasSame;
		}

		private bool CheckPass2(string v) {
			char last = v[0];
			for (int i = 1; i < v.Length; i++) {
				var next = v[i];
				if (last > next) return false;
				last = next;
			}
			return v.GroupBy(x => x).Any(g => g.Count() == 2);
		}

		protected override long SolvePart2() {
			long result = 0;
			for (int i = from; i < to; i++) {
				if (CheckPass2(i.ToString())) {
					//Console.WriteLine(i);
					result++;
				}
			}
			return result;
		}
	}
}
