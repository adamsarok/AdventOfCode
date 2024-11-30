using System;

namespace AdventOfCode;

public class Day6 {
    public static void SolvePart1() {
        var lines = File.ReadAllLines("day6input.txt");
        List<int> times = new List<int>();
        List<int> distances = new List<int>();
        lines[0]
            .Split(':')[1]
            .Split(' ')
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList()
            .ForEach(x => times.Add(int.Parse(x)));
        lines[1]
            .Split(':')[1]
            .Split(' ')
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList()
            .ForEach(x => distances.Add(int.Parse(x)));
        int result = 1;
        for (int i = 0; i < times.Count; i++) {
            Console.WriteLine($"Race:{i}");
            int waysToBeat = 0;
            var maxTime = times[i];
            var distToBeat = distances[i];
            for (int tPressed = 1; tPressed <= maxTime - 1; tPressed++) { //press for at least 1 ms, max totalTime -1 ms
                var distance = tPressed * (maxTime - tPressed);
                if (distance > distToBeat) {
                    Console.WriteLine($"tPressed:{tPressed} tTravel:{maxTime - tPressed} dTraveled:{distance}");
                    waysToBeat++;
                }
            }
            result *= waysToBeat;
        }
        if (result == 1) result = 0;
        Console.WriteLine(result);
    }
    public static void SolvePart2() {
        var lines = File.ReadAllLines("day6input.txt");
        var maxTime = long.Parse(string.Join("", lines[0]
            .Split(':')[1]
            .Split(' ')
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList()));
        var distToBeat = long.Parse(string.Join("", lines[1]
            .Split(':')[1]
            .Split(' ')
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList()));
        int result = 1;
        int waysToBeat = 0;
        for (long tPressed = 1; tPressed <= maxTime - 1; tPressed++) {
            long distance = tPressed * (maxTime - tPressed);
            if (distance > distToBeat) {
                waysToBeat++;
            }
        }
        result *= waysToBeat;
        if (result == 1) result = 0;
        Console.WriteLine(result);
    }
}
