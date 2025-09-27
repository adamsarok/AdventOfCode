using Helpers;

namespace Year2023 {
	public class Day12 : IAocSolver {
		public long SolvePart1(string[] input) {
			long result = 0;
			foreach (var line in input) {
				var solver = new Day12Solver(line.Split(' '), 1);
				result += solver.CountArrangements();
			}
			return result;
		}

		public long SolvePart2(string[] input) {
			long result = 0;
			foreach (var line in input) {
				var solver = new Day12Solver(line.Split(' '), 5);
				result += solver.CountArrangements();
			}
			return result;
		}

		private class Day12Solver {
			private string springs;
			private int[] groups;
			private Dictionary<(int, int, int), long> memo = new Dictionary<(int, int, int), long>();

			public Day12Solver(string[] inputs, int repeat) {
				var springParts = new List<string>();
				var groupParts = new List<string>();
				
				for (int i = 0; i < repeat; i++) {
					springParts.Add(inputs[0]);
					groupParts.Add(inputs[1]);
				}
				
				springs = string.Join("?", springParts);
				groups = string.Join(",", groupParts)
					.Split(',')
					.Select(int.Parse)
					.ToArray();
			}

			public long CountArrangements() {
				return CountArrangements(0, 0, 0);
			}

			private long CountArrangements(int springIndex, int groupIndex, int currentGroupLength) {
				var key = (springIndex, groupIndex, currentGroupLength);
				if (memo.ContainsKey(key)) {
					return memo[key];
				}

				if (springIndex == springs.Length) {
					if (groupIndex == groups.Length && currentGroupLength == 0) {
						return 1;
					}
					if (groupIndex == groups.Length - 1 && currentGroupLength == groups[groupIndex]) {
						return 1;
					}
					return 0;
				}

				long count = 0;
				char current = springs[springIndex];

				// Try placing a damaged spring (#)
				if (current == '#' || current == '?') {
					count += CountArrangements(springIndex + 1, groupIndex, currentGroupLength + 1);
				}

				// Try placing an operational spring (.)
				if (current == '.' || current == '?') {
					if (currentGroupLength == 0) {
						// Not in a group, just continue
						count += CountArrangements(springIndex + 1, groupIndex, 0);
					} else if (groupIndex < groups.Length && currentGroupLength == groups[groupIndex]) {
						// Finished a group, move to next group
						count += CountArrangements(springIndex + 1, groupIndex + 1, 0);
					}
					// If currentGroupLength > 0 but doesn't match the expected group size, this is invalid
				}

				memo[key] = count;
				return count;
			}
		}
	}
}
