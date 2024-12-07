using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day7 {
	public class Day7 : Solver {
		public Day7() : base(2024, 7) {
		}
		public record EQ(long total, List<long> values) {
			char[] operators;
			char[] opTypes;
			public bool Solve() {
				opTypes = ['+', '*'];
				return CalcCombinations();
			}
			public bool SolvePart2() {
				opTypes = ['+', '*', 'c'];
				return CalcCombinations();
			}
			private bool CalcCombinations() {
				operators = new char[values.Count - 1];
				int totalCombinations = (int)Math.Pow(opTypes.Length, operators.Length);
				for (int i = 0; i < totalCombinations; i++) {
					int temp = i;
					for (int pos = 0; pos < operators.Length; pos++) {
						operators[pos] = opTypes[temp % opTypes.Length];
						temp /= opTypes.Length;
					}
					if (GetResult() == total) return true;
				}
				return false;
			}
			private long GetResult() {
				long result = values[0];
				for (int i = 1; i < values.Count; i++) {
					switch (operators[i - 1]) {
						case '+': result += values[i]; break;
						case '*': result *= values[i]; break;
						case 'c': result = long.Parse(result.ToString() + values[i].ToString()); break;
					}
				}
				return result;
			}
		}
		public List<EQ> equations;
	
		protected override void ReadInputPart1(string fileName) {
			equations = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				var s = l.Split(':');
				var ops = s[1].Split(' ');
				List<long> values = new();
				foreach (var op in ops.Where(x => !string.IsNullOrWhiteSpace(x))) values.Add(long.Parse(op));
				equations.Add(new EQ(long.Parse(s[0]), values));
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var eq in equations) {
				if (eq.Solve()) result += eq.total;
			}
			return result;
		}


		protected override long SolvePart2() {
			long result = 0;
			foreach (var eq in equations) {
				if (eq.SolvePart2()) result += eq.total;
			}
			return result;
		}
	}
}
