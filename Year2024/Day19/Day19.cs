using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Year2024.Day19.Day19;

namespace Year2024.Day19 {
	public class Day19 : Solver {
		public Day19() : base(2024, 19) {
		}
		string[] input;
		private HashSet<string> towels;
		List<string> designs;

		protected override void ReadInputPart1(string fileName) {
			input = File.ReadAllLines(fileName);
			towels = new HashSet<string>();
			foreach (var t in input[0].Split(',')) {
				towels.Add(t.Trim());
			}
			designs = new List<string>();
			for (int i = 2; i < input.Length; i++) {
				designs.Add(input[i]);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		//naive: 7ms
		//with trie: fail
		//memoization?: works but oh so slow
		protected override long SolvePart1() {
			long result = 0;
			for (int i = 0; i < designs.Count; i++) {
				var design = designs[i];
				int acc;
				if (Match(design, out acc)) {
					Console.WriteLine($"Found {acc} ways: {design}");
					result++;
				} else {
					Console.WriteLine($"Failed: {design}");
				}
			}
			return result;
		}

		//TODO: slowwwwwwwwwwww and acc does not show correct amount
		private bool Match(string design, out int acc) {
			HashSet<string> cache = new HashSet<string>(towels);
			acc = 0;
			while (true) {
				if (cache.Contains(design)) {
					return true;
				}
				HashSet<string> news = new HashSet<string>();
				foreach (var cached in cache) {
					foreach (var towel in towels) {
						var add = cached + towel;
						if (!cache.Contains(add) && design.StartsWith(add)) news.Add(add);
					}
				}
				if (news.Count == 0) return false;
				acc += news.Count;
				cache.UnionWith(news);
			}
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
