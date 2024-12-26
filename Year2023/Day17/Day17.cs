using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023.Day17 {
	public class Day17 : Solver {
		private string[] input;
		private int height;
		private int width;

		public Day17() : base(2023, 17) {
		}
		protected override void ReadInputPart1(string fileName) {
			input = File.ReadAllLines(fileName);
			height = input.Length;
			width = input[0].Length;
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}



		int[,] costs;

		// we have to store visited as visited FROM a direction, otherwose we would mark the 2nd row as visited
		// starting from the left and we could not turn back (from example input)

		Directions[,] visited;

		[Flags]
		enum Directions {
			None = 0,
			Left = 1,
			Right = 1 << 1,
			Up = 1 << 2,
			Down = 1 << 3
		}
		HashSet<Vertex> dirty;

		//this whole thing is bad, none of the costs match, what was I doing here?

		record struct Vertex(int x, int y, int cost, Directions lastDir, int dirCount) { }
		protected override long SolvePart1() {
			//there is an error somewhere - if I check visited early, I don't get the optimal route 
			//if I check late there are too many queued vertices but result is correct
			//there is a fundamental error how dijkstra is implemented for this matrix

			//input[0][0] = '0';
			costs = new int[width, height];
			visited = new Directions[width, height];
			for (int x = 0; x < input[0].Length; x++) {
				for (int y = 0; y < input.Length; y++) {
					costs[y, x] = int.MaxValue;
				}
			}
			dirty = new HashSet<Vertex>();
			dirty.Add(new Vertex(0, 0, 0, Directions.None, 0));
			long cnt = 0;
			while (dirty.Count > 0) {
				//cnt++;
				var next = dirty.First();
				dirty.Remove(next);
				//if (next.lastDir != Directions.None && visited[next.y, next.x].HasFlag(next.lastDir)) continue;
				//PrintOut(next);
				visited[next.x, next.y] = visited[next.x, next.y] | next.lastDir;

				//TODO: we now finish in OK time, but something is still not right
				//on the testinput we get 104 instead of 102 :(

				//Console.WriteLine($"Visiting {next.x}:{next.y}");
				//if (IsDone(ref cnt)) break;
				if (next.lastDir != Directions.Down) TryQueue(next, Directions.Up);
				if (next.lastDir != Directions.Up) TryQueue(next, Directions.Down);
				if (next.lastDir != Directions.Right) TryQueue(next, Directions.Left);
				if (next.lastDir != Directions.Left) TryQueue(next, Directions.Right);
				Debug();
			}
			Debug();
			return cnt;
		}

		private void Debug( List<Vec> blue = null, List<Vec> red = null) {
			Console.Clear();
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (blue != null && blue.Any(p => p.x == x && p.y == y)) {
						Console.ForegroundColor = ConsoleColor.Blue;
					}
					if (red != null && red.Any(p => p.x == x && p.y == y)) {
						Console.ForegroundColor = ConsoleColor.Red;
					}
					if (input[y][x] == '#') Console.Write(height > 20 ? "#" : "  #  ");
					else {
						if (height > 20) Console.Write(input[y][x]);
						else Console.Write(costs[x, y] == long.MaxValue ? " 000 " : $" {costs[x, y].ToString("000")} ");
					}
					Console.ResetColor();
				}
				Console.WriteLine();
			}
			Thread.Sleep(250);
		}


		private void TryQueue(Vertex prev, Directions dir) {
			int x = prev.x, y = prev.y;
			int dirCount = prev.lastDir == dir ? prev.dirCount + 1 : 1;
			if (dirCount > 3) {
				return;
			}
			switch (dir) {
				case Directions.Left:
					x--;
					break;
				case Directions.Right:
					x++;
					break;
				case Directions.Up:
					y--;
					break;
				case Directions.Down:
					y++;
					break;
			}
			if (x < 0 || y < 0 ||
				x >= input[0].Length || y >= input.Length) return;
			var costTo = int.Parse((input[y][x]).ToString()) + prev.cost;
			if (costTo < costs[y, x]) costs[y, x] = costTo;
			if (dir != Directions.None && visited[x, y].HasFlag(dir)) return;

			// if (visited[y, x] != Directions.None) return;
			dirty.Add(new Vertex(x, y, costTo, dir, dirCount));
		}

		private void PrintResult(long cnt) {
			var result = costs[input.Length - 1, input[0].Length - 1];
			var l = result.ToString().Length + 1;
			for (int y = 0; y < input.Length; ++y) {
				for (int x = 0; x < input[0].Length; x++) {
					var d = costs[y, x].ToString();
					Console.Write(d.PadRight(l, ' '));
				}
				Console.WriteLine();
			}
			Console.WriteLine(costs[input.Length - 1, input[0].Length - 1]);
			Console.WriteLine($"in steps: {cnt}");
		}


	}
}
