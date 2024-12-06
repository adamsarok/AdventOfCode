using System;

namespace AdventOfCode;

public class Day8
{
    public static void SolvePart1() {
		Dictionary<string, (string, string)> nodes;
		char[] lr;
		ReadInput(out nodes, out lr);
		string node = "AAA";
		int actLr = 0;
		int result = 0;
		while (node != "ZZZ") {
			//Console.WriteLine(node);
			result++;
			if (actLr >= lr.Length) actLr = 0;
			var next = nodes[node];
			node = lr[actLr++] == 'L' ? next.Item1 : next.Item2;
		}
		Console.WriteLine(result);
	}
    public static void SolvePart2() {
		Dictionary<string, (string, string)> nodes;
		char[] lr;
		ReadInput(out nodes, out lr);
        List<long> stepsToReach = new List<long>();
        foreach (var k in nodes.Keys.Where(x => x.EndsWith("A"))) {
            var s = new Day8Solver(k, nodes, lr);
            stepsToReach.Add(s.CycleLength);
        }

		Console.WriteLine(LCM(stepsToReach.ToArray()));
	}
    static long LCM(long[] numbers)
    {
        return numbers.Aggregate(lcm);
    }
    static long lcm(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }
    static long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }

    private class Day8Solver {
        //seems brute force will not work here..
        //we are going in cycles, so we can identify for each start the number of steps it takes to reach Z 
        //we never go back to A, so we can start counting from the first step after A
        private string node { get; set; } = "";
        Dictionary<string, (string, string)> nodes;
        public int CycleLength { get; private set; }
        public Day8Solver(string node, Dictionary<string, (string, string)> nodes, char[] lr) {
            Console.Write($"{node}:");
            this.node = node;
            this.nodes = nodes;
            int actLr = 0;
            CycleLength = 1;
            while (!MoveNext(lr[actLr] == 'L')) { 
                actLr++;
                CycleLength++;
                if (actLr >= lr.Length) actLr = 0;
            }
            Console.WriteLine($"{CycleLength}");
        }
        private bool MoveNext(bool isLeft) {

			var next = nodes[node];
			node = isLeft ? next.Item1 : next.Item2;
            return node.EndsWith("Z");
        }
    }

	private static void ReadInput(out Dictionary<string, (string, string)> nodes, out char[] lr) {
        var lines = File.ReadAllLines("testinput.txt");
		nodes = new Dictionary<string, (string, string)>();
		lr = lines[0].ToCharArray();
		for (int i = 2; i < lines.Length; i++) {
			var line = lines[i];
			var s = line.Split('=');
			var key = s[0].Trim();
			var refs = s[1].Split(',');
			nodes.Add(key, (
				refs[0].Trim().Substring(1, 3),
				refs[1].Trim().Substring(0, 3)
			));
		}
	}

    public class BureForceSolver {
        private string node { get; set; } = "";
        Dictionary<string, (string, string)> nodes;
        public BureForceSolver(string node, Dictionary<string, (string, string)> nodes) {
            this.node = node;
            this.nodes = nodes;
        }
        public bool MoveNext(bool isLeft) {
            //Console.WriteLine(node);
			var next = nodes[node];
			node = isLeft ? next.Item1 : next.Item2;
            return node.EndsWith("Z");
        }
    }
}
