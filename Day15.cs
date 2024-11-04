using System;
using System.Diagnostics;

namespace AdventOfCode;

public class Day15 {
    public static void SolvePart1() {
        var inputs = File.ReadAllLines("testinput.txt")[0].Split(',');
        var result = 0;
        Debug.Assert(Hash("HASH") == 52, "Hashing is incorrect");
        foreach (var input in inputs) {
            result += Hash(input);
        }
        Console.WriteLine(result);
    }
    private static int Hash(string input) {
        var result = 0;
        foreach (var c in input) {
            var a = (int)c;
            result += c;
            result *= 17;
            result = result % 256;
        }
        return result;
    }
}
