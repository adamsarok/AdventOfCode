using System;
using System.Diagnostics;

namespace AdventOfCode;

public class Day25 {
    static Dictionary<string, HashSet<string>> connections;
    public static void SolvePart1() {
        connections = new Dictionary<string, HashSet<string>>();
        foreach (var l in File.ReadAllLines("testinput.txt")) {
            var from = l.Split(':')[0];
            var to = l.Split(':')[1].Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet();
            connections.Add(from, to);
        }
        var s = new Solver(connections);
    }
    class Solver {
        Dictionary<string, HashSet<string>> connectionsFromTo;
        int maxSubTreeFound = 0;
        int connCount = 0;
        public Solver(Dictionary<string, HashSet<string>> input) {
            Stopwatch sw = Stopwatch.StartNew();
            connectionsFromTo = new Dictionary<string, HashSet<string>>(input);
            BuildGraph();
            connCount = connectionsFromTo.Count;
            Console.WriteLine(IsOneGraph());
            sw.Stop();
            Console.WriteLine($"Elapsed {sw.ElapsedMilliseconds} ms, max subtrees: {maxSubTreeFound}");
            Console.WriteLine($"Result: {maxSubTreeFound * (connCount - maxSubTreeFound)}");
        }
        //ms
        private void BuildGraph() {
            connectionsFromTo = new Dictionary<string, HashSet<string>>(connections);
            foreach (var c in connections) {
                foreach (var v in c.Value) {
                    HashSet<string> connsBack;
                    if (connectionsFromTo.TryGetValue(v, out connsBack)) {
                        connsBack.Add(c.Key);
                    }
                    else {
                        connectionsFromTo.Add(v, new HashSet<string>() { c.Key });
                    }
                }
            }
        }
        private bool IsOneGraph() {
            var start = connectionsFromTo.Keys.First();
            return Traverse(start, new HashSet<string>() { start });
        }
        private bool Traverse(string act, HashSet<string> traversed) {
            traversed.Add(act);
            maxSubTreeFound = Math.Max(maxSubTreeFound, traversed.Count);
            Console.WriteLine(string.Join(' ', traversed));
            if (maxSubTreeFound == connCount) return true;
            HashSet<string> conns;
            if (!connectionsFromTo.TryGetValue(act, out conns)) return false;
            foreach (var next in conns) {
                if (!traversed.Contains(next)) {
                    if (Traverse(next, new HashSet<string>(traversed))) return true;
                }
            }
            return false;
        }
    }

}
