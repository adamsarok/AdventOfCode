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
			// First get the main loop path from part 1
			SolvePart1(input);

			int height = input.Length;
			int width = input[0].Length;
			bool[,] isOutside = new bool[height * 2 - 1, width * 2 - 1];
			Queue<(int y, int x)> queue = new Queue<(int y, int x)>();

			// Initialize edges of the expanded grid
			for (int y = 0; y < height * 2 - 1; y++) {
				queue.Enqueue((y, 0));
				queue.Enqueue((y, width * 2 - 2));
			}
			for (int x = 0; x < width * 2 - 1; x++) {
				queue.Enqueue((0, x));
				queue.Enqueue((height * 2 - 2, x));
			}

			// Flood fill on a doubled grid to handle squeezing
			while (queue.Count > 0) {
				var (y, x) = queue.Dequeue();
				if (y < 0 || x < 0 || y >= height * 2 - 1 || x >= width * 2 - 1 || isOutside[y, x])
					continue;

				isOutside[y, x] = true;

				// If we're on an actual tile position (not between tiles)
				if (y % 2 == 0 && x % 2 == 0) {
					int realY = y / 2;
					int realX = x / 2;
					// Only spread through non-pipe tiles or unvisited coordinates
					if (!IsPartOfLoop(realX, realY)) {
						queue.Enqueue((y + 2, x)); // Down
						queue.Enqueue((y - 2, x)); // Up
						queue.Enqueue((y, x + 2)); // Right
						queue.Enqueue((y, x - 2)); // Left
					}
				}

				// Handle squeezing - check if we can pass between pipes
				if (y % 2 == 1) { // Vertical gap
					bool canSqueeze = CanSqueezeVertically(x / 2, y / 2);
					if (canSqueeze) {
						queue.Enqueue((y + 1, x));
						queue.Enqueue((y - 1, x));
					}
				}
				if (x % 2 == 1) { // Horizontal gap
					bool canSqueeze = CanSqueezeHorizontally(x / 2, y / 2);
					if (canSqueeze) {
						queue.Enqueue((y, x + 1));
						queue.Enqueue((y, x - 1));
					}
				}
			}

			// Count inside tiles
			int insideCount = 0;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (!IsPartOfLoop(x, y) && !isOutside[y * 2, x * 2]) {
						insideCount++;
					}
				}
			}

			// Write debug output to file
            WriteDebugOutput(input, isOutside);

			return insideCount;
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
