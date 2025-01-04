using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day18 {
	//slow and will not finish on the full dataset. on short examples can be used to cross check
	public class Day18 : Solver {
		public Day18() : base(2019, 18) {
		}

		Dictionary<char, Vec> doors;
		Dictionary<char, Vec> keys;
		Vec player;
		char[,] input;
		int width, height;
		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			var f = File.ReadAllLines(fileName);
			width = f[0].Length;
			height = f.Length;
			doors = new Dictionary<char, Vec>();
			keys = new Dictionary<char, Vec>();
			input = new char[width, height];
			for (int y = 0; y < height; y++) {
				var l = f[y];
				for (int x = 0; x < width; x++) {
					var c = l[x];
					input[x, y] = c;
					if (c == '@') player = new Vec(x, y);
					else if (char.IsAsciiLetterUpper(c)) doors.Add(c, new Vec(x, y));
					else if (char.IsAsciiLetterLower(c)) keys.Add(c, new Vec(x, y));
				}
			}
		}

		record KeyDistance(char key1, char key2, List<char> neededKeys, long distance);
		Dictionary<(char, char), KeyDistance> keyDistances;

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		long steps = 0;
		long result = 0;
		Stopwatch sw;
		protected override long SolvePart1() {
			//if (!IsShort) return -1;
			result = long.MaxValue;
			//2nd iteration - find all (distances, neededkeys) between key pairs
			//in the short examples this is trivial, however in the full example, what happens if we have two paths
			//path1 shorter but needing additional keys, path2 longer? build a graph, where path1 has a prerequisite length to the needed keys? 
			keyDistances = new();
			sw = Stopwatch.StartNew();
			var d = new Dijkstra18(input);
			foreach (var key in keys) { //key -> key
				var costs = d.Map(key.Value);
				foreach (var other in keys.Where(x => x.Key != key.Key)) {
					var cost = costs[other.Value.x, other.Value.y];
					keyDistances.Add((key.Key, other.Key), new KeyDistance(key.Key, other.Key, cost.neededKeys, cost.cost)); 
				}
			} //player -> key
			var plCosts = d.Map(player);
			foreach (var key in keys) {
				var cost = plCosts[key.Value.x, key.Value.y];
				keyDistances.Add(('@', key.Key), new KeyDistance('@', key.Key, cost.neededKeys, cost.cost));
			}

			//foreach (var permutation in keys.Keys.Permute()) {
			//	var dist = CheckKeyPermutation(permutation);
			//	if (dist > 0 && dist < result) {
			//		Console.WriteLine($"Found current best {dist} in {sw.ElapsedMilliseconds} ms");
			//		result = dist;
			//	}
			//}
			minCost = long.MaxValue;
			var possibleSteps = keyDistances.Where(x => x.Key.Item1 == '@' && x.Value.neededKeys.Count == 0).ToList();
			foreach (var st in possibleSteps) {
				Traverse(st.Value.distance, new List<char>(), '@', st.Key.Item2);
			}

			Console.WriteLine($"Finished in {totalIterations} iterations");
			return minCost;
		}

		//4548 not the solution
		//Found current best 4118 in 16115 ms - this is also not the right answer. something is still way too slow
		//Found current best 4090 in 179302 ms
		//Much better still not complete: Found current best 3830 in 81 ms

		//still not good enough - maybe a single dijkstra where the key is not just position but collected keys?

		long minCost;
		private void Traverse(long steps, List<char> keysFound, char keyFrom, char keyTo) {
			if (steps > minCost) return;
			var newKeys = new List<char>(keysFound);
			newKeys.Add(keyTo);
			if (newKeys.Count == keys.Count) {
				if (steps < minCost) {
					minCost = steps;
					Console.WriteLine($"Found current best {steps} in {sw.ElapsedMilliseconds} ms");
				}
				return;
			}
			var possibleSteps = keyDistances.Where(x => x.Key.Item1 == keyTo
				&& !keysFound.Contains(x.Key.Item2)
				&& !x.Value.neededKeys.Except(newKeys).Any()).ToList();
			foreach (var st in possibleSteps.OrderBy(x => x.Value.distance)) {
				Traverse(steps + st.Value.distance, newKeys, keyTo, st.Key.Item2);
			}
			totalIterations++;
		}

		//we can rule out permutations where we don't have the necessary keys
		//however this is still painfully slow to iterate on all permutations, we need something smarter.
		private long CheckKeyPermutation(IEnumerable<char> permutation) {
			var p = permutation.Prepend('@').ToList();
			List<char> haveKeys = new List<char>();
			long dist = 0;
			for (int i = 0; i < p.Count - 1; i++) {
				totalIterations++;
				var key = p[i];
				var keyDist = keyDistances[(key, p[i + 1])];               
				if (keyDist.neededKeys.Except(haveKeys).Where(x => x != key).Any()) { 
					//Console.WriteLine($"skipping impossible combination {string.Join(',', p)}");
					return -1;
				}
				haveKeys.Add(key);
				//Console.WriteLine($"{key}->{p[i+1]}={keyDist.distance}:");
				dist += keyDist.distance;
			}
			//Console.WriteLine($"{dist}: Valid combination {string.Join(',', p)}");
			return dist;
		}

		long totalIterations = 0;
			

		public class Dijkstra18(char[,] input) {
			int width, height;
			HashSet<Vec> dirty;
			public record Cost(long cost, List<char> neededKeys);
			public Cost[,] Map(Vec start) {


				width = input.GetLength(0);
				height = input.GetLength(1);
				dirty = new();
				var costs = new Cost[width, height];
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						costs[x, y] = new Cost(long.MaxValue, new());
					}
				}
				dirty.Add(start);
				costs[start.x, start.y] = new Cost(0, new());
				while (dirty.Any()) {
					var next = dirty.First();
					dirty.Remove(next);
					var prevcost = costs[next.x, next.y];
					var neededKeys = new List<char>(prevcost.neededKeys);
					if (char.IsAsciiLetterUpper(input[next.x, next.y])) {
						neededKeys.Add(char.ToLower(input[next.x, next.y]));
					}
					var cost = new Cost(prevcost.cost + 1, neededKeys);
					Process(next.x - 1, next.y, cost, costs);
					Process(next.x + 1, next.y, cost, costs);
					Process(next.x, next.y - 1, cost, costs);
					Process(next.x, next.y + 1, cost, costs);
				}
				//Debug(costs);
				return costs;
			}

			private void Debug(Cost[,] costs) {
				Console.Clear();
				bool zebra = false;
				for (long y = 0; y < height; y++) {
					for (long x = 0; x < width; x++) {
						var cost = costs[x, y].cost;
						if (cost > 99) cost = 99;
						Console.ForegroundColor = ConsoleColor.Green;
						if (input[x, y] == '#') Console.Write("##");
						else {
							if (cost == long.MaxValue) Console.ResetColor();
							else {
								Console.ForegroundColor = zebra ? ConsoleColor.Yellow : ConsoleColor.Blue;
								zebra = !zebra;
							}
							Console.Write($"{cost.ToString("00")}");
						}
					}
					Console.WriteLine();
				}
				Console.ForegroundColor = ConsoleColor.White;
			}


			private void Process(int x, int y, Cost cost, Cost[,] costs) {
				if (x < 0 || y < 0 || x >= width || y >= height) return;
				var c = input[x, y];
				if (c == '#') return;

				var old = costs[x, y];
				if (old.cost < cost.cost) return;

				if (old.cost < long.MaxValue && c < cost.neededKeys.Count) {
					bool TODO = true;
				}
				//what happens if we have two paths
				//path1 shorter but needing additional keys, path2 longer? store the shortest per all possible key combinations?

				costs[x, y] = cost;
				//maxCost = Math.Max(maxCost, cost);
				dirty.Add(new Vec(x, y));
			}
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
