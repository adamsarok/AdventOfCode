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
        List<Solver> solvers = new List<Solver>();
        foreach (var k in nodes.Keys.Where(x => x.EndsWith("A"))) {
            solvers.Add(new Solver(k, nodes));
        }
		int actLr = 0;
		int result = 0;
		while (true) { 
            //seems brute force will not work here..
            bool solved = true;
            foreach (var solver in solvers) { 
                if (!solver.MoveNext(lr[actLr] == 'L')) solved = false;
            }
			result++;
            actLr++;
            if (actLr >= lr.Length) actLr = 0;
            if (solved) break;
		}
		Console.WriteLine(result);
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

    public class Solver {
        private string node { get; set; } = "";
        Dictionary<string, (string, string)> nodes;
        public Solver(string node, Dictionary<string, (string, string)> nodes) {
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
