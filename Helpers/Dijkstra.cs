using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Helpers.Solver;

namespace Helpers {
	public class Dijkstra {
		private long[,] costs;
		Tile[,] input;
		int width, height;
		HashSet<Vec> dirty;
		long maxCost;
		public enum Tile { Wall, Path, Start, Finish }
		public enum CostType { StartFinish, LongestPath }
		public long GetCost(Tile[,] input, CostType costType = CostType.StartFinish) {
			this.input = input;
			width = input.GetLength(0);
			height = input.GetLength(1);
			dirty = new();
			costs = new long[width, height];
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					costs[x, y] = long.MaxValue;
				}
			}
			Vec? start = null, end = null;
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					switch (input[x, y]) {
						case Tile.Start:
							start = new Vec(x, y);
							break;
						case Tile.Finish:
							end = new Vec(x, y);
							break;
					}
				}
			}
			if (start == null || end == null) throw new Oopsie("Start or finish not found");
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
			Debug();
			switch (costType) {
				case CostType.StartFinish:
					return costs[end.x, end.y];
				case CostType.LongestPath:
					return maxCost;
				default: throw new Oopsie("Unknown CostType");
			}
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
			if (x < 0 || y < 0 || x >= width || y >= height) return;
			if (input[x, y] == Tile.Wall) return;
			if (costs[x, y] < cost) return;
			costs[x, y] = cost;
			maxCost = Math.Max(maxCost, cost);
			dirty.Add(new Vec(x, y));
		}
	}
}
