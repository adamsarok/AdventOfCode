using Helpers;
using System;
using System.Collections.Generic;
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
		protected override long SolvePart1() {
			result = long.MaxValue;
			for (int y = 0; y < input.Length; y++) {
				for (int x = 0; x < input.Length; x++) {
					if (input[y][x] == 'S') {
						GoFrom(new Point(x, y), Direction.Right, 0, new List<Point>());
					}
				}
			}
			Console.WriteLine($"Solved in {iter} iterations");
			//3999 6352
			return result;
		}
		enum Direction {
			Up,
			Down,
			Left,
			Right
		}
		private void Debug(List<Point> visited) {
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
		private void GoFrom(Point point, Direction facing, long currentScore, List<Point> visited) {
			iter++;
			visited.Add(point);
			Go(point, facing, facing, currentScore, new List<Point>(visited)); //first try straight line
			foreach (var dir in new[]{Direction.Left, Direction.Right, Direction.Up, Direction.Down }) {
				if (dir != facing) Go(point, facing, dir, currentScore, new List<Point>(visited));
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
		private void Go(Point from, Direction currentFacing, Direction dirToGo, long currentScore, List<Point> visited) {
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
			if (visited.Contains(dest)) return;
			long nextScore = currentScore + (GetTurns(currentFacing, dirToGo) * 1000) + 1;
			if (nextScore >= result) return;
			if (input[dest.y][dest.x] == 'E') {
				//Debug(visited);
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
