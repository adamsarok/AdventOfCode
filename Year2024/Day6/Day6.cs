using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day6 {
	public class Day6 : Solver {
		public Day6() : base(6) {
		}
		List<char[]> input;
		int guardX, guardY;
		int guardStartX, guardStartY;
		char guard, guardStart;
		List<(int, int)> stepsTaken;
		enum Directions { Up, Right, Down, Left }
		Dictionary<char, Directions> guardDirs = new Dictionary<char, Directions>() {
			{ '^', Directions.Up },
			{ '>', Directions.Right },
			{ 'v', Directions.Down },
			{ '<', Directions.Left }};
		List<char>[,] visiteds;
		protected override void ReadInputPart1(string fileName) {
			input = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				input.Add(l.ToCharArray());
			}
			FindGuard();
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		private void FindGuard() {
			for (int y = 0; y < input.Count; y++) {
				for (int x = 0; x < input[0].Length; x++) {
					var i = input[y][x];
					if (guardDirs.ContainsKey(i)) {
						guardX = x;
						guardY = y;
						guardStartX = x;
						guardStartY = y;
						guard = i;
						guardStart = i;
					}
				}
			}
		}
		private bool InBounds(int x, int y) {
			if (x >= 0 && x < input[0].Length && y >= 0 && y < input.Count) return true;
			return false;
		}

		protected override long SolvePart1() {
			stepsTaken = new();
			Step();
			return stepsTaken.Count + 1;
		}

		private string GetDebug() {
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var l in input) {
				foreach (var c in l) stringBuilder.Append(c);
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		private void Step() {
			int nextX = 0, nextY = 0;
			switch (guardDirs[guard]) {
				case Directions.Up:
					nextX = guardX;
					nextY = guardY - 1;
					break;
				case Directions.Right:
					nextX = guardX + 1;
					nextY = guardY;
					break;
				case Directions.Down:
					nextX = guardX;
					nextY = guardY + 1;
					break;
				case Directions.Left:
					nextX = guardX - 1;
					nextY = guardY;
					break;
			}
			if (!InBounds(nextX, nextY)) return;
			char next = input[nextY][nextX];
			if (next == '#') {
				Turn();
			} else {
				if (input[nextY][nextX] != 'X') {
					input[nextY][nextX] = 'X';
					stepsTaken.Add((nextX, nextY));
				}
				guardX = nextX;
				guardY = nextY;
			}
			Step();
		}

		private bool IsLoop(int nextX, int nextY) {
			var v = visiteds[nextY, nextX];
			if (v == null) v = new List<char>();
			else if (v.Contains(guard)) return true;
			return false;
		}

		private void Turn() {
			int dirId = (int)guardDirs[guard] + 1;
			if (dirId == 4) dirId = 0;
			var nextDir = (Directions)Enum.GetValues(typeof(Directions)).GetValue(dirId);
			foreach (var kvp in guardDirs) {
				if (kvp.Value == nextDir) guard = kvp.Key;
			}
		}

		protected override long SolvePart2() {
			SolvePart1();
			//solve part1, try placing obstructions on the path
			long result = 0;
			(int, int)? lastObstPlaced = null;
			foreach (var step in stepsTaken) {
				CleanUp();
				guardX = guardStartX;
				guardY = guardStartY;
				guard = guardStart;
				visiteds = new List<char>[input.Count, input[0].Length];
				if (lastObstPlaced != null) input[lastObstPlaced.Value.Item2][lastObstPlaced.Value.Item1] = '.';
				input[step.Item2][step.Item1] = 'O';
				lastObstPlaced = (step.Item1, step.Item2);
				if (StepPart2()) result++;
			}
			return result;
		}

		private void CleanUp() {
			for (int y = 0; y < input.Count; y++) {
				for (int x = 0; x < input[0].Length; x++) {
					if (input[y][x] == 'X') input[y][x] = '.';
				}
			}
		}

		private bool StepPart2() {
			int nextX = 0, nextY = 0;
			switch (guardDirs[guard]) {
				case Directions.Up:
					nextX = guardX;
					nextY = guardY - 1;
					break;
				case Directions.Right:
					nextX = guardX + 1;
					nextY = guardY;
					break;
				case Directions.Down:
					nextX = guardX;
					nextY = guardY + 1;
					break;
				case Directions.Left:
					nextX = guardX - 1;
					nextY = guardY;
					break;
			}
			if (!InBounds(nextX, nextY)) {
				return false;
			}
			if (IsLoop(nextX, nextY)) {
				return true;
			}
			char next = input[nextY][nextX];
			if (next == '#' || next == 'O') {
				Turn();
			} else {
				if (input[nextY][nextX] != 'X') {
					input[nextY][nextX] = 'X';
				}
				var v = visiteds[nextY, nextX];
				if (v == null) visiteds[nextY, nextX] = new List<char>() { guard };
				else v.Add(guard);
				guardX = nextX;
				guardY = nextY;
			}
			return StepPart2();
		}
	}
}
