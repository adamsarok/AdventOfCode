using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023 {
	public class Day25 : IAocSolver {
		public long SolvePart1(string[] input) {
			long result = 0;

			return result;
		}
		public long SolvePart2(string[] input) {
			long result = 0;

			return result;
		}

		static Dictionary<string, List<string>> connections;

		private class Day25Solver {
			List<HashSet<string>> unions = new List<HashSet<string>>();
			Dictionary<string, List<string>> input;
			HashSet<string> allComponents = new HashSet<string>();
			int maxSubTreeFound = 0;
			int connCount = 0;
			int allComponentsCount = 0;
			public bool Solved => maxSubTreeFound < allComponentsCount;
			public Day25Solver(Dictionary<string, List<string>> input, int allComponentsCount) {
				//Stopwatch sw = Stopwatch.StartNew();
				this.input = input;
				this.allComponentsCount = allComponentsCount;
				BuildUnions();
				connCount = input.Count;
				//maxSubTreeFound = unions.Max(x => x.Count);
				// sw.Stop();
				// Console.WriteLine($"Elapsed {sw.ElapsedMilliseconds} ms, longest subtree: {maxSubTreeFound}");
				if (Solved) Console.WriteLine($"Result: {maxSubTreeFound * (allComponentsCount - maxSubTreeFound)}");
			}
			//47 ms for test input this is now ok?
			private void BuildUnions() {
				unions = new List<HashSet<string>>();
				foreach (var c in input) {
					HashSet<string> union = new HashSet<string>() { c.Key };
					union.UnionWith(c.Value);
					allComponents.UnionWith(union);
					bool intersected = false;
					foreach (var target in unions) {
						if (target.Overlaps(union)) {
							target.UnionWith(union);
							intersected = true;
						}
					}
					if (!intersected) unions.Add(union);
				}
				HashSet<string> final = new HashSet<string>(unions[0]);
				bool haveOverlap = false;
				int i = 1;
				do {
					if (i == unions.Count && haveOverlap) {
						i = 1;
						haveOverlap = false;
					}
					if (final.Overlaps(unions[i])) {
						final.UnionWith(unions[i]);
						unions.RemoveAt(i);
						haveOverlap = true;
					} else {
						i++;
					}
				} while (final.Count < allComponentsCount && unions.Count > 1 && haveOverlap);
				maxSubTreeFound = final.Count;
			}

			public static void SolvePart1() {
				connections = new Dictionary<string, List<string>>();
				HashSet<string> allComponents = new HashSet<string>();
				int allConnections = 0;
				foreach (var l in File.ReadAllLines("testinput.txt")) {
					var from = l.Split(':')[0];
					var to = l.Split(':')[1].Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
					allComponents.Add(from);
					allComponents.UnionWith(to);
					connections.Add(from, to);
					allConnections += to.Count;
				}
				var r = new Remover(connections, allComponents.Count, allConnections);
				//var s = new Solver(connections);
				//ignore three conns, or remove them from the input?
			}

			class Remover {
				public Remover(Dictionary<string, List<string>> input, int allComponentsCount, int allConnections) {
					//this looks terrible : (
					long iterations = 0;
					for (int a = 0; a < allConnections - 2; a++) {
						for (int b = a + 1; b < allConnections - 1; b++) {
							for (int c = b + 1; c < allConnections; c++) {
								int actIndex = 0;
								var copy = new Dictionary<string, List<string>>();
								foreach (var line in input) {
									var elements = new List<string>();
									copy.Add(line.Key, elements);
									foreach (var element in line.Value) {
										if (actIndex != a && actIndex != b && actIndex != c) {
											elements.Add(element);
										}
										actIndex++;
									}
								}
								var s = new Day25Solver(copy, allComponentsCount);
								iterations++;
								if (iterations % 1000 == 0) Console.WriteLine(iterations);
								if (a == 2 && b == 10 && c == 17) {
									foreach (var l in copy) {
										Console.WriteLine($"{l.Key}: {string.Join(' ', l.Value)}");
									}
								}
								if (s.Solved) {
									Console.WriteLine($"Solved!");
									return;
								}

							}

						}
					}
				}
			}


		}
	}
}
