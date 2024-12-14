using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day1 {
	public class Day1 : Solver {
		List<int> list1 = new();
		List<int> list2 = new();
		Dictionary<int, int> list2counts = new Dictionary<int, int>();

		public Day1() : base(2024, 1) {
		}

		protected override void ReadInputPart1(string fileName) {
			list1 = new();
			list2 = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				var s = l.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
				list1.Add(int.Parse(s[0]));
				list2.Add(int.Parse(s[1]));
			}
			list1.Sort();
			list2.Sort();
		}

		protected override void ReadInputPart2(string fileName) {
			list1 = new();
			list2counts = new Dictionary<int, int>();
			foreach (var l in File.ReadAllLines(fileName)) {
				var s = l.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
				list1.Add(int.Parse(s[0]));
				var n2 = int.Parse(s[1]);
				if (list2counts.ContainsKey(n2)) list2counts[n2]++;
				else list2counts.Add(n2, 1);
			}
		}

		protected override long SolvePart1() {
			long result = 0;
			for (int i = 0; i < list1.Count; i++) {
				var dist = Math.Abs(list1[i] - list2[i]);
				result += dist;
			}
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;
			for (int i = 0; i < list1.Count; i++) {
				var n1 = list1[i];
				int n2;
				list2counts.TryGetValue(n1, out n2);
				result += n1 * n2;
			}
			return result;
		}
	}
}
