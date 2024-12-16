using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day16 {
	public class Day16 : Solver {
		private string[] input;

		public Day16() : base(2024, 16) {
		}
		protected override void ReadInputPart1(string fileName) {
			input = File.ReadAllLines(fileName);
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}
		long result;
		long iter;
		Stopwatch sw = new Stopwatch();

		protected override long SolvePart1() {
			result = long.MaxValue;
			iter = 0;
			sw.Restart();
			for (int y = 0; y < input.Length; y++) {
				for (int x = 0; x < input.Length; x++) {
					if (input[y][x] == 'S') {
						GoFrom(new Point(x, y), Direction.Right, 0, new bool[input.Length, input[0].Length]);
					}
				}
			}

			Console.WriteLine($"Solved in {iter} iterations");

			//211800 in 2521857 iterations in 86 seconds
			//211800 in 2521857 iterations in 26 seconds
			//211800 in 2454094 iterations in 6 seconds

			//min until now: 157568

			return result;
		}
		enum Direction {
			Up,
			Down,
			Left,
			Right
		}
		private void Debug(HashSet<Point> visited) {
			Console.Clear();
			for (int y = 0; y < input.Length; y++) {
				for (int x = 0; x < input.Length; x++) {
					var p = new Point(x, y);
					if (visited.Contains(p)) Console.Write("X");
					else Console.Write(input[y][x]);
				}
				Console.WriteLine();
			}
		}
		//long[,] costs
		private void GoFrom(Point point, Direction facing, long currentScore, bool[,] visited) {
			iter++;
			visited[point.x, point.y] = true;
			var clone = visited.Clone() as bool[,];
			Go(point, facing, facing, currentScore, clone); //first try straight line
			foreach (var dir in new[]{ Direction.Right, Direction.Up, Direction.Down, Direction.Left }) {
				if (dir != facing) Go(point, facing, dir, currentScore, clone);
			}
		}
		private int GetTurns(Direction currentFacing, Direction dirToGo) {
			switch (currentFacing) {
				case Direction.Up:
					switch (dirToGo) {
						case Direction.Up:
							return 0;
						case Direction.Down:
							return 2;
						case Direction.Left:
							return 1;
						case Direction.Right:
							return 1;
					}
					break;
				case Direction.Down:
					switch (dirToGo) {
						case Direction.Up:
							return 2;
						case Direction.Down:
							return 0;
						case Direction.Left:
							return 1;
						case Direction.Right:
							return 1;
					}
					break;
				case Direction.Left:
					switch (dirToGo) {
						case Direction.Up:
							return 1;
						case Direction.Down:
							return 1;
						case Direction.Left:
							return 0;
						case Direction.Right:
							return 2;
					}
					break;
				case Direction.Right:
					switch (dirToGo) {
						case Direction.Up:
							return 1;
						case Direction.Down:
							return 1;
						case Direction.Left:
							return 2;
						case Direction.Right:
							return 0;
					}
					break;
			}
			throw new Exception("shouldn't happen");
		}
		private void Go(Point from, Direction currentFacing, Direction dirToGo, long currentScore, bool[,] visited) {
			Point dest;
			switch (dirToGo) {
				case Direction.Up:
					dest = new Point(from.x, from.y - 1);
					break;
				case Direction.Down:
					dest = new Point(from.x, from.y + 1);
					break;
				case Direction.Left:
					dest = new Point(from.x - 1, from.y);
					break;
				case Direction.Right:
					dest = new Point(from.x + 1, from.y);
					break;
				default:
					throw new Exception("shouldn't happen");
			}
			if (dest.y < 0 || dest.y >= input.Length || dest.x < 0 || dest.x >= input.Length) return;
			if (input[dest.y][dest.x] == '#') return;
			if (visited[dest.x, dest.y]) return;
			long nextScore = currentScore + (GetTurns(currentFacing, dirToGo) * 1000) + 1;
			if (nextScore >= result) return;
			if (input[dest.y][dest.x] == 'E') {
				//Debug(visited);
				Console.WriteLine($"{nextScore} in {iter} iterations in {sw.ElapsedMilliseconds / 1000} seconds");
				result = Math.Min(nextScore, result);
			}
			GoFrom(dest, dirToGo, nextScore, visited);
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
