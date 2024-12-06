using System;
using System.Diagnostics;

namespace AdventOfCode;

public class Day12 {
    public static void SolvePart1() {
        var lines = File.ReadAllLines("testinput.txt");
        int result = 0;
        Stopwatch sw = Stopwatch.StartNew();
        // Parallel.ForEach(lines, line => {
        //     var solver = new Solver(line.Split(' '), false);
        //     result += solver.Solutions.Count;
        // });
        foreach (var line in lines) { //current best is 1590ms 
            var solver = new Day12Solver(line.Split(' '), 5);
            result += solver.Solutions.Count;
        }
        sw.Stop();
        Console.WriteLine($"Part1 elapsed: {sw.ElapsedMilliseconds}");

        //day2 will never finish with brute force approach 
        // Stopwatch sw2 = Stopwatch.StartNew();
        // Parallel.ForEach(lines, line => {
        //     var solver = new Solver(line.Split(' '), true);
        //     result += solver.Solutions.Count;
        // });
        // sw2.Stop();
        // Console.WriteLine($"Part2 elapsed: {sw2.ElapsedMilliseconds}");
        Console.WriteLine(result);
    }
    private class Day12Solver {
        public HashSet<string> Solutions = new HashSet<string>();
        List<int> conditions = new List<int>();
        string line = "";
        public Day12Solver(string[] inputs, int repeat) {
            for (int i = 0; i < repeat; i++) {
                line += inputs[0];
                conditions.AddRange(GetConditions(inputs[1]));
            }
            Solve();
        }
        private List<int> GetConditions(string str) {
            List<int> conditions = new List<int>();
            str.Split(',').ToList().ForEach(x => conditions.Add(int.Parse(x.Trim())));
            return conditions;
        }

        private void Solve() {
            switch (CheckLine(line, conditions)) {
                case Conditions.Underflow:
                    SolveFrom(line, 0, conditions);
                    break;
                case Conditions.Satisfied:
                    Solutions.Add(line); //already complete input
                    break;
            }
        }

        private void SolveFrom(string input, int startFrom, List<int> conditions) {
            if (startFrom >= input.Length) return;
            SolveFrom(input, startFrom + 1, conditions);
            var next = input;
            for (int i = startFrom; i < input.Length; i++) {
                if (next[i] == '?') {
                    var arr = input.ToCharArray();
                    arr[i] = '#';
                    next = new string(arr);
                    break;
                }
            }
            if (checkedAlready.Contains(next)) return;
            var res = CheckLine(next, conditions);
            checkedAlready.Add(next);
            //Console.WriteLine($" : {res}");
            switch (res) {
                case Conditions.Satisfied:
                    //Console.WriteLine(next);
    
                    Solutions.Add(next);
                    return;
                case Conditions.Overflow:
                    return;
                case Conditions.Underflow:
                    SolveFrom(next, startFrom + 1, conditions);
                    break;
            }
        }
        HashSet<string> checkedAlready = new HashSet<string>();

        private enum Conditions { Satisfied, Overflow, Underflow }
        private Conditions CheckLine(string line, List<int> conditions) { //TODO: if we would pass the last counts to this method, we would be done?
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

}
