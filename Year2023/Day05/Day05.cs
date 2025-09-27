using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023 {
	public class Day05 : IAocSolver {
		public class Map(long sourceStart, long destStart, long range) {
			public long GetMapValue(long source) {
				if (source < sourceStart || source > SourceEnd) return -1;
				return destStart + (source - sourceStart);
			}
			public long SourceStart => sourceStart;
			public long SourceEnd => sourceStart + range - 1;
			public long DestEnd => destStart + range - 1;
		}
		public class Level {
			public string Name { get; set; }
			public List<Map> Maps { get; set; } = new List<Map>();
			public void FillGaps() {
				long act = 0;
				var temp = new List<Map>();
				foreach (var map in Maps.OrderBy(x => x.SourceStart)) {
					if (map.SourceStart > act) {
						temp.Add(new Map(sourceStart: act, destStart: act, range: map.SourceStart - act));
					}
					temp.Add(map);
					act = map.SourceEnd + 1;
				}
				temp.Add(new Map(sourceStart: act, destStart: act, range: long.MaxValue - act));
				Maps = temp;
			}
		}
		public long SolvePart1(string[] input) {
			var seebs = new List<long>();
			var levels = new List<Level>();
			var level = new Level();
			foreach (var line in input) {
				if (!seebs.Any()) {
					var s = line.Split(':')[1].Split(' ');
					foreach (var seeb in s) {
						if (!string.IsNullOrWhiteSpace(seeb)) {
							seebs.Add(long.Parse(seeb));
						}
					}
				} else {
					if (!string.IsNullOrWhiteSpace(line)) {
						if (!char.IsNumber(line[0])) {
							level = new Level() {
								Name = line
							};
							levels.Add(level);
						} else {
							var split = line.Split(' ');
							level.Maps.Add(new Map(
								sourceStart: long.Parse(split[1]),
								destStart: long.Parse(split[0]),
								range: long.Parse(split[2])
							));
						}
					}
				}
			}
			long result = long.MaxValue;
			foreach (var seeb in seebs) {
				long source = seeb;
				foreach (var l in levels) {
					foreach (var map in l.Maps) {
						var nextKey = map.GetMapValue(source);
						if (nextKey <= 0) continue;
						source = nextKey;
						break;
					}
				}
				result = Math.Min(result, source);
			}
			return result;
		}

		public long SolvePart2(string[] input) {
			var seebs = new List<long>();
			var levels = new List<Level>();
			var level = new Level();
			foreach (var line in input) {
				if (!seebs.Any()) {
					var s = line.Split(':')[1].Split(' ');
					foreach (var seeb in s) {
						if (!string.IsNullOrWhiteSpace(seeb)) {
							seebs.Add(long.Parse(seeb));
						}
					}
				} else {
					if (!string.IsNullOrWhiteSpace(line)) {
						if (!char.IsNumber(line[0])) {
							level = new Level() {
								Name = line
							};
							levels.Add(level);
						} else {
							var split = line.Split(' ');
							level.Maps.Add(new Map(
								sourceStart: long.Parse(split[1]),
								destStart: long.Parse(split[0]),
								range: long.Parse(split[2])
							));
						}
					}
				}
			}
			foreach (var l in levels) l.FillGaps();
			long result = long.MaxValue;
			for (int s = 0; s <= seebs.Count / 2; s += 2) {
				long start = seebs[s];
				long end = seebs[s] + seebs[s + 1];
				var maps = new List<Map>() {
					new Map(
						sourceStart: seebs[s],
						destStart: seebs[s],
						range: seebs[s + 1]
					)
				};
				foreach (var l in levels) {
					var newMaps = new List<Map>();
					foreach (var map in maps) {
						newMaps.AddRange(ProcessMap(l, map));
					}
					maps = newMaps;
				}
				result = Math.Min(result, maps.Min(x => x.SourceStart));
			}
			return result;
		}
		private static List<Map> ProcessMap(Level l, Map s) {
			var result = new List<Map>();
			var intersecting = l.Maps
				.Where(d =>
					(d.SourceStart <= s.SourceStart && d.SourceEnd >= s.SourceStart)
					|| (d.SourceStart >= s.SourceStart && d.SourceStart <= s.SourceEnd))
				.ToList();
			Console.WriteLine($"{l.Name} {s.SourceStart}:{s.SourceEnd}->");
			foreach (var intersect in intersecting) {
				var start = Math.Max(s.SourceStart, intersect.SourceStart);
				var end = Math.Min(s.SourceEnd, intersect.SourceEnd);
				Console.Write($"{start}:{end}->");
				var map = new Map(
					sourceStart: intersect.GetMapValue(start),
					destStart: -1, //not used here, doesn't matter
					range: end - start + 1
				);
				Console.WriteLine($"{map.SourceStart}:{map.SourceEnd}");
				result.Add(map);
			}
			return result;
		}
	}
}
