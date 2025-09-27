using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Year2023 {
	public class Day17 : IAocSolver {
		private string[] input;
		private int height;
		private int width;

		public long SolvePart1(string[] input) {
			this.input = input;
			height = input.Length;
			width = input[0].Length;
			return FindMinHeatLoss(1, 3);
		}

		public long SolvePart2(string[] input) {
			this.input = input;
			height = input.Length;
			width = input[0].Length;
			return FindMinHeatLoss(4, 10);
		}

		private long FindMinHeatLoss(int minSteps, int maxSteps) {
			// State: (row, col, direction, consecutive_steps)
			var distances = new Dictionary<State, int>();
			var pq = new PriorityQueue<State, int>();
			
			// Initialize with starting positions (can go right or down from start)
			var startRight = new State(0, 0, Direction.Right, 0);
			var startDown = new State(0, 0, Direction.Down, 0);
			
			distances[startRight] = 0;
			distances[startDown] = 0;
			pq.Enqueue(startRight, 0);
			pq.Enqueue(startDown, 0);

			while (pq.Count > 0) {
				var current = pq.Dequeue();
				var currentDist = distances[current];

				// Check if we reached the destination
				if (current.Row == height - 1 && current.Col == width - 1) {
					return currentDist;
				}

				// Try all possible moves
				foreach (var direction in Enum.GetValues<Direction>()) {
					var newRow = current.Row + GetRowDelta(direction);
					var newCol = current.Col + GetColDelta(direction);

					// Check bounds
					if (newRow < 0 || newRow >= height || newCol < 0 || newCol >= width) {
						continue;
					}

					// Check if we can't go in the opposite direction
					if (IsOppositeDirection(current.Direction, direction)) {
						continue;
					}

					var newConsecutive = (current.Direction == direction) ? current.ConsecutiveSteps + 1 : 1;

					// Check consecutive step constraints
					if (newConsecutive > maxSteps) {
						continue;
					}

					// For Part 2: must move at least minSteps before turning
					if (current.ConsecutiveSteps > 0 && current.ConsecutiveSteps < minSteps && current.Direction != direction) {
						continue;
					}

					var newState = new State(newRow, newCol, direction, newConsecutive);
					var newDist = currentDist + GetHeatLoss(newRow, newCol);

					if (!distances.ContainsKey(newState) || newDist < distances[newState]) {
						distances[newState] = newDist;
						pq.Enqueue(newState, newDist);
					}
				}
			}

			return -1; // Should not reach here
		}

		private int GetHeatLoss(int row, int col) {
			return int.Parse(input[row][col].ToString());
		}

		private int GetRowDelta(Direction direction) {
			return direction switch {
				Direction.Up => -1,
				Direction.Down => 1,
				_ => 0
			};
		}

		private int GetColDelta(Direction direction) {
			return direction switch {
				Direction.Left => -1,
				Direction.Right => 1,
				_ => 0
			};
		}

		private bool IsOppositeDirection(Direction dir1, Direction dir2) {
			return (dir1 == Direction.Up && dir2 == Direction.Down) ||
				   (dir1 == Direction.Down && dir2 == Direction.Up) ||
				   (dir1 == Direction.Left && dir2 == Direction.Right) ||
				   (dir1 == Direction.Right && dir2 == Direction.Left);
		}

		private enum Direction {
			Up, Down, Left, Right
		}

		private record State(int Row, int Col, Direction Direction, int ConsecutiveSteps);
	}
}
