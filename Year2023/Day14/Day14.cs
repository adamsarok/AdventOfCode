using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023 {
	public class Day14 : IAocSolver {
		const char ROLLING = 'O';
		const char EMPTY = '.';
		public enum Directions { N, E, S, W }
		public long SolvePart1(string[] input) {
			List<char[]> lines = ReadInput(input);
			Roll(lines, Directions.N);
			Console.WriteLine("After:");
			WriteState(lines);
			return GetResult(lines);
		}

		public long SolvePart2(string[] input) {
			List<char[]> lines = ReadInput(input);
			var cl = new CycleDetector();
			for (long i = 0; i < 1000; i++) {
				Roll(lines, Directions.N);
				Roll(lines, Directions.W);
				Roll(lines, Directions.S);
				Roll(lines, Directions.E);
				var sum = GetResult(lines);
				var r = cl.Detect(sum);
				Console.WriteLine($"Iteration: {i} => {sum}");
				if (r > 0) {
					bool found = true;
				}
			}
			return 0;
		}

		private static int GetResult(List<char[]> lines) {
			int result = 0;
			int mul = lines.Count;
			foreach (var l in lines) {
				result += mul * l.Where(x => x == ROLLING).Count();
				mul--;
			}
			return result;
		}

		class CycleDetector {
			int length = 0;
			int varalueToFindCL = 0;
			static HashSet<int> results = new HashSet<int>();
			int sameCycles = 0;
			int lastCycle = 0;
			public int Detect(int val) {
				length++;
				if (!results.Contains(val)) {
					results.Add(val);
				} else {
					if (varalueToFindCL == 0) {
						varalueToFindCL = val;
						length = 0;
					} else if (val == varalueToFindCL) {
						if (lastCycle == length) {
							sameCycles++;
							if (sameCycles >= 10) return lastCycle;
						}
						lastCycle = length;
						length = 0;
					}
				}
				return -1;
			}
		}

		private static void Roll(List<char[]> lines, Directions dir) {
			switch (dir) {
				case Directions.N:
					for (int x = 0; x < lines[0].Length; x++) {
						for (int y = 1; y < lines.Count; y++) {
							if (lines[y][x] == ROLLING) {
								int rollto = y - 1;
								while (rollto >= 0 && lines[rollto][x] == EMPTY) {
									lines[rollto + 1][x] = EMPTY;
									lines[rollto][x] = ROLLING;
									rollto--;
								}
							}
						}
					}
					break;
				case Directions.S:
					for (int x = 0; x < lines[0].Length; x++) {
						for (int y = lines.Count - 2; y >= 0; y--) {
							if (lines[y][x] == ROLLING) {
								int rollto = y + 1;
								while (rollto < lines.Count && lines[rollto][x] == EMPTY) {
									lines[rollto - 1][x] = EMPTY;
									lines[rollto][x] = ROLLING;
									rollto++;
								}
							}
						}
					}
					break;
				case Directions.W:
					for (int y = 0; y < lines.Count; y++) {
						for (int x = 0; x < lines[0].Length; x++) {
							if (lines[y][x] == ROLLING) {
								int rollto = x - 1;
								while (rollto >= 0 && lines[y][rollto] == EMPTY) {
									lines[y][rollto + 1] = EMPTY;
									lines[y][rollto] = ROLLING;
									rollto--;
								}
							}
						}
					}
					break;
				case Directions.E:
					for (int y = 0; y < lines.Count; y++) {
						for (int x = lines[0].Length - 2; x >= 0; x--) {
							if (lines[y][x] == ROLLING) {
								int rollto = x + 1;
								while (rollto < lines[0].Length && lines[y][rollto] == EMPTY) {
									lines[y][rollto - 1] = EMPTY;
									lines[y][rollto] = ROLLING;
									rollto++;
								}
							}
						}
					}
					break;
			}
		}

		private static List<char[]> ReadInput(string[] input) {
			var lines = new List<char[]>();
			foreach (var l in input) {
				lines.Add(l.ToCharArray());
			}
			WriteState(lines);
			return lines;
		}

		static void WriteState(List<char[]> state) {
			foreach (var l in state) {
				Console.WriteLine(new string(l));
			}
		}
	}
}
