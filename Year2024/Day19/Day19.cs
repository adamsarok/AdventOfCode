using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
				Stopwatch sw = Stopwatch.StartNew();
				if (Match(design, out acc)) {
					Console.WriteLine($"Found {acc} ways in {sw.ElapsedMilliseconds} ms: {design}");
					result++;
				} else {
					Console.WriteLine($"Failed in {sw.ElapsedMilliseconds} ms: {design}");
				}
			}
			return result;
		}
		private void Debug(Dictionary<string, int> cache, string act) {
			Console.Clear();
			Console.WriteLine($"Act: {act}");
			foreach (var kvp in cache) {
				Console.WriteLine($"{kvp.Key}: {kvp.Value}");
			} 
		}
		//TODO: slowwwwwwwwwwww and acc does not show correct amount
		private bool Match(string design, out int acc) {
			Dictionary<string, int> cache = towels.ToDictionary(k => k, v => 1);
			acc = 0;
			while (true) {
				if (cache.TryGetValue(design, out acc)) {
					return true;
				}
				var keys = cache.Keys.ToList();
				bool added = false;
				foreach (var key in keys) {
					foreach (var towel in towels) {
						var add = key + towel;
						if (design.StartsWith(add)) {
							if (!cache.ContainsKey(add)) {
								cache.Add(add, cache[key]);
								added = true;
							} else {
								cache[add]++;
							}
							//Debug(cache, add);
						}
					}
				}
				if (!added) return false;
			}
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
