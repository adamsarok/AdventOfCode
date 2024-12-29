using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static Helpers.Enums;

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
			optimalPaths = new bool[input.Length, input[0].Length];
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
						GoFrom(new Vec(x, y), Direction.Right, 0);
					}
				}
			}

			Console.WriteLine($"Solved in {iter} iterations");

			return result;
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
		bool[,] optimalPaths;
		private bool GoFrom(Vec point, Direction facing, long currentScore) {
			iter++;
			if (Go(point, facing, facing, currentScore)) return true; //first try straight line
			switch (facing) {
				case Direction.Up:
				case Direction.Down:
					if (Go(point, facing, Direction.Left, currentScore)) return true;
					if (Go(point, facing, Direction.Right, currentScore)) return true;
					break;
				case Direction.Left:
				case Direction.Right:
					if (Go(point, facing, Direction.Up, currentScore)) return true;
					if (Go(point, facing, Direction.Down, currentScore)) return true;
					break;
			}
			return false;
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
		private bool Go(Vec from, Direction currentFacing, Direction dirToGo, long currentScore) {
			//Debug();
			Vec dest;
			switch (dirToGo) {
				case Direction.Up:
					dest = new Vec(from.x, from.y - 1);
					break;
				case Direction.Down:
					dest = new Vec(from.x, from.y + 1);
					break;
				case Direction.Left:
					dest = new Vec(from.x - 1, from.y);
					break;
				case Direction.Right:
					dest = new Vec(from.x + 1, from.y);
					break;
				default:
					throw new Exception("shouldn't happen");
			}
			if (dest.y < 0 || dest.y >= input.Length || dest.x < 0 || dest.x >= input.Length) return false;
			if (input[dest.y][dest.x] == '#') return false;
			long nextScore = currentScore + (GetTurns(currentFacing, dirToGo) * 1000) + 1;
			if (costs[dest.y, dest.x] < nextScore) return false;
			costs[dest.y, dest.x] = nextScore;
			if (input[dest.y][dest.x] == 'E') {
				result = Math.Min(nextScore, result);
				if (nextScore == target) {
					return true;
				}
			}
			var r = GoFrom(dest, dirToGo, nextScore);
			if (r) {
				optimalPaths[dest.y, dest.x] = true;
			}
			return r;
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
		private void PrintOptimalPaths() {
			Console.Clear();
			for (int y = 0; y < input.Length; y++) {
				for (int x = 0; x < input.Length; x++) {
					if (optimalPaths[y, x]) {
						Console.Write('X');
					} else Console.Write(input[y][x]);
				}
				Console.WriteLine();
			}
		}

	
		long target = long.MinValue;
		long minCost;
		List<Vec> dirs = [new Vec(0, 1), new Vec(0, -1), new Vec(1, 0), new Vec(-1, 0)];
		Dictionary<char, HashSet<Vec>> tiles;
		Dictionary<Vec, long> costs2;
		Stack<Action> actions;
		List<(long s, HashSet<Vec> v)> best;
		protected override long SolvePart2() {
			minCost = long.MaxValue;
			best = new();
			actions = [];
			tiles = new() { { '#', [] }, };
			costs2 = [];
			for (int y = 0; y < input.Length; y++) {
				for (int x = 0; x < input[y].Length; x++) {
					var c = input[y][x];
					if (!tiles.ContainsKey(c)) {
						tiles[c] = [new Vec(x, y)];
					} else {
						tiles[c].Add(new Vec(x, y));
					}
				}
			}
			//for a long time this step was what I was missing - going from S->E than E->S can solve the problem of some turns which would be skipped with a simple Dijkstra
			DoPath([.. tiles['S']], new Vec(1, 0), 0, tiles['E'].First());
			DoPath([.. tiles['E']], new Vec(1, 0), 0, tiles['S'].First());
			return best.Where(x => x.s == minCost).SelectMany(x => x.v)
				.Distinct().Count();
		}

		private void Path(List<Vec> path, Vec lastDir, long lastCost, int dirOrder, Vec dest) {
			if (lastCost > minCost) return;
			foreach (var direction in dirs.Skip(dirOrder).Concat(dirs.Take(dirOrder))) {
				Vec nextCell = path.Last() + direction;
				Vec newDirection = nextCell - path.Last();
				Vec dirDiff = newDirection + lastDir;
				long cost = lastCost;
				if (dirDiff.x == 0 && dirDiff.y == 0) cost += 2000;
				else if (dirDiff.x != 0 && dirDiff.y != 0) cost += 1000;
				cost += 1;
				if (cost > minCost || tiles['#'].Contains(nextCell)
					|| (costs2.TryGetValue(nextCell, out long tileCost) 
					&& cost > tileCost)) {
					continue;
				}
				costs2[nextCell] = cost;
				if (nextCell == dest) {
					minCost = Math.Min(minCost, cost);
					best.Add((cost, [.. path, nextCell]));
					continue;
				}

				actions.Push(() => Path([.. path, nextCell], newDirection, cost, dirOrder, dest));
			}
		}

		void DoPath(List<Vec> path, Vec dir, long cost, Vec dest) {
			for (int dirOrder = 0; dirOrder < 4; dirOrder++) {
				costs2.Clear();
				Path(path, dir, cost, dirOrder, dest);
				while (actions.Count > 0) actions.Pop().Invoke();
			}
		}
	}
}
