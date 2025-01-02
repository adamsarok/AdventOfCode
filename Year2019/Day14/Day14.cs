using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day14 {
	public class Day14 : Solver {
		public Day14() : base(2019, 14) {
		}

		record BOM(string name, long quantity, List<(string, long)> materials);
		Dictionary<string, BOM> input;
		Dictionary<string, long> accumulator;
		Dictionary<string, long> remainder;
		long oreNeeded = 0;
		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			input = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				var materials = new List<(string, long)>();
				foreach (var mat in l.Split("=>")[0].Split(",")) {
					materials.Add((mat.Trim().Split(" ")[1], long.Parse(mat.Trim().Split(" ")[0])));
				}
				var key = l.Split("=>")[1].Trim();
				input.Add(key.Split(" ")[1], new BOM(key.Split(" ")[1], long.Parse(key.Split(" ")[0]), materials));
			}
		}


		

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			accumulator = new();
			remainder = new();
			accumulator.Add("FUEL", 1);
			while (accumulator.Count > 0 && !accumulator.All(x => x.Key == "ORE")) {
				Fill();
			}
			return accumulator["ORE"];
		}

		private void Fill() {
			var temp = accumulator.ToArray();
			accumulator.Clear();
			foreach (var t in temp) {
				if (t.Key == "ORE") {
					AddOrInrement(t.Key, t.Value);
				} else {
					var bom = input[t.Key];
					long needed = (long)Math.Round(((double)(t.Value - GetRemanider(t.Key)) / (double)bom.quantity), MidpointRounding.ToPositiveInfinity);
					var remainder = (needed * bom.quantity) - t.Value;
					AddRemainder(t.Key, remainder); 
					foreach (var mat in bom.materials) {
						//Console.WriteLine($"To make {t.Value} of {bom.name} we need {mat.Item2 * needed} {mat.Item1}");
						AddOrInrement(mat.Item1, mat.Item2 * needed);
					}
				}
			}
		}
		private long GetRemanider(string key) {
			long value;
			if (remainder.TryGetValue(key, out value)) return value;
			return 0;
		}
		
		private void AddRemainder(string key, long value) {
			if (remainder.ContainsKey(key)) remainder[key] += value;
			else remainder.Add(key, value);
		}
		private void AddOrInrement(string key, long value) {
			if (accumulator.ContainsKey(key)) accumulator[key] += value;
			else accumulator.Add(key, value);
		}

		//is it still brute force if it completes in 15 sec?
		//I could automate the range estimation, but for now this works for my test case
		protected override long SolvePart2() {
			for (long i = 1300000; i < 10000000; i++) {
				accumulator = new();
				remainder = new();
				accumulator.Add("FUEL", i);
				while (accumulator.Count > 0 && !accumulator.All(x => x.Key == "ORE")) {
					Fill();
				}
				if (accumulator["ORE"] > 1000000000000) return i - 1;
			}
			return -1;
		}
	}
}
