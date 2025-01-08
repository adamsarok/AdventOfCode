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
						var r = f[y][x + 1];
						string key;
						Vec portalPos;
						bool isOuter = x <= 1 || y <= 1 || x >= width - 2 || y >= height - 2;
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
						TryAddPortal(key, portalPos, isOuter);
					}
				}
			}
		}

		private void TryAddPortal(string key, Vec portalPos, bool isOuter) {
			if (key.Contains(".")) throw new Exception();
			procd.Add(portalPos);
			if (key == "AA") {
				start = portalPos;
			} else if (key == "ZZ") {
				end = portalPos;
			} else {
				Portal p;
				if (portals.TryGetValue(key, out p)) {
					portals[key] = isOuter ? new Portal(key, p.inner, portalPos) : new Portal(key, portalPos, p.outer);
				} else {
					portals.Add(key, isOuter ? new Portal(key, null, portalPos) : new Portal(key, portalPos, null));
				}
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			var d = new Dijkstra(input, portals.Values.ToList(), start, end, false);
			return d.GetCost();
		}
		Dictionary<string, Portal> portals;
		public record Portal(string code, Vec? inner, Vec? outer);
		public class Dijkstra(char[,] input, List<Portal> portals, Vec start, Vec end, bool hasDepths) {
			private List<long[,]> costsPerDepth;
			int width, height;
			HashSet<(Vec, int)> dirty;
			long maxCost;
			public enum CostType { StartFinish, LongestPath }
			public long GetCost() {
				width = input.GetLength(0);
				height = input.GetLength(1);
				dirty = new();
				costsPerDepth = new();
				for (int i = 0; i < portals.Count; i++) {
					var c = new long[width, height];
					for (int x = 0; x < width; x++) {
						for (int y = 0; y < height; y++) {
							c[x, y] = long.MaxValue;
						}
					}
					costsPerDepth.Add(c);
				}
				dirty.Add((start, 0));
				costsPerDepth[0][start.x, start.y] = 0;
				while (dirty.Any()) {
					var next = dirty.First();
					dirty.Remove(next);
					var cost = costsPerDepth[next.Item2][next.Item1.x, next.Item1.y] + 1;
					Process(next.Item1.x - 1, next.Item1.y, cost, next.Item2);
					Process(next.Item1.x + 1, next.Item1.y, cost, next.Item2);
					Process(next.Item1.x, next.Item1.y - 1, cost, next.Item2);
					Process(next.Item1.x, next.Item1.y + 1, cost, next.Item2);
				}
				//Debug();
				return costsPerDepth[0][end.x, end.y];
			}
			private void Debug() {
				Console.Clear();
				for (long y = 0; y < height; y++) {
					for (long x = 0; x < width; x++) {
						var cost = costsPerDepth[0][x, y];
						if (cost == long.MaxValue) Console.ResetColor();
						else Console.ForegroundColor = ConsoleColor.Green;
						if (cost > 999) cost = 999;
						Console.Write($"{cost.ToString("000")}");
					}
					Console.WriteLine();
				}
			}
			private void Process(int x, int y, long cost, int depth) {
				var pos = new Vec(x, y);
				var p = portals.FirstOrDefault(x => x.inner == pos || x.outer == pos);
				if (p != null) {
					Vec to = (p.inner == pos ? p.outer : p.inner);
					if (hasDepths && p.inner == pos) depth++;
					else if (hasDepths) depth--;
					if (depth < 0 || depth >= portals.Count) return;
					if (cost + 1 < costsPerDepth[depth][to.x, to.y]) {
						costsPerDepth[depth][to.x, to.y] = cost + 1;
						dirty.Add((to, depth));
					}
				} else {
					if (x < 0 || y < 0 || x >= width || y >= height) return;
					if (input[x, y] != '.') return;
					if (costsPerDepth[depth][x, y] < cost) return;
					if (pos == end) {
						if (depth != 0) {
							return;
						} else {
							bool what = true;
						}
					}
					costsPerDepth[depth][x, y] = cost;
					dirty.Add((new Vec(x, y), depth));
				}
			}
		}

		protected override long SolvePart2() {
			long result = 0;
			var d = new Dijkstra(input, portals.Values.ToList(), start, end, true);
			return d.GetCost();
		}
	}
}
