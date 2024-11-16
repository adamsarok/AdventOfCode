namespace AdventOfCode;

public class Day19 {
	public static void SolvePart1() {
		var s = new SolverPart1("shortinput.txt");
		s.Solve();
		var s2 = new SolverPart1("testinput.txt");
		s2.Solve();
	}

	public static void SolvePart2() {
		var s = new SolverPart2("shortinput.txt");
		s.Solve();
		// var s2 = new Solver("testinput.txt");
		// s2.Solve();
	}

	public class SolverPart2(string inputFileName) : SolverBase(inputFileName) {
		//TODO
		record struct Interval(
			int aFrom,
			int aTo,
			int xFrom,
			int xTo,
			int mFrom,
			int mTo,
			int sFrom,
			int sTo,
			string result) { }

		private List<Interval> intervals;
		private Queue<Interval> intervalsToProcess;

		//brute force would take 243 years so that is not very feasible
		//build up ranges from all rules, so we have a list of intervals:
		//0<a<123 && 10<x<55 && 555<m<556 && 9<s<15 => A
		//then count the number of values possible in all A intervals
		public void Solve() {
			ReadInput();
			intervals = new List<Interval>();
			intervalsToProcess = new Queue<Interval>();
			long result = 0;
			var i = new Interval(1, 4000, 1, 4000, 1, 4000, 1, 4000, "in");
			intervalsToProcess.Enqueue(i);
			Interval next;
			while (intervalsToProcess.TryDequeue(out next)) {
				if (next.result == "A") {
					result += (next.aTo - next.aFrom) * (next.xTo - next.xFrom) * (next.mTo - next.mFrom) *
					          (next.sTo - next.sFrom);
				} else if (next.result != "R") {
					SplitInterval(next);
				}
			}
		}

		private void SplitInterval(Interval i) {
			var rules = workflows[i.result];

			//1. check incoming interval (example 50<a<100 ...)
			//2. if rule[0] encompasses interval, set result and return (a < 1000)
			//3. if rule[0] intersects interval, split and set result for intersect (a < 75)
			//4. for input + split intervals, we check the next rule if rule[0] did not set result
			foreach (var rule in rules) {
				if (rule.Rel == Rule.Relations.Fallback) {
					intervalsToProcess.Enqueue(new Interval(i.aFrom, i.aTo, i.xFrom, i.xTo, i.mFrom, i.mTo, i.sFrom, i.sTo, rule.NextStep));
					return;
				}
				// switch (rule.TargetField) {
				// 	case Rule.TargetFields.x:
				// 		if (rule.Rel == Rule.Relations.GreaterThan) {
				// 			if (i.xFrom < rule.Value) {
				// 				//TODO: this is not correct, I need to go to the next rule for the split interval and not process the matching interval
				// 				// intervalsToProcess.Enqueue(new Interval(i.aFrom, i.aTo, i.xFrom, rule.Value, i.mFrom, i.mTo, i.sFrom, i.sTo, rule.NextStep));
				// 				// intervalsToProcess.Enqueue(new Interval(i.aFrom, i.aTo, i.xFrom, i.xTo, i.mFrom, i.mTo, i.sFrom, i.sTo, rule.NextStep));
				// 				// interval.aFrom = rule.Value;
				// 				// var split = new Interval()
				// 			}
				// 		}
				// 	case Rule.TargetFields.m:
				// 		return (rel == Rule.Relations.GreaterThan && value < part.m)
				// 		       || (rel == Rule.Relations.LessThan && value > part.m);
				// 	case Rule.TargetFields.a:
				// 		return (rel == Rule.Relations.GreaterThan && value < part.a)
				// 		       || (rel == Rule.Relations.LessThan && value > part.a);
				// 	case Rule.TargetFields.s:
				// 		return (rel == Rule.Relations.GreaterThan && value < part.s)
				// 		       || (rel == Rule.Relations.LessThan && value > part.s);
				// }
			}
			
			//throw new Exception("shouldn't happen");
		}

		class RulePart2(Rule.Relations rel, Rule.TargetFields targetField, int value, string nextStep) {
			
		}
	}

