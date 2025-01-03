using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day18 {
	//slow and will not finish on the full dataset. on short examples can be used to cross check
	public class Day18Bad : Solver {
		public Day18Bad() : base(2019, 18) {
		}

		Dictionary<char, Vec> doors;
		Dictionary<char, Vec> keys;
		Vec player;
		char[,] input;
		int width, height;
		//List<char> passThrough;
		List<char> keysPickedUp;
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

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		record SaveState(Vec playerPos, long Steps, List<char> keys, (char, Vec) nextStep, long[,] costs);
		Stack<SaveState> branches;
		long steps = 0;
		long result = 0;
		protected override long SolvePart1() {
			//if (!IsShort) return -1;
			result = long.MaxValue;
			//1. find keys we can pickup
			//2. if we can pick multiple keys up, branch & try all keys?
			branches = new Stack<SaveState>();
			keysPickedUp = new List<char>();
			keyCosts = new();
			costsMemos = new();
			Stopwatch sw = Stopwatch.StartNew();
			var d = new Dijkstra18(input, keys);
			var costs = d.Map(player, keysPickedUp);
			var possibleSteps = keys.Where(x => costs[x.Value.x, x.Value.y] != long.MaxValue).ToList();
			foreach (var nextStep in possibleSteps) TryStack(nextStep, steps, costs);
			SaveState nextBranch;
			while (branches.TryPop(out nextBranch)) {
				costs = nextBranch.costs;
				steps = nextBranch.Steps;
				player = nextBranch.playerPos;
				keysPickedUp = nextBranch.keys;
				var nextPos = nextBranch.nextStep.Item2;
				steps += costs[nextPos.x, nextPos.y];
				if (steps > result) {
					continue;
				}
				totalIterations++;
				//Console.ForegroundColor = ConsoleColor.White;
				//Console.WriteLine($"\nPicking up key {nextBranch.nextStep.Item1} at {nextPos} costing {costs[nextPos.x, nextPos.y]} steps");
				//Console.WriteLine($"Total steps: {steps}");
				player = nextPos;
				var key = input[nextPos.x, nextPos.y];
				keysPickedUp.Add(key);
				if (!keys.Keys.Except(keysPickedUp).Any()) {
					//Console.WriteLine($"{steps}: {string.Join(',', keysPickedUp)}");
					if (steps < result) {
						Console.WriteLine($"Found better result {steps} in {sw.ElapsedMilliseconds} ms");
						sw.Restart();
						result = steps;
					}
					continue;
				}
				costs = d.Map(player, keysPickedUp);
				possibleSteps = keys.Where(x => !keysPickedUp.Contains(x.Key) && costs[x.Value.x, x.Value.y] != long.MaxValue).ToList();
				foreach (var nextStep in possibleSteps) TryStack(nextStep, steps, costs);
			}
			Console.WriteLine($"Finished in {totalIterations} iterations");
			return result;
		}
		long totalIterations = 0;



		Dictionary<string, long> keyCosts;
		//this pruning is incorrect, we are much faster but the best solutions are skipped : (
		private void TryStack(KeyValuePair<char, Vec> nextStep, long steps, long[,] costs) { //does not work : (
			//if (steps > result || steps > costs[nextStep.Value.x, nextStep.Value.y]) return;
			//var key = string.Join(',', keysPickedUp.Append(nextStep.Key).Order());
			//long prevCost;
			//if (!keyCosts.TryGetValue(key, out prevCost)) {
			//	keyCosts.Add(key, steps);
			//	prevCost = long.MaxValue;
			//}
			//if (prevCost > steps) {
			//	keyCosts[key] = steps;
			branches.Push(new SaveState(player, steps, new List<char>(keysPickedUp), (nextStep.Key, nextStep.Value), (long[,])costs.Clone()));
			//} else {
				//Console.WriteLine($"Pruning {key}, we have better solution of {prevCost} instead of {steps}");
			//}
		}

		private void Debug(long[,] costs) {
			Console.Clear();
			bool zebra = false;
			for (long y = 0; y < height; y++) {
				for (long x = 0; x < width; x++) {
					var cost = costs[x, y];
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
			Console.WriteLine();
			for (long y = 0; y < height; y++) {
				for (long x = 0; x < width; x++) {
					var cost = costs[x, y];
					if (cost > 99) cost = 99;
					var c = input[x, y];
					if (keysPickedUp.Contains(char.ToLower(input[x, y]))) Console.Write("..");
					else Console.Write($"{input[x, y]}{input[x, y]}");
				}
				Console.WriteLine();
			}
			//Console.WriteLine($"{")
		}

		//4548 not the solution

		ConcurrentDictionary<(Vec, List<char>), char[,]> costsMemos;

		public class Dijkstra18(char[,] input, Dictionary<char, Vec> allKeys) {
			ConcurrentDictionary<(Vec, List<char>), long[,]> costsMemos = new ConcurrentDictionary<(Vec, List<char>), long[,]>();
			int width, height;
			HashSet<Vec> dirty;
			List<char> passThrough;
			public long[,] Map(Vec start, List<char> keysPickedUp) {
				if (costsMemos.ContainsKey((start, keysPickedUp))) Console.WriteLine($"Memo found!"); //this memo does not work
				return costsMemos.GetOrAdd((start, keysPickedUp), cc => {
					passThrough = new List<char>(allKeys.Keys);
					passThrough.Add('.');
					passThrough.Add('@');
					passThrough.AddRange(keysPickedUp.Select(x => char.ToUpper(x)));

					width = input.GetLength(0);
					height = input.GetLength(1);
					dirty = new();
					var costs = new long[width, height];
					for (int x = 0; x < width; x++) {
						for (int y = 0; y < height; y++) {
							costs[x, y] = long.MaxValue;
						}
					}
					dirty.Add(start);
					costs[start.x, start.y] = 0;
					while (dirty.Any()) {
						var next = dirty.First();
						dirty.Remove(next);
						var cost = costs[next.x, next.y] + 1;
						Process(next.x - 1, next.y, cost, costs);
						Process(next.x + 1, next.y, cost, costs);
						Process(next.x, next.y - 1, cost, costs);
						Process(next.x, next.y + 1, cost, costs);
					}
					//Debug();
					return costs;
				});
			}

			private void Process(int x, int y, long cost, long[,] costs) {
				if (x < 0 || y < 0 || x >= width || y >= height) return;
				if (!passThrough.Contains(input[x, y])) return;
				if (costs[x, y] < cost) return;
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
