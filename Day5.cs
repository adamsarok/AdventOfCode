
namespace AdventOfCode;

public class Day5 {
    public class Map {
        public Map(long sourceStart, long destStart, long range) {
            SourceStart = sourceStart;
            DestStart = destStart;
            Range = range;
        }
        public long GetMapValue(long source) {
            if (source < SourceStart || source > SourceEnd) return -1;
            return DestStart + (source - SourceStart);
        }
        public long SourceStart { get; set; }
        public long SourceEnd => SourceStart + Range - 1;
        public long DestEnd => DestStart + Range - 1;
        public long DestStart { get; set; }
        public long Range { get; set; }
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
    public static void SolvePart1() {
        var lines = File.ReadAllLines("day5input.txt");
        var seebs = new List<long>();
        var levels = new List<Level>();
        var level = new Level();
        foreach (var line in lines) {
            if (!seebs.Any()) {
                var s = line.Split(':')[1].Split(' ');
                foreach (var seeb in s) {
                    if (!string.IsNullOrWhiteSpace(seeb)) {
                        seebs.Add(long.Parse(seeb));
                    }
                }
            }
            else {
                if (!string.IsNullOrWhiteSpace(line)) {
                    if (!char.IsNumber(line[0])) {
                        level = new Level() {
                            Name = line
                        };
                        levels.Add(level);
                    }
                    else {
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
        Console.WriteLine($"Result: {result}");
    }
    public static void SolvePart2() {
        var lines = File.ReadAllLines("day5input.txt");
        var seebs = new List<long>();
        var levels = new List<Level>();
        var level = new Level();
        foreach (var line in lines) {
            if (!seebs.Any()) {
                var s = line.Split(':')[1].Split(' ');
                foreach (var seeb in s) {
                    if (!string.IsNullOrWhiteSpace(seeb)) {
                        seebs.Add(long.Parse(seeb));
                    }
                }
            }
            else {
                if (!string.IsNullOrWhiteSpace(line)) {
                    if (!char.IsNumber(line[0])) {
                        level = new Level() {
                            Name = line
                        };
                        levels.Add(level);
                    }
                    else {
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

        //1. we start from the seed ranges as source ranges
        //2. iterate on levels
        //  a. check which ranges of the level intersect the source ranges
        //  b. if there are more than 1 ranges intersecting the source range, split the source range
        //  c. the next level will process the newly split source ranges

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

        Console.WriteLine($"Result: {result}");
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

    private class SeedRange() {
        public long Start;
        public long End;
        public bool InRange(long val) { return val >= Start && val <= End; }
        public static List<SeedRange> GetSeeds(List<long> seebs) {
            var result = new List<SeedRange>();
            for (int s = 0; s <= seebs.Count / 2; s += 2) {
                result.Add(new SeedRange() { Start = seebs[s], End = seebs[s] + seebs[s + 1] });
            }
            return result;
        }
    }
}
