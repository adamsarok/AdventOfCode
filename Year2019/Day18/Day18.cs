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
using static System.Net.Mime.MediaTypeNames;
using static Year2019.Day18.Day18.Dijkstra18;

namespace Year2019.Day18 {
	//slow and will not finish on the full dataset. on short examples can be used to cross check
	public class Day18 : Solver {
		public Day18() : base(2019, 18) {
		}

		Dictionary<char, Vec> doors;
		Dictionary<char, Vec> keys;
		long allKeys;
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
			allKeys = (1 << keys.Count) - 1;
		}

		record KeyDistance(char key1, long key1bits, char key2, long key2bits, long neededKeys, long distance);
		Dictionary<char, List<KeyDistance>> keyDistances;

		Stopwatch sw;
		protected override long SolvePart1() {
			return -1;
			keyDistances = new();
			minimumCosts = new();
			sw = Stopwatch.StartNew();
			var d = new Dijkstra18(input);
			foreach (var key in keys) {
				var costs = d.Map(key.Value);
				keyDistances.Add(key.Key, new List<KeyDistance>());
				foreach (var other in keys.Where(x => x.Key != key.Key)) {
					var cost = costs[other.Value.x, other.Value.y];
					keyDistances[key.Key].Add(new KeyDistance(key.Key, GetBitMask(key.Key), other.Key, GetBitMask(other.Key), cost.neededKeys, cost.cost));
				}
			}
			var plCosts = d.Map(player);
			keyDistances.Add('@', new List<KeyDistance>());
			foreach (var key in keys) {
				var cost = plCosts[key.Value.x, key.Value.y];
				keyDistances['@'].Add(new KeyDistance('@', 0, key.Key, GetBitMask(key.Key), cost.neededKeys, cost.cost));
			}
			minCost = long.MaxValue;
			var possibleSteps = keyDistances['@'].Where(x => x.neededKeys == 0).ToList();
			foreach (var st in possibleSteps) {
				Traverse(st.distance, 0, st.key2);
			}
			Console.WriteLine($"Finished in {totalIterations} iterations");
			return minCost;
		}

		//Found current best 3764 in 2850353 ms
		//this runs for 2 hours :(

		Dictionary<(Vec, long), long> minimumCosts;

		long minCost;

		long GetBitMask(char c) => (1 << c - 'a');
		long GetBitMask(List<char> l) {
			long r = 0;
			foreach (var c in l) r |= GetBitMask(c);
			return r;
		}

		private void Traverse(long steps, long keysFound, char keyTo) {
			if (steps > minCost) return;
			long newKeys = keysFound;
			newKeys |= (1u << keyTo - 'a');
			long prevCost;
			var posKey = (keys[keyTo], newKeys);
			if (!minimumCosts.TryGetValue(posKey, out prevCost)) {
				minimumCosts.Add(posKey, steps);
			} else if (prevCost < steps) {
				return;
			}
			if ((allKeys & ~newKeys) == 0) {
				if (steps < minCost) {
					minCost = steps;
					Console.WriteLine($"Found current best {steps} in {sw.ElapsedMilliseconds} ms");
				}
				return;
			}
			var possibleSteps = keyDistances[keyTo].Where(x =>
				(x.key2bits & ~newKeys) != 0          //is a key we don't have yet
				&& ((x.neededKeys & ~newKeys) == 0)); //we have all prerequisite keys

			foreach (var st in possibleSteps.OrderBy(x => x.distance)) {
				Traverse(steps + st.distance, newKeys, st.key2);
			}
			totalIterations++;
		}

		long totalIterations = 0;

