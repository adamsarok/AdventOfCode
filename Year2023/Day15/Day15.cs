using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023.Day15 {
	public class Day15 : Solver {
		public Day15() : base(2023, 15) {
		}
		protected override void ReadInputPart1(string fileName) {
			//input = new();
			foreach (var l in File.ReadAllLines(fileName)) {

			}
		}

		protected override void ReadInputPart2(string fileName) {
			//input = new();
			foreach (var l in File.ReadAllLines(fileName)) {

			}
		}

		protected override long SolvePart1() {
			var inputs = File.ReadAllLines("testinput.txt")[0].Split(',');
			var result = 0;
			Debug.Assert(HASHMAP.Hash("HASH") == 52, "Hashing is incorrect");
			foreach (var input in inputs) {
				result += HASHMAP.Hash(input);
			}

			return result;
		}

		protected override long SolvePart2() {
			long result = 0;
			var inputs = File.ReadAllLines("testinput.txt")[0].Split(',');
			Debug.Assert(HASHMAP.Hash("HASH") == 52, "Hashing is incorrect");
			var h = new HASHMAP(256);
			foreach (var input in inputs) {
				if (input.Contains("=")) {
					var s = input.Split('=');
					var label = s[0];
					var focal = s[1];
					h.Add(label, int.Parse(focal));
				} else {
					var label = input.Split('-')[0];
					h.Remove(label);
				}
			}
			h.PrintResult();
			return result; //TODO what
		}

		private class Lens {
			public string Label { get; set; } = "";
			public int FocalLength { get; set; }
		}
		private class HASHMAP {
			public void PrintResult() {
				int result = 0;
				for (int i = 0; i < buckets.Length; i++) {
					var bucket = buckets[i];
					if (bucket != null) {
						Console.Write($"Box {i}:");
						for (int j = 0; j < bucket.Count; j++) {
							var lens = bucket[j];
							Console.Write($"[{lens.Label} {lens.FocalLength}]");
							result += (i + 1) * (j + 1) * bucket[j].FocalLength;
						}
						Console.WriteLine();
					}
				}
				Console.WriteLine(result);
			}
			private List<Lens>[] buckets;
			public HASHMAP(int size) {
				buckets = new List<Lens>[size];
			}
			public void Add(string label, int value) {
				int b = Hash(label);
				if (buckets[b] == null) {
					buckets[b] = new List<Lens>();
				}
				var bucket = buckets[b];
				var lens = bucket.FirstOrDefault(x => x.Label == label);
				if (lens != null) {
					lens.FocalLength = value;
				} else {
					bucket.Add(new Lens { Label = label, FocalLength = value });
				}
			}
			public void Remove(string label) {
				int b = Hash(label);
				if (buckets[b] == null) return;
				var bucket = buckets[b];
				for (int i = 0; i < bucket.Count; i++) {
					if (bucket[i].Label == label) {
						bucket.RemoveAt(i);
						return;
					}
				}
			}
			public static int Hash(string input) {
				var result = 0;
				foreach (var c in input) {
					var a = (int)c;
					result += c;
					result *= 17;
					result = result % 256;
				}
				return result;
			}
		}
	}
}
