using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode.Day25;

public class Main {
    static Dictionary<string, List<string>> connections;
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
                        var s = new Solver(copy, allComponentsCount);
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