		public class Dijkstra18(char[,] input) {
			int width, height;
			HashSet<Vec> dirty;
			public record Cost(long cost, long neededKeys);
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
					long neededKeys = prevcost.neededKeys;
					var c = input[next.x, next.y];
					if (char.IsAsciiLetterUpper(c)) {
						neededKeys |= (1u << char.ToLower(c) - 'a');
					}
					var cost = new Cost(prevcost.cost + 1, neededKeys);
					Process(next.x - 1, next.y, cost, costs);
					Process(next.x + 1, next.y, cost, costs);
					Process(next.x, next.y - 1, cost, costs);
					Process(next.x, next.y + 1, cost, costs);
				}
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
				costs[x, y] = cost;
				dirty.Add(new Vec(x, y));
			}
		}

		Dictionary<char, Vec> players;
		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
			//@#@
			//###
			//@#@
			input[player.x - 1, player.y - 1] = '1';
			input[player.x, player.y - 1] = '#';
			input[player.x + 1, player.y - 1] = '2';

			input[player.x - 1, player.y] = '#';
			input[player.x, player.y] = '#';
			input[player.x + 1, player.y] = '#';

			input[player.x - 1, player.y + 1] = '3';
			input[player.x, player.y + 1] = '#';
			input[player.x + 1, player.y + 1] = '4';
			players = new Dictionary<char, Vec>();
			players.Add('1', new Vec(player.x - 1, player.y - 1));
			players.Add('2', new Vec(player.x + 1, player.y - 1));
			players.Add('3', new Vec(player.x - 1, player.y + 1));
			players.Add('4', new Vec(player.x + 1, player.y + 1));
			//Debug();
		}

		private void Debug() {
			Console.Clear();
			for (long y = 0; y < height; y++) {
				for (long x = 0; x < width; x++) {
					Console.Write(input[x, y]);
				}
				Console.WriteLine();
			}
			Console.ForegroundColor = ConsoleColor.White;
		}

		private void TraversePart2(long steps, long keysFound, char pl1pos, char pl2pos, char pl3pos, char pl4pos, char keyFrom, char keyTo) {
			long newKeys = keysFound;
			newKeys |= (1u << keyTo - 'a');
			long prevCost;
			var posKey = (keys[keyTo], newKeys);
			if (!minimumCosts.TryGetValue(posKey, out prevCost)) {
				minimumCosts.Add(posKey, steps);
			} else if (prevCost < steps) {
				return;
			}
			if ((allKeys & ~newKeys) == 0) {
				minCost = steps;
				Console.WriteLine($"Found current best {steps} in {sw.ElapsedMilliseconds} ms");
				return;
			}
			var maxCost = minCost - steps;
			var possibleSteps = keyDistances[pl1pos].Where(x =>
				(x.key2bits & ~newKeys) != 0
				&& ((x.neededKeys & ~newKeys) == 0)
				&& x.distance < maxCost);
			foreach (var st in possibleSteps.OrderBy(x => x.distance)) {
				TraversePart2(steps + st.distance, newKeys, st.key2, pl2pos, pl3pos, pl4pos, pl1pos, st.key2);
			}
			possibleSteps = keyDistances[pl2pos].Where(x =>
				(x.key2bits & ~newKeys) != 0
				&& ((x.neededKeys & ~newKeys) == 0)
				&& x.distance < maxCost);
			foreach (var st in possibleSteps.OrderBy(x => x.distance)) {
				TraversePart2(steps + st.distance, newKeys, pl1pos, st.key2, pl3pos, pl4pos, pl1pos, st.key2);
			}
			possibleSteps = keyDistances[pl3pos].Where(x =>
				(x.key2bits & ~newKeys) != 0
				&& ((x.neededKeys & ~newKeys) == 0)
				&& x.distance < maxCost);
			foreach (var st in possibleSteps.OrderBy(x => x.distance)) {
				TraversePart2(steps + st.distance, newKeys, pl1pos, pl2pos, st.key2, pl4pos, pl1pos, st.key2);
			}
			possibleSteps = keyDistances[pl4pos].Where(x =>
				(x.key2bits & ~newKeys) != 0
				&& ((x.neededKeys & ~newKeys) == 0)
				&& x.distance < maxCost);
			foreach (var st in possibleSteps.OrderBy(x => x.distance)) {
				TraversePart2(steps + st.distance, newKeys, pl1pos, pl2pos, pl3pos, st.key2, pl1pos, st.key2);
			}
			totalIterations++;
		}

		protected override long SolvePart2() {
			keyDistances = new();
			minimumCosts = new();
			sw = Stopwatch.StartNew();
			var d = new Dijkstra18(input);
			var allObj = keys.Union(players);
			foreach (var key in allObj) {
				var costs = d.Map(key.Value);
				keyDistances.Add(key.Key, new List<KeyDistance>());
				foreach (var other in allObj.Where(x => x.Key != key.Key && !char.IsNumber(x.Key))) {
					var cost = costs[other.Value.x, other.Value.y];
					if (cost.cost != long.MaxValue) {
						keyDistances[key.Key].Add(new KeyDistance(key.Key, GetBitMask(key.Key), other.Key, GetBitMask(other.Key), cost.neededKeys, cost.cost));
					}
				}
			}
			minCost = long.MaxValue;
			var possibleSteps = keyDistances['1'].Where(x => x.neededKeys == 0).ToList();
			foreach (var st in possibleSteps) {
				TraversePart2(st.distance, 0, st.key2, '2', '3', '4', '1', st.key2);
			}
			possibleSteps = keyDistances['2'].Where(x => x.neededKeys == 0).ToList();
			foreach (var st in possibleSteps) {
				TraversePart2(st.distance, 0, '1', st.key2, '3', '4', '2', st.key2);
			}
			possibleSteps = keyDistances['3'].Where(x => x.neededKeys == 0).ToList();
			foreach (var st in possibleSteps) {
				TraversePart2(st.distance, 0, '1', '2', st.key2, '4', '3', st.key2);
			}
			possibleSteps = keyDistances['4'].Where(x => x.neededKeys == 0).ToList();
			foreach (var st in possibleSteps) {
				TraversePart2(st.distance, 0, '1', '2', '3', st.key2, '4', st.key2);
			}
			Console.WriteLine($"Finished in {totalIterations} iterations");
			return minCost;
		}
	}
}
