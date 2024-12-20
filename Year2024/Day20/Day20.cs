using Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Helpers.Point;

namespace Year2024.Day20 {
	public class Day20 : Solver {
		private long[,] costs;
		bool[,] dirty;
		private string[] input;
		long height, width;
		Point start;
		private Point end;

		public Day20() : base(2024, 20) {
		}
		protected override void ReadInputPart1(string fileName) {
			input = File.ReadAllLines(fileName);
			height = input.Length;
			width = input[0].Length;
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}


		override protected long SolvePart1() {
			dirty = new bool[height, width];
			costs = new long[height, width];
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (input[y][x] == 'S') {
						start = new Point(x, y);
						costs[x, y] = 0;
						dirty[x, y] = true;
					} else if (input[y][x] == 'E') {
						costs[x, y] = long.MaxValue;
						end = new Point(x, y);
					} else {
						costs[x, y] = long.MaxValue;
					}
				}
			}
			costs[0, 0] = 0;
			dirty[0, 0] = true;
			while (true) {
				bool wasDirty = false;
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						if (dirty[x, y]) {
							dirty[x, y] = false;
							var cost = costs[x, y] + 1;
							Process(x - 1, y, cost);
							Process(x + 1, y, cost);
							Process(x, y - 1, cost);
							Process(x, y + 1, cost);
							wasDirty = true;
						}
					}
				}
				if (!wasDirty) {
					//Debug();
					return FindCheats();
				}
			}
		}

		private long FindCheats() {
			Point next = end;
			cheats = new Dictionary<long, long>();
			while (next != start) {
				long cost = costs[next.x, next.y];
				GetSaved(next, new Point(1, 0));
				GetSaved(next, new Point(-1, 0));
				GetSaved(next, new Point(0, 1));
				GetSaved(next, new Point(0, -1));
				next = FindNext(next, cost - 1);
			}
			//foreach (var s in cheats) {
			//	Console.WriteLine($"There are {s.Value} cheats that save {s.Key} picoseconds.");
			//}
			return cheats.Where(x => x.Key >= 100).Sum(x => x.Value);
		}

		Dictionary<long, long> cheats;

		private char Get(Point point) {
			if (point.x < 0 || point.y < 0 || point.x >= width || point.y >= height) return ' ';
			return input[point.y][point.x];
		}
		private long GetCost(Point point) {
			if (point.x < 0 || point.y < 0 || point.x >= width || point.y >= height) return long.MaxValue;
			return costs[point.x, point.y];
		}

		private void GetSaved(Point from, Point vector) {
			var cheat1 = Get(from + vector);
			if (cheat1 == '#') {
				long walls = 1;
				Point dest;
				var cheat2 = Get(from + vector * 2);
				if (cheat2 == '#') {
					walls = 2;
					dest = from + vector * 3;
				} else dest = from + vector * 2;
				var cDest = GetCost(dest);
				var cFrom = GetCost(from);
				if (cDest == long.MaxValue) return; //previously unreachable cell, should not mater?
				var saved = cFrom - cDest - walls - 1;
				if (saved > 0) {
					if (cheats.ContainsKey(saved)) cheats[saved]++;
					else cheats.Add(saved, 1);
				}
			}
		}

		private Point FindNext(Point next, long nextCost) {
			var xy = new Point(next.x + 1, next.y);
			if (costs[xy.x, xy.y] == nextCost) return xy;
			xy = new Point(next.x - 1, next.y);
			if (costs[xy.x, xy.y] == nextCost) return xy;
			xy = new Point(next.x, next.y + 1);
			if (costs[xy.x, xy.y] == nextCost) return xy;
			xy = new Point(next.x, next.y - 1);
			if (costs[xy.x, xy.y] == nextCost) return xy;
			throw new Oopsie("Path broken");
		}

		private void Process(int x, int y, long cost) {
			if (x < 0 || y < 0 || x >= width || y >= height) return;
			if (input[y][x] == '#') return;
			if (costs[x, y] < cost) return;
			costs[x, y] = cost;
			dirty[x, y] = true;
		}

		private void Debug() {
			Console.Clear();
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (input[y][x] == '#') Console.Write("  #  ");
					else Console.Write(costs[x, y] == long.MaxValue ? " 000 " : $" {costs[x, y].ToString("000")} ");
				}
				Console.WriteLine();
			}
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
