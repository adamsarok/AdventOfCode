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

namespace Year2024.Day11 {
	public class Day11 : Solver {
		public Day11() : base(2024, 11) {
		}
		LinkedList<long> input;

		protected override void ReadInputPart1(string fileName) {
			input = new LinkedList<long>();
			foreach (var item in File.ReadAllLines(fileName)[0].Split(" ")) {
				input.AddLast(long.Parse(item));
			}
		}

		protected override long SolvePart1() {
			for (int i = 0; i < 25; i++) Blink();
			return input.Count;
		}

		private void Blink() {
			var currentNode = input.First;
			while (currentNode != null) {
				var stone = currentNode.Value;
				if (stone == 0) {
					currentNode.Value = 1;
				} else {
					var stoneStr = stone.ToString();
					var l = stoneStr.Length;
					if (l % 2 == 0) {
						currentNode.Value = long.Parse(stoneStr.Substring(0, l / 2));
						input.AddAfter(currentNode, long.Parse(stoneStr.Substring(l / 2, l / 2)));
						currentNode = currentNode.Next;
					} else {
						currentNode.Value *= 2024;
					}
				}
				if (currentNode != null) currentNode = currentNode.Next;
			}
		}

		Dictionary<long, long> stonesCounts = new Dictionary<long, long>();
		private void TryAdd(Dictionary<long, long> dict, long num, long count) {
			if (dict.ContainsKey(num)) {
				dict[num] += count;
			} else {
				dict.Add(num, count);
			}
		}
		
		private void BlinkPart2() {
			//the order of the stones does NOT matter! we don't need to use a linked list!
			//however we have to build a new dictionary on each blink,
			//otherwise when 2024 is created from blink 1, we are already processing it in that same blink
			var newDict = new Dictionary<long, long>();
			foreach (var stone in stonesCounts.Where(x => x.Value > 0)) {
				if (stone.Key == 0) {
					TryAdd(newDict, 1, stone.Value);
				} else {
					var stoneStr = stone.Key.ToString();
					var l = stoneStr.Length;
					if (l % 2 == 0) {
						TryAdd(newDict, long.Parse(stoneStr.Substring(0, l / 2)), stone.Value);
						TryAdd(newDict, long.Parse(stoneStr.Substring(l / 2, l / 2)), stone.Value);
					} else {
						TryAdd(newDict, stone.Key * 2024, stone.Value);
					}
				}
			}
			stonesCounts = newDict;
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart2() {
			stonesCounts = new Dictionary<long, long>();
			foreach (var item in input) {
				TryAdd(stonesCounts, item, 1);
			}
			for (int i = 0; i < 75; i++) BlinkPart2();
			return stonesCounts.Values.Sum();
		}
	}
}
		