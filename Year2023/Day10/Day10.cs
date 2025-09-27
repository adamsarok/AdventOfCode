using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023 {
	public class Day10 : IAocSolver {
		public long SolvePart1(string[] input) {
			Queue<Step> q = new Queue<Step>();
			for (int y = 0; y < input.Length; y++) {
				var row = input[y];
				for (int x = 0; x < row.Length; x++) {
					if (row[x] == 'S') {
						visitedCoords.Add(x, new List<int>() { y });
						q.Enqueue(new Step(x + 1, y, Directions.West, 0));
						q.Enqueue(new Step(x - 1, y, Directions.East, 0));
						q.Enqueue(new Step(x, y + 1, Directions.North, 0));
						q.Enqueue(new Step(x, y - 1, Directions.South, 0));
						while (q.Any()) {
							var next = q.Dequeue();
							Visit(next, q);
						}
					}
				}
			}
			return result;
		}

		public long SolvePart2(string[] input) {
			//start flood fill from all edge tiles
			//how do I check if a tile is floodable with the squeeze rule?
			//it would be easy without the squeeze
			return result;
		}

		static Dictionary<int, List<int>> visitedCoords = new Dictionary<int, List<int>>();
		enum Directions { North, East, West, South };
		static int result = 0;
		static char[][] input;

		private static void Queue(int xFrom, int yFrom, int x, int y, Directions direction, int length, Queue<Step> q) {
			Console.WriteLine($"[{xFrom},{yFrom}]={input[yFrom][xFrom]} thisLength={length}");
			result = Math.Max(result, length);
			AddVisited(xFrom, yFrom);
			q.Enqueue(new Step(x, y, direction, length));
		}

		private static void Visit(Step s, Queue<Step> q) {
			if (!CanVisit(s.x, s.y)) return;
			var next = input[s.y][s.x];
			switch (next) {
				case '.':
				case 'S':
					return;
				case '-':
					switch (s.from) {
						case Directions.East:
							Queue(s.x, s.y, s.x - 1, s.y, Directions.East, s.length + 1, q);
							break;
						case Directions.West:
							Queue(s.x, s.y, s.x + 1, s.y, Directions.West, s.length + 1, q);
							break;
					}
					break;
				case '|':
					switch (s.from) {
						case Directions.North:
							Queue(s.x, s.y, s.x, s.y + 1, Directions.North, s.length + 1, q);
							break;
						case Directions.South:
							Queue(s.x, s.y, s.x, s.y - 1, Directions.South, s.length + 1, q);
							break;
					}
					break;
				case 'J':
					switch (s.from) {
						case Directions.North:
							Queue(s.x, s.y, s.x - 1, s.y, Directions.East, s.length + 1, q);
							break;
						case Directions.West:
							Queue(s.x, s.y, s.x, s.y - 1, Directions.South, s.length + 1, q);
							break;
					}
					break;
				case 'L':
					switch (s.from) {
						case Directions.North:
							Queue(s.x, s.y, s.x + 1, s.y, Directions.West, s.length + 1, q);
							break;
						case Directions.East:
							Queue(s.x, s.y, s.x, s.y - 1, Directions.South, s.length + 1, q);
							break;
					}
					break;
				case '7':
					switch (s.from) {
						case Directions.South:
							Queue(s.x, s.y, s.x - 1, s.y, Directions.East, s.length + 1, q);
							break;
						case Directions.West:
							Queue(s.x, s.y, s.x, s.y + 1, Directions.North, s.length + 1, q);
							break;
					}
					break;
				case 'F':
					switch (s.from) {
						case Directions.South:
							Queue(s.x, s.y, s.x + 1, s.y, Directions.West, s.length + 1, q);
							break;
						case Directions.East:
							Queue(s.x, s.y, s.x, s.y + 1, Directions.North, s.length + 1, q);
							break;
					}
					break;
				default:
					throw new NotImplementedException();
			}
		}
		struct Step {
			public int x;
			public int y;
			public Directions from;
			public int length;
			public Step(int x, int y, Directions from, int length) {
				this.x = x;
				this.y = y;
				this.from = from;
				this.length = length;
			}
		}
		private static bool CanVisit(int x, int y) {
			if (y < 0 || y >= input.Length
				|| x < 0 || x >= input[y].Length) {
				return false;
			}
			List<int> l;
			if (visitedCoords.TryGetValue(x, out l)) {
				if (l.Contains(y)) return false;
			}
			return true;
		}
		private static void AddVisited(int x, int y) {
			List<int> l;
			if (!visitedCoords.TryGetValue(x, out l)) {
				l = new List<int>();
				visitedCoords.Add(x, l);
			}
			l.Add(y);
		}
		private static char[][] ReadInput() {
			var lines = File.ReadAllLines("testinput.txt");
			char[][] result = new char[lines.Length][];
			for (int i = 0; i < lines.Length; i++) {
				var s = lines[i];
				var row = new char[s.Length];
				result[i] = row;
				for (int j = 0; j < s.Length; j++) {
					row[j] = s[j];
				}
			}
			return result;
		}

	}
}
