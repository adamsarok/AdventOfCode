using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day5 {
	public class Day5 : Solver {
		public Day5() : base(2024, 5) {
		}
		List<(int, int)> rules = new();
		List<List<int>> updates = new();
		protected override void ReadInputPart1(string fileName) {
			rules = new();
			updates = new();
			bool upds = false;
			foreach (var l in File.ReadAllLines(fileName)) {
				if (string.IsNullOrWhiteSpace(l)) upds = true;
				else if (upds) {
					List<int> r = new List<int>();
					var s = l.Split(',');
					foreach (var c in s) r.Add(int.Parse(c));
					updates.Add(r);
				} else {
					var s = l.Split('|');
					rules.Add((int.Parse(s[0]), int.Parse(s[1])));
				}
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		private List<int> GetBefores(int num) {
			List<int> result = new List<int>();
			foreach (var r in rules.Where(x => x.Item2 == num)) result.Add(r.Item1);
			return result;
		}
		protected override long SolvePart1() {
			long result = 0;
			foreach (var u in updates) {
				var bad = CheckBad(u);
				if (!bad.Bad) result += u[u.Count / 2];
			}
			return result;
		}
		record Result(bool Bad, int Index1, int Index2);
		private Result CheckBad(List<int> u) {
			for (int i = 0; i < u.Count; i++) {
				var num = u[i];
				var befores = GetBefores(num);
				for (int b = i; b < u.Count; b++) {
					var after = u[b];
					if (befores.Contains(after)) {
						return new Result(true, i, b);
					}
				}
			}
			return new Result(false, -1, -1); ;
		}

		protected override long SolvePart2() {
			long result = 0;
			foreach (var u in updates) {
				Result r = CheckBad(u);
				if (!r.Bad) continue;
				while (r.Bad) {
					int t = u[r.Index1];
					u[r.Index1] = u[r.Index2];
					u[r.Index2] = t;
					r = CheckBad(u);
				}
				result += u[u.Count / 2];
			}
			return result;
		}
	}
}
