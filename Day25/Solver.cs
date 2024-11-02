using System;
using System.Diagnostics;

namespace AdventOfCode.Day25;

public class Solver {
    List<HashSet<string>> unions = new List<HashSet<string>>();
    Dictionary<string, List<string>> input;
    HashSet<string> allComponents = new HashSet<string>();
    int maxSubTreeFound = 0;
    int connCount = 0;
    int allComponentsCount = 0;
    public bool Solved => maxSubTreeFound < allComponentsCount;
    public Solver(Dictionary<string, List<string>> input, int allComponentsCount) {
        //Stopwatch sw = Stopwatch.StartNew();
        this.input = input;
        this.allComponentsCount = allComponentsCount;
        BuildUnions();
        connCount = input.Count;
        //maxSubTreeFound = unions.Max(x => x.Count);
        // sw.Stop();
        // Console.WriteLine($"Elapsed {sw.ElapsedMilliseconds} ms, longest subtree: {maxSubTreeFound}");
        if (Solved) Console.WriteLine($"Result: {maxSubTreeFound * (allComponentsCount - maxSubTreeFound)}");
    }
    //47 ms for test input this is now ok?
    private void BuildUnions() {
        unions = new List<HashSet<string>>();
        foreach (var c in input) {
            HashSet<string> union = new HashSet<string>() { c.Key };
            union.UnionWith(c.Value);
            allComponents.UnionWith(union);
            bool intersected = false;
            foreach (var target in unions) {
                if (target.Overlaps(union)) {
                    target.UnionWith(union);
                    intersected = true;
                }
            }
            if (!intersected) unions.Add(union);
        }
        HashSet<string> final = new HashSet<string>(unions[0]);
        bool haveOverlap = false;
        int i = 1;
        do {
            if (i == unions.Count && haveOverlap) {
                i = 1;
                haveOverlap = false;
            }
            if (final.Overlaps(unions[i])) {
                final.UnionWith(unions[i]);
                unions.RemoveAt(i);
                haveOverlap = true;
            }
            else {
                i++;
            }
        } while (final.Count < allComponentsCount && unions.Count > 1 && haveOverlap);
        maxSubTreeFound = final.Count;
    }
}
