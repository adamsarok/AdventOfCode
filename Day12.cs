using System;

namespace AdventOfCode;

public class Day12 {
    public static void SolvePart1() {
        var lines = File.ReadAllLines("testinput.txt");
        foreach (var line in lines) {
            SolveLine(line.Split(' '));
        }
        foreach (var s in solutions) {
            Console.WriteLine(s);
        }
        Console.WriteLine(solutions.Count);
        //TODO: works on test input but fails on real input :(
        //Console.WriteLine(CheckLine("?#?###?###?####", new List<int>() { 1,3,1,6 }));
    }
    private static List<int> GetConditions(string str) {
        List<int> conditions = new List<int>();
        str.Split(',').ToList().ForEach(x => conditions.Add(int.Parse(x.Trim())));
        return conditions;
    }

    private static void SolveLine(string[] inputs) {
        string line = inputs[0];
        var conditions = GetConditions(inputs[1]);
        SolveLine(line, 0, conditions);
    }
    static HashSet<string> solutions = new HashSet<string>();
    private static void SolveLine(string input, int startFrom, List<int> conditions) {
        if (startFrom >= input.Length) return;
        SolveLine(input, startFrom + 1, conditions);
        var next = input;
        bool placed = false;
        for (int i = startFrom; i < input.Length; i++) {
            if (next[i] == '?') {
                var arr = input.ToCharArray();
                arr[i] = '#';
                next = new string(arr);
                placed = true;
                break;
            }
        }
        //if (!placed) return;
        var res = CheckLine(next, conditions);
        //Console.WriteLine($" : {res}");
        switch (res) {
            case Conditions.Satisfied:
                solutions.Add(next);
                return;
            case Conditions.Overflow:
                return;
            case Conditions.Underflow:
                SolveLine(next, startFrom + 1, conditions);
                break;
        }
    }

    private enum Conditions { Satisfied, Overflow, Underflow }
    private static Conditions CheckLine(string line, List<int> conditions) {
        //Console.Write($"Checking:{line}, {string.Join(",", conditions)} ");
        int actCond = 0;
        int actSprings = 0;
        int totalSprings = 0;
        for (int i = 0; i < line.Length; i++) {
            if (line[i] == '#') {
                actSprings++;
                totalSprings++;
            }
            else {
                if (actSprings > 0) {
                    if (actCond < conditions.Count && actSprings < conditions[actCond]) return Conditions.Underflow;
                    actSprings = 0;
                    actCond++;
                }
            }
        }
        if (totalSprings > conditions.Sum()) return Conditions.Overflow;
        if ((actCond < conditions.Count && conditions[actCond] > actSprings) 
            || actCond < conditions.Count - 1) {
            return Conditions.Underflow;
        }
        return Conditions.Satisfied;
    }
}
