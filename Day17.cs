using System;

namespace AdventOfCode;

public class Day17 {

    public static void SolvePart1() {
		char[][] input = ReadFile("shortinput.txt"); 
		var d = new Dijkstra(input);
		d.Solve();
		// var input2 = ReadFile("testinput.txt"); 
		// var d2 = new Dijkstra(input);
		// d2.Solve();
	}

	private static char[][] ReadFile(string fileName) {
		char[][] input;
		var file = File.ReadAllLines(fileName);
		input = new char[file.Length][];
		for (int i = 0; i < file.Length; i++) {
			input[i] = file[i].ToCharArray();
		}

		return input;
	}

	public class Dijkstra(char[][] input) {
        int[,] distances;

        // we have to store visited as visited FROM a direction, otherwose we would mark the 2nd row as visited
        // starting from the left and we could not turn back (from example input)
        // 456467998645v
        // 12246868655<v
        // 25465488877v5
        // 43226746555v>
        Directions[,] visited;

        [Flags]
        enum Directions { None = 0, 
            Left = 1, 
            Right = 1 << 1, 
            Up = 1 << 2,
            Down = 1 << 3
        }
        PriorityQueue<Vertex, int> priorityQueue;
        record struct Vertex(int x, int y, int cost, Directions lastDir, int dirCount) { }
        public void Solve() {
            //input[0][0] = '0';
			distances = new int[input.Length, input[0].Length];
			visited = new Directions[input.Length, input[0].Length];
			for (int x = 0; x < input[0].Length; x++) {
				for (int y = 0; y < input.Length; y++) {
					distances[y, x] = int.MaxValue;
				}
			}
			priorityQueue = new PriorityQueue<Vertex, int>();
			priorityQueue.Enqueue(new Vertex(0, 0, 0, Directions.None, 0), 0);
			long cnt = 0;
			while (priorityQueue.Count > 0) {
				cnt++;
				var next = priorityQueue.Dequeue();
				//if (next.lastDir != Directions.None && visited[next.y, next.x].HasFlag(next.lastDir)) continue;
				//PrintOut(next);
				visited[next.y, next.x] = visited[next.y, next.x] | next.lastDir;

                //TODO: we now finish in OK time, but something is still not right
                //on the testinput we get 104 instead of 102 :(

				//Console.WriteLine($"Visiting {next.x}:{next.y}");
				//if (IsDone(ref cnt)) break;
				if (next.lastDir != Directions.Down) TryQueue(next, Directions.Up);
				if (next.lastDir != Directions.Up) TryQueue(next, Directions.Down);
				if (next.lastDir != Directions.Right) TryQueue(next, Directions.Left);
				if (next.lastDir != Directions.Left) TryQueue(next, Directions.Right);
			}
			PrintResult(cnt);
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
            if (costTo < distances[y, x]) distances[y, x] = costTo;
 			if (dir != Directions.None && visited[y, x].HasFlag(dir)) return;

            // if (visited[y, x] != Directions.None) return;
            priorityQueue.Enqueue(new Vertex(x, y, costTo, dir, dirCount), costTo);
        }

		private void PrintResult(long cnt) {
			var result = distances[input.Length - 1, input[0].Length - 1];
			var l = result.ToString().Length + 1;
			for (int y = 0; y < input.Length; ++y) {
				for (int x = 0; x < input[0].Length; x++) {
					var d = distances[y, x].ToString();
					Console.Write(d.PadRight(l, ' '));
				}
				Console.WriteLine();
			}
			Console.WriteLine(distances[input.Length - 1, input[0].Length - 1]);
			Console.WriteLine($"in steps: {cnt}");
		}

    }
}
