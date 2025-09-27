using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023 {
	public class Day11 : IAocSolver {
		public long SolvePart1(string[] input) {
			return SolveDist(input, 2);
		}

		public long SolvePart2(string[] input) {
			return SolveDist(input, 1000000);
		}

		private long SolveDist(string[] input, int d) {
			List<(int, int)> galaxies = new List<(int, int)>();
			List<int> emptyRows = new List<int>();
			List<int> emptyCols = new List<int>();
			for (int y = 0; y < input.Length; y++) {
				var line = input[y];
				bool rowEmpty = true;
				for (int x = 0; x < line.Length; x++) {
					if (line[x] == '#') {
						galaxies.Add((x, y));
						rowEmpty = false;
					}
				}
				if (rowEmpty) emptyRows.Add(y);
			}
			for (int x = 0; x < input[0].Length; x++) {
				if (!galaxies.Any(g => g.Item1 == x)) emptyCols.Add(x);
			}
			long result = 0;
			int emptiesAdded = 0;
			for (int i = 0; i < galaxies.Count - 1; i++) {
				for (int j = i + 1; j < galaxies.Count; j++) {
					var g1 = galaxies[i];
					var g2 = galaxies[j];
					var emptyRowsBetween = (g1.Item2 < g2.Item2 ?
						emptyRows.Where(y => y > g1.Item2 && y < g2.Item2) :
						emptyRows.Where(y => y < g1.Item2 && y > g2.Item2)).ToList();
					var emptyColsBetween = (g1.Item1 < g2.Item1 ?
						emptyCols.Where(x => x > g1.Item1 && x < g2.Item1) :
						emptyCols.Where(x => x < g1.Item1 && x > g2.Item1)).ToList();
					emptiesAdded += emptyColsBetween.Count() + emptyRowsBetween.Count();
					int hDist = Math.Abs(g1.Item1 - g2.Item1) + (emptyColsBetween.Count() * (d - 1));
					int vDist = Math.Abs(g1.Item2 - g2.Item2) + (emptyRowsBetween.Count() * (d - 1));
					int dist = hDist + vDist;
					result += dist;
				}
			}
			Console.WriteLine($"Empties added: {emptiesAdded}");
			return result;
		}
	}
}
