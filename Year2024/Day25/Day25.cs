using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day25 {
	public class Day25 : Solver {
		public Day25() : base(2024, 25) {
		}
		List<int[]> locks;
		List<int[]> keys;
		int height;
		protected override void ReadInputPart1(string fileName) {
			List<string> curr = new List<string>();
			locks = new();
			keys = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				if (string.IsNullOrEmpty(l)) {
					Add(curr);
					curr = new List<string>();
				} else curr.Add(l);
			}
			Add(curr);
		}

		private void Add(List<string> curr) {
			var result = new int[curr[0].Length];
			for (int y = 0; y < curr.Count; y++) {
				for (int x = 0; x < curr[0].Length; x++) {
					if (curr[y][x] == '#') result[x]++;
				}
			}
			height = curr.Count;
			if (curr[0][0] == '#') locks.Add(result);
			else keys.Add(result);
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var key in keys) {
				foreach (var locky in locks) {
					bool fit = CheckFit(locky, key);
					if (fit) result++;
					//Console.WriteLine($"{string.Join(',', locky)}->{string.Join(',', key)}={fit}");
				}
			}
			return result;
		}

		private bool CheckFit(int[] locky, int[] key) {
			for (int i = 0; i < locky.Length; i++) {
				if (key[i] + locky[i] > height) {
					return false;
				}
			}
			return true;
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
