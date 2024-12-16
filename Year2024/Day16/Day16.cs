using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
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
			costs = new long[input.Length, input[0].Length];
			for (int y = 0; y < input.Length; y++) {
				for (int x = 0; x < input[0].Length; x++) {
					costs[x, y] = long.MaxValue;
				}
			}
			for (int y = 0; y < input.Length; y++) {
				for (int x = 0; x < input[0].Length; x++) {
					if (input[y][x] == 'S') {
						costs[y,x] = -1;
						GoFrom(new Point(x, y), Direction.Right, 0, 0);
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
		private void Debug() {
			Console.Clear();
			for (int y = 0; y < input.Length; y++) {
				for (int x = 0; x < input.Length; x++) {
					if (costs[y, x] < long.MaxValue) Console.Write("X");
					else Console.Write(input[y][x]);
				}
				Console.WriteLine();
			}
		}
		long[,] costs;
		private void GoFrom(Point point, Direction facing, long currentScore, int steps) {
			iter++;
			Go(point, facing, facing, currentScore, steps + 1); //first try straight line
			switch (facing) {
				case Direction.Up:
				case Direction.Down:
					Go(point, facing, Direction.Left, currentScore, steps + 1);
					Go(point, facing, Direction.Right, currentScore, steps + 1);
					break;
				case Direction.Left:
				case Direction.Right:
					Go(point, facing, Direction.Up, currentScore, steps + 1);
					Go(point, facing, Direction.Down, currentScore, steps + 1);
					break;
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
		private void Go(Point from, Direction currentFacing, Direction dirToGo, long currentScore, int steps) {
			//Debug();
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
			long nextScore = currentScore + (GetTurns(currentFacing, dirToGo) * 1000) + 1;
			if (costs[dest.y, dest.x] < nextScore) return;
			costs[dest.y, dest.x] = nextScore;
			//if (nextScore >= result) return;
			if (input[dest.y][dest.x] == 'E') {
				//Debug();
				//Console.WriteLine($"{nextScore} in {iter} iterations in {sw.ElapsedMilliseconds / 1000} seconds");
				//if (nextScore == target) {
				//	part2result += steps;
				//}
				result = Math.Min(nextScore, result);
			}
			GoFrom(dest, dirToGo, nextScore, steps);
		}
		private void PrintCosts() {
			Console.Clear();
			for (int y = 0; y < input.Length; y++) {
				for (int x = 0; x < input.Length; x++) {
					if (costs[y, x] < long.MaxValue) {
						Console.Write(costs[y, x].ToString().PadRight(5));
					} else Console.Write($"  {input[y][x]}  ");
				}
				Console.WriteLine();
			}
		}

//We can find one best path with this logic, but not all best paths
//3008 4009 3010 -> here the path through 4009 is skipped even though its the same cost as through 3009
//#    #    #    #    #    #    #    #    #    #    #    #    #    #    #
//#  5016 6017 6018 6019 6020 6021 6022   #  8040 8039 8038 8037 7036   #
//#  5015   #  7017   #    #    #  7023   #  9023   #    #    #  7035   #
//#  5014 6015 6016 6017 6018   #  7024   #  8022 8021 7020   #  7034   #
//#  5013   #    #    #  7019   #    #    #    #    #  7019   #  7033   #
//#  5012   #  3010   #  6020 6019 6018 6017 5016 6017 6018   #  7032   #
//#  5011   #  3009   #    #    #    #    #  5015   #    #    #  7031   #
//#  4010 4009 3008 4009 3010 4011 4012 4013 4014 4015 4016   #  7030   #
//#    #    #  3007   #  3009   #    #    #    #    #  5017   #  7029   #
//#  1004 2005 2006   #  3008 4009 4010 4011 4012   #  5018   #  7028   #
//#  1003   #  3005   #  3007   #    #    #  5013   #  5019   #  7027   #
//#  1002 2003 2004 2005 2006   #  5012 6013 5014   #  5020   #  7026   #
//#  1001   #    #    #  3007   #  5011   #  5013   #  5021   #  7025   #
//#   -1    1    2    #  3008 4009 4010 4011 4012   #  5022 6023 6024   #
//#    #    #    #    #    #    #    #    #    #    #    #    #    #    #


		long part2result;
		long target = long.MinValue;
		protected override long SolvePart2() {
			part2result = 0;
			SolvePart1();
			target = result; //hacky
			SolvePart1();
			//PrintCosts();
			//for (int y = 0; y < input.Length; y++) {
			//	for (int x = 0; x < input[0].Length; x++) {
			//		if (input[y][x] == 'E') {
			//			CountBack(new Point(x, y));
			//		}
			//	}
			//}
			return part2result;
		}
		private long GetCost(long y, long x) {
			if (x < 0 || x >= input[0].Length || y < 0 || y >= input.Length) return long.MaxValue;
			return costs[y, x];
		}
		private void CountBack(Point point) {
			long min = new long[]{
				GetCost(point.y + 1, point.x),
				GetCost(point.y - 1, point.x),
				GetCost(point.y, point.x + 1),
				GetCost(point.y, point.x - 1)
			}.Min();
			if (min < 0) return;
			if (GetCost(point.y + 1, point.x) == min) {
				part2result++;
				CountBack(new Point(point.x, point.y + 1));
			}
			if (GetCost(point.y - 1, point.x) == min) {
				part2result++;
				CountBack(new Point(point.x, point.y - 1));
			}
			if (GetCost(point.y, point.x + 1) == min) {
				part2result++;
				CountBack(new Point(point.x + 1, point.y));
			}
			if (GetCost(point.y, point.x - 1) == min) {
				part2result++;
				CountBack(new Point(point.x - 1, point.y + 1));
			}
		}
	}
}
