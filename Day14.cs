using System;
using System.Diagnostics;

namespace AdventOfCode;

public class Day14 {
    //this is such a cool task!
    const char ROLLING = 'O';
    const char EMPTY = '.';
    const char CUBE = '#';
    public enum Directions { N, E, S, W }
    public static void SolvePart1() {
		List<char[]> lines = ReadInput();
		Roll(lines, Directions.N);
		Console.WriteLine("After:");
		WriteState(lines);
		PrintResult(lines);
	}

	private static void PrintResult(List<char[]> lines) {
		int result = 0;
		int mul = lines.Count;
		foreach (var l in lines) {
			result += mul * (l.Where(x => x == ROLLING).Count());
			mul--;
		}
		Console.WriteLine($"Load: {result}");
	}

	public static void SolvePart2() {
        //N W S E
        //1000000000 cycles
        //TODO: this wayyyyyyyyyyyyyyyy too slow
      	List<char[]> lines = ReadInput();
        Stopwatch sw = Stopwatch.StartNew();
        long maxIter = 1000000000;
        long step = maxIter / 100;
        for (long i = 0; i < maxIter; i++) {
            if (i % (step) == 0) { 
                Console.WriteLine($"{Math.Round((float)i / (float)maxIter * 100)} %");
            }
            Roll(lines, Directions.N);
            Roll(lines, Directions.W);
            Roll(lines, Directions.S);
            Roll(lines, Directions.E);
        }
        sw.Stop();
		Console.WriteLine($"Rolled in: {sw.ElapsedMilliseconds} ms");
		WriteState(lines);
		PrintResult(lines);
    }

    //326 ms
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

    private static List<char[]> ReadInput() {
        var lines = new List<char[]>();
        foreach (var l in File.ReadAllLines("testinput.txt")) {
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
