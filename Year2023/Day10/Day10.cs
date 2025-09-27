using Helpers;

namespace Year2023;
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
		SolvePart1(input);


		int height = input.Length;
		int width = input[0].Length;
		int insideCount = 0;

		for (int y = 0; y < height; y++) {
			bool inside = false;
			char lastCorner = ' ';

			for (int x = 0; x < width; x++) {
				if (IsPartOfLoop(x, y)) {
					char c = input[y][x];

					// Replace S with the actual pipe type it represents
					if (c == 'S') {
						c = GetStartPipeType(x, y, input);
					}

					// Ray casting: count crossings
					if (c == '|') {
						inside = !inside;
					} else if (c == 'F' || c == 'L') {
						lastCorner = c;
					} else if (c == '7') {
						if (lastCorner == 'L') {
							inside = !inside;
						}
						lastCorner = ' ';
					} else if (c == 'J') {
						if (lastCorner == 'F') {
							inside = !inside;
						}
						lastCorner = ' ';
					}
					// '-' doesn't change anything, just continues the horizontal segment
				} else if (inside) {
					insideCount++;
				}
			}
		}

		Console.WriteLine($"Inside count: {insideCount}");
		return insideCount;
	}

	private bool IsPartOfLoop(int x, int y) {
		return visitedCoords.TryGetValue(x, out var yList) && yList.Contains(y);
	}

	private char GetStartPipeType(int x, int y, string[] input) {
		// Find the start position
		var start = visitedCoords.SelectMany(kvp => kvp.Value.Select(y => (kvp.Key, y))).First();
		if (x != start.Item1 || y != start.Item2) {
			return 'S'; // Not the start position
		}

		// Determine what pipe type S should be based on its connections
		bool hasNorth = y > 0 && IsPartOfLoop(x, y - 1) && (input[y - 1][x] == '|' || input[y - 1][x] == '7' || input[y - 1][x] == 'F');
		bool hasSouth = y < input.Length - 1 && IsPartOfLoop(x, y + 1) && (input[y + 1][x] == '|' || input[y + 1][x] == 'J' || input[y + 1][x] == 'L');
		bool hasEast = x < input[0].Length - 1 && IsPartOfLoop(x + 1, y) && (input[y][x + 1] == '-' || input[y][x + 1] == 'J' || input[y][x + 1] == '7');
		bool hasWest = x > 0 && IsPartOfLoop(x - 1, y) && (input[y][x - 1] == '-' || input[y][x - 1] == 'L' || input[y][x - 1] == 'F');

		if (hasNorth && hasSouth) return '|';
		if (hasEast && hasWest) return '-';
		if (hasNorth && hasEast) return 'L';
		if (hasNorth && hasWest) return 'J';
		if (hasSouth && hasEast) return 'F';
		if (hasSouth && hasWest) return '7';

		return 'S'; // Fallback
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
}