	public class SolverPart1(string inputFileName) : SolverBase(inputFileName) {
		public void Solve() {
			ReadInput();
			result = 0;
			foreach (var part in parts) {
				CheckPart(part);
			}

			Console.WriteLine(result);
		}

		private void CheckPart(Part part) {
			var r = TraverseWorkflows(part, "in");
			switch (r) {
				case Results.A:
					result += part.a + part.m + part.x + part.s;
					//Console.WriteLine($"part {part} is accepted");
					break;
				case Results.R:
					//Console.WriteLine($"part {part} is rejected");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public class SolverBase(string inputFileName) {
		protected Dictionary<string, List<Rule>> workflows;
		protected List<Part> parts;
		protected long result;

		protected enum Results {
			A,
			R
		}

		protected Results TraverseWorkflows(Part part, string workflowKey) {
			var rules = workflows[workflowKey];
			foreach (var rule in rules) {
				if (rule.Match(part)) {
					Results r;
					if (Enum.TryParse(rule.NextStep, out r)) return r;
					return TraverseWorkflows(part, rule.NextStep);
				}
			}

			throw new Exception("shouldn't happen");
		}


		protected void ReadInput() {
			var lines = File.ReadAllLines(inputFileName);
			workflows = new Dictionary<string, List<Rule>>();
			parts = new();
			bool isWorkflows = true;
			foreach (var l in lines) {
				if (string.IsNullOrWhiteSpace(l)) isWorkflows = false;
				else if (isWorkflows) {
					var w = l.Split('{');
					List<Rule> rules = new List<Rule>();
					foreach (var rule in w[1].Substring(0, w[1].Length - 1).Split(',')) {
						rules.Add(Rule.ParseRule(rule));
					}

					workflows.Add(w[0], rules);
				}
				else {
					var p = l.Substring(1, l.Length - 2).Split(',');
					var x = int.Parse(p[0].Split('=')[1]);
					var m = int.Parse(p[1].Split('=')[1]);
					var a = int.Parse(p[2].Split('=')[1]);
					var s = int.Parse(p[3].Split('=')[1]);
					parts.Add(new Part(x, m, a, s));
				}
			}
		}

		protected record struct Part(int x, int m, int a, int s);

		protected class Rule(Rule.Relations rel, Rule.TargetFields targetField, int value, string nextStep) {
			public string NextStep => nextStep;
			public Relations Rel => rel;
			public TargetFields TargetField => targetField;
			public int Value => value;

			public static Rule ParseRule(string str) {
				if (!str.Contains("<") && !str.Contains(">")) {
					return new Rule(Relations.Fallback, TargetFields.a, 0, str);
				}

				var targetField = Enum.Parse<Rule.TargetFields>(str.Substring(0, 1));
				var relation = str.Substring(1, 1) == "<" ? Relations.LessThan : Relations.GreaterThan;
				var value = str.Split(':')[0].Substring(2);
				var nextStep = str.Split(':')[1];
				return new Rule(relation, targetField, int.Parse(value), nextStep);
			}

			public enum Relations {
				GreaterThan,
				LessThan,
				Fallback
			}

			public enum TargetFields {
				x,
				m,
				a,
				s
			}

			public bool Match(Part part) {
				if (rel == Relations.Fallback) return true;
				switch (targetField) {
					case TargetFields.x:
						return (rel == Rule.Relations.GreaterThan && value < part.x)
						       || (rel == Rule.Relations.LessThan && value > part.x);
					case TargetFields.m:
						return (rel == Rule.Relations.GreaterThan && value < part.m)
						       || (rel == Rule.Relations.LessThan && value > part.m);
					case TargetFields.a:
						return (rel == Rule.Relations.GreaterThan && value < part.a)
						       || (rel == Rule.Relations.LessThan && value > part.a);
					case TargetFields.s:
						return (rel == Rule.Relations.GreaterThan && value < part.s)
						       || (rel == Rule.Relations.LessThan && value > part.s);
				}

				throw new Exception("shouldnt happen");
			}
		}
	}
}