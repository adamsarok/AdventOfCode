using System;
using System.Reflection.Metadata;

namespace AdventOfCode;

public class Day11 {
    public static void SolvePart1() {
        SolveDist(2);
    }

    public static void SolvePart2() {
        SolveDist(1000000);
    }
    
    private static void SolveDist(int d) {
        var lines = File.ReadAllLines("testinput.txt");
        List<(int, int)> galaxies = new List<(int, int)>();
        List<int> emptyRows = new List<int>();
        List<int> emptyCols = new List<int>();
        for (int y = 0; y < lines.Length; y++) {
            var line = lines[y];
            bool rowEmpty = true;
            for (int x = 0; x < line.Length; x++) {
                if (line[x]=='#') {
                    galaxies.Add((x, y));
                    rowEmpty = false;
                }
            }
            if (rowEmpty) emptyRows.Add(y);
        }
        for (int x = 0; x < lines[0].Length; x++) {
            if (!galaxies.Any(g => g.Item1 == x)) emptyCols.Add(x);
        }

        //logic:
        //1. calc distance
        //2. doubling only means that if we cross an empty row or column, that is counted as 2
        //why does this not for d > 1?
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
                
                int hDist = Math.Abs(g1.Item1 - g2.Item1) + (emptyColsBetween.Count() * (d-1));
                int vDist = Math.Abs(g1.Item2 - g2.Item2) + (emptyRowsBetween.Count() * (d-1));
                int dist = hDist + vDist;
                //Console.WriteLine($"[Galaxy {i + 1}]->[Galaxy {j + 1}]:[{g1.Item1}:{g1.Item2}]->[{g2.Item1}:{g2.Item2}]={dist}");
                result += dist;
            }
        }
        Console.WriteLine($"Empties added: {emptiesAdded}");
        Console.WriteLine(result);
    }
}
