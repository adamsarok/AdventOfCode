using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers.Dijkstra;

namespace Year2019.Day20 {
	public class Day20 : Solver {
		public Day20() : base(2019, 20) {
		}
		char[,] input;
		Vec start, end;
		int height, width;
		List<Vec> procd;
		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			portals = new();
			var f = File.ReadAllLines(fileName);
			width = f.Max(x => x.Length);
			height = f.Length;
			procd = new List<Vec>();
			input = new char[width, f.Length];
			for (int y = 0; y < f.Length; y++) {
				for (int x = 0; x < f[y].Length; x++) {
					var c = f[y][x];
					input[x, y] = c;
					if (!procd.Contains(new Vec(x,y)) && char.IsAsciiLetter(c) && c != '\0') {
						var r = f[y][x + 1]; //  input[x + 1, y];
						string key;
						Vec portalPos;
						bool isOuter = x <= 1 || y <= 1 || x >= width - 1 || y >= height - 1;
						if (char.IsLetter(r) && r != '\0') {
							portalPos = x + 2 < width && f[y][x + 2] == '.' ? new Vec(x + 2, y) : new Vec(x - 1, y);
							procd.Add(new Vec(x + 1, y));
							key = c.ToString() + r.ToString();
						} else {
							var d = f[y + 1][x];
							procd.Add(new Vec(x, y + 1));
							portalPos = y < height - 2 && f[y + 2][x] == '.' ? new Vec(x, y + 2) : new Vec(x, y - 1);
							key = c.ToString() + d.ToString();
						}
						TryAddPortal(key, portalPos);
					}
				}
			}
		}

		private void TryAddPortal(string key, Vec portalPos) {
			if (key.Contains(".")) throw new Exception();
			procd.Add(portalPos);
			if (key == "AA") {
				start = portalPos;
			} else if (key == "ZZ") {
				end = portalPos;
			} else {
				Portal p;
				if (portals.TryGetValue(key, out p)) {
					portals[key] = new Portal(key, p.from, portalPos);
				} else {
					portals.Add(key, new Portal(key, portalPos, null));
				}
			}
		}

		protected override void ReadInputPart2(string fileName) {
			base.ReadInputPart2(fileName);
			//input = new();
			foreach (var l in File.ReadAllLines(fileName)) {

			}
		}

		protected override long SolvePart1() {
			long result = 0;
			var d = new Dijkstra(input, portals.Values.ToList());
			return d.GetCost(start, end);
		}
		Dictionary<string, Portal> portals;
		public record Portal(string code, Vec? from, Vec? to);
		public class Dijkstra(char[,] input, List<Portal> portals) {
			private long[,] costs;
			int width, height;
			HashSet<Vec> dirty;
			long maxCost;
			public enum CostType { StartFinish, LongestPath }
			public long GetCost(Vec start, Vec end) {
				width = input.GetLength(0);
				height = input.GetLength(1);
				dirty = new();
				costs = new long[width, height];
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
					Process(next.x - 1, next.y, cost);
					Process(next.x + 1, next.y, cost);
					Process(next.x, next.y - 1, cost);
					Process(next.x, next.y + 1, cost);
				}
				//Debug();
				return costs[end.x, end.y];
			}
			private void Debug() {
				Console.Clear();
				for (long y = 0; y < height; y++) {
					for (long x = 0; x < width; x++) {
						var cost = costs[x, y];
						if (cost == long.MaxValue) Console.ResetColor();
						else Console.ForegroundColor = ConsoleColor.Green;
						if (cost > 999) cost = 999;
						Console.Write($"{cost.ToString("000")}");
					}
					Console.WriteLine();
				}
			}
			private void Process(int x, int y, long cost) {
				var pos = new Vec(x, y);
				var p = portals.FirstOrDefault(x => x.from == pos || x.to == pos);
				if (p != null) {
					Vec to = (p.from == pos ? p.to : p.from);
					if (cost + 1 < costs[to.x, to.y]) {
						costs[to.x, to.y] = cost + 1;
						dirty.Add(to);
					}
				}
				if (x < 0 || y < 0 || x >= width || y >= height) return;
				if (input[x, y] != '.') return;
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
