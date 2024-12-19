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

		protected override long SolvePart1() {
			long result = 0;
			for (int i = 0; i < designs.Count; i++) {
				var design = designs[i];
				long acc;
				Stopwatch sw = Stopwatch.StartNew();
				if (Match2(design, out acc)) {
					//Console.WriteLine($"Found {acc} ways in {sw.ElapsedMilliseconds} ms: {design}");
					result++;
				} else {
					//Console.WriteLine($"Failed in {sw.ElapsedMilliseconds} ms: {design}");
				}
			}
			return result;
		}
		protected override long SolvePart2() {

			long result = 0;
			for (int i = 0; i < designs.Count; i++) {
				var design = designs[i];
				long acc;
				Stopwatch sw = Stopwatch.StartNew();
				if (Match2(design, out acc)) {
					//Console.WriteLine($"Found {acc} ways in {sw.ElapsedMilliseconds} ms: {design}");
					result += acc;
				} else {
					//Console.WriteLine($"Failed in {sw.ElapsedMilliseconds} ms: {design}");
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
		
		private bool Match2(string design, out long acc) {
			Dictionary<string, long> memo = new Dictionary<string, long>();
			acc = CountWays(design, memo);
			return acc > 0;
		}

		private long CountWays(string design, Dictionary<string, long> memo) {
			//Console.WriteLine(design);
			if (string.IsNullOrEmpty(design)) {
				return 1;
			}
			if (memo.ContainsKey(design)) return memo[design];

			long totalWays = 0;
			foreach (var towel in towels) {
				if (design.StartsWith(towel)) {
					string remainingDesign = design.Substring(towel.Length);
					totalWays += CountWays(remainingDesign, memo);
				}
			}

			memo[design] = totalWays;
			return totalWays;
		}


	}
}
