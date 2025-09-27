using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Year2023 {
	public class Day10 : IAocSolver {
		public long SolvePart1(string[] input) {
			this.input = ParseInput(input);
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
			// Get the main loop path from part 1
			SolvePart1(input);
			int height = input.Length;
			int width = input[0].Length;

			// Build ordered loop path
			var loopPath = BuildOrderedLoopPath();

			int insideCount = 0;
			using (StreamWriter writer = new StreamWriter("debug_output.txt")) {
				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						if (IsPartOfLoop(x, y)) {
							writer.Write(input[y][x]);
						} else if (IsPointInPolygon(x + 0.5, y + 0.5, loopPath)) {
							writer.Write('*');
							insideCount++;
						} else {
							writer.Write('.');
						}
					}
					writer.WriteLine();
				}
			}
			return insideCount;
		}

		private List<(double x, double y)> BuildOrderedLoopPath() {
			// Find the start point
			var start = visitedCoords.SelectMany(kvp => kvp.Value.Select(y => (kvp.Key, y))).First();
			var path = new List<(double x, double y)>();
			var visited = new HashSet<(int, int)>();
			(int, int) current = start;
			visited.Add(current);
			path.Add((current.Item1 + 0.5, current.Item2 + 0.5));
			while (true) {
				bool found = false;
				foreach (var (dx, dy) in new[] { (0, 1), (1, 0), (0, -1), (-1, 0) }) {
					var next = (current.Item1 + dx, current.Item2 + dy);
					if (visited.Contains(next)) continue;
					if (IsPartOfLoop(next.Item1, next.Item2)) {
						visited.Add(next);
						path.Add((next.Item1 + 0.5, next.Item2 + 0.5));
						current = next;
						found = true;
						break;
					}
				}
				if (!found) break;
				if (current == start) break;
			}
			return path;
		}

		private bool IsPointInPolygon(double px, double py, List<(double x, double y)> poly) {
			int crossings = 0;
			for (int i = 0; i < poly.Count; i++) {
				var a = poly[i];
				var b = poly[(i + 1) % poly.Count];
				if (((a.y > py) != (b.y > py)) &&
					(px < (b.x - a.x) * (py - a.y) / (b.y - a.y + 1e-12) + a.x)) {
					crossings++;
				}
			}
			return crossings % 2 == 1;
		}

		private bool IsPartOfLoop(int x, int y) {
			return visitedCoords.TryGetValue(x, out var yList) && yList.Contains(y);
		}

		private bool CanSqueezeVertically(int x, int y) {
			if (y < 0 || y >= input.Length - 1) return true;
			char top = input[y][x];
			char bottom = input[y + 1][x];
			bool topLoop = IsPartOfLoop(x, y);
			bool bottomLoop = IsPartOfLoop(x, y + 1);

			// Only block if both are part of the loop and both connect across the gap
			return !(topLoop && bottomLoop && ConnectsDown(top) && ConnectsUp(bottom));
		}

		private bool CanSqueezeHorizontally(int x, int y) {
			if (x < 0 || x >= input[y].Length - 1) return true;
			char left = input[y][x];
			char right = input[y][x + 1];
			bool leftLoop = IsPartOfLoop(x, y);
			bool rightLoop = IsPartOfLoop(x + 1, y);

			// Only block if both are part of the loop and both connect across the gap
			return !(leftLoop && rightLoop && ConnectsRight(left) && ConnectsLeft(right));
		}

		private bool CanSqueezeDiagonalDR(int x, int y) {
    // Between (x, y) and (x+1, y+1)
    if (x < 0 || y < 0 || x + 1 >= input[0].Length || y + 1 >= input.Length) return true;
    char c1 = input[y][x];
    char c2 = input[y + 1][x + 1];
    bool loop1 = IsPartOfLoop(x, y);
    bool loop2 = IsPartOfLoop(x + 1, y + 1);
    return !(loop1 && loop2 && ConnectsDownLeft(c1) && ConnectsUpRight(c2));
}
private bool CanSqueezeDiagonalDL(int x, int y) {
    // Between (x+1, y) and (x, y+1)
    if (x + 1 >= input[0].Length || y < 0 || x < 0 || y + 1 >= input.Length) return true;
    char c1 = input[y][x + 1];
    char c2 = input[y + 1][x];
    bool loop1 = IsPartOfLoop(x + 1, y);
    bool loop2 = IsPartOfLoop(x, y + 1);
    return !(loop1 && loop2 && ConnectsDownRight(c1) && ConnectsUpLeft(c2));
}
		private bool ConnectsDownLeft(char c) {
			return c == 'J';
		}
		private bool ConnectsUpRight(char c) {
			return c == 'F';
		}
		private bool ConnectsDownRight(char c) {
			return c == 'L';
		}
		private bool ConnectsUpLeft(char c) {
			return c == '7';
		}

		private bool ConnectsUp(char c) {
			// Up: |, F, 7, S
			return c == '|' || c == 'F' || c == '7' || c == 'S';
		}

		private bool ConnectsDown(char c) {
			// Down: |, J, L, S
			return c == '|' || c == 'J' || c == 'L' || c == 'S';
		}

		private bool ConnectsLeft(char c) {
			// Left: -, L, F, S
			return c == '-' || c == 'L' || c == 'F' || c == 'S';
		}	

		private bool ConnectsRight(char c) {
			// Right: -, J, 7, S
			return c == '-' || c == 'J' || c == '7' || c == 'S';
		}

		Dictionary<int, List<int>> visitedCoords = new Dictionary<int, List<int>>();
		enum Directions { North, East, West, South };
		int result = 0;
		char[][] input;
		private static char[][] ParseInput(string[] lines) {
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

		private void Queue(int xFrom, int yFrom, int x, int y, Directions direction, int length, Queue<Step> q) {
			Console.WriteLine($"[{xFrom},{yFrom}]={input[yFrom][xFrom]} thisLength={length}");
			result = Math.Max(result, length);
			AddVisited(xFrom, yFrom);
			q.Enqueue(new Step(x, y, direction, length));
		}

		private void Visit(Step s, Queue<Step> q) {
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
		private bool CanVisit(int x, int y) {
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
		private void AddVisited(int x, int y) {
			List<int> l;
			if (!visitedCoords.TryGetValue(x, out l)) {
				l = new List<int>();
				visitedCoords.Add(x, l);
			}
			l.Add(y);
		}

		private void WriteDebugOutput(string[] input, bool[,] isOutside) {
            int height = input.Length;
            int width = input[0].Length;
            
            using (StreamWriter writer = new StreamWriter("debug_output.txt")) {
                // Write original input with inside tiles marked
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        if (!IsPartOfLoop(x, y) && !isOutside[y * 2, x * 2]) {
                            writer.Write('*'); // Inside tile
                        } else if (IsPartOfLoop(x, y)) {
                            writer.Write(input[y][x]); // Loop tile
                        } else {
                            writer.Write('.'); // Outside tile
                        }
                    }
                    writer.WriteLine();
                }
            }
        }
	}
}
