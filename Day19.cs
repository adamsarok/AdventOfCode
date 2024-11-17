using System.Diagnostics;
using System.Runtime.CompilerServices;

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
		var s2 = new SolverPart2("testinput.txt");
		s2.Solve();
	}

	public class SolverPart2(string inputFileName) : SolverBase(inputFileName) {
		record struct Interval(Rule.TargetFields targetField, long from, long to) { }

		//brute force would take 243 years so that is not very feasible
		//build up ranges from all rules, so we have a list of intervals:
		//0<a<123 && 10<x<55 && 555<m<556 && 9<s<15 => A
		//then count the number of values possible in all A intervals
		void SplitIntervals(Rule.Relations rel, int value, ref List<Interval> intervals) {
			Interval? toSplit;
			if (rel == Rule.Relations.GreaterThan) {
				toSplit = intervals.FirstOrDefault(x => x.from < value && x.to > value);
			}
			else {
				toSplit = intervals.FirstOrDefault(x => x.from < value && x.to >= value);
			}

			if (toSplit != null) {
				var val = toSplit.Value;
				intervals.Remove(val);
				Interval n1, n2;
				if (rel == Rule.Relations.GreaterThan) {
					n1 = new Interval(val.targetField, val.from, value);
					n2 = new Interval(val.targetField, value + 1, val.to);
				}
				else {
					n1 = new Interval(val.targetField, val.from, value - 1);
					n2 = new Interval(val.targetField, value, val.to);
				}

				if (n1.from > 0 && n1.from <= 4000 && n1.to > 0 && n1.to <= 4000) intervals.Add(n1);
				if (n2.from > 0 && n2.from <= 4000 && n2.to > 0 && n2.to <= 4000) intervals.Add(n2);
				// Console.WriteLine($"Relation: {rel.ToString()}, Value: {value}");
				// Console.WriteLine($"Removed: {val}");
				// Console.WriteLine($"Split into: {n1}, {n2}");
			}
		}

		public void Solve() {
			ReadInput();
			Dictionary<Rule.TargetFields, List<Interval>> intervals =
				new Dictionary<Rule.TargetFields, List<Interval>>();
			intervals.Add(Rule.TargetFields.a, [new Interval(Rule.TargetFields.a, 1, 4000)]);
			intervals.Add(Rule.TargetFields.x, [new Interval(Rule.TargetFields.x, 1, 4000)]);
			intervals.Add(Rule.TargetFields.m, [new Interval(Rule.TargetFields.m, 1, 4000)]);
			intervals.Add(Rule.TargetFields.s, [new Interval(Rule.TargetFields.s, 1, 4000)]);

			//collect all points in a,x,m,s where a new interval is needed
			//later run the same rule matching as day 1, but only for 1 number in all intervals
			foreach (var wf in workflows) {
				foreach (var r in wf.Value.Where(r => r.Rel != Rule.Relations.Fallback)) {
					var list = intervals[r.TargetField];
					SplitIntervals(r.Rel, r.Value, ref list);
				}
			}

			foreach (var kvp in intervals) {
				Console.WriteLine($"{kvp.Key}: {kvp.Value.Count} intervals");
				// foreach (var a in kvp.Value.OrderBy(x => x.from)) {
				// 	Console.WriteLine(a);
				// }
			}

			foreach (var a in intervals[Rule.TargetFields.a]) {
				foreach (var x in intervals[Rule.TargetFields.x]) {
					foreach (var m in intervals[Rule.TargetFields.m]) {
						foreach (var s in intervals[Rule.TargetFields.s]) {
							var dummy = new Part(x.from, m.from, a.from, s.from);
							var rDummy = TraverseWorkflows(dummy, "in");
							var dummy2 = new Part(x.to, m.to, a.to, s.to);
							var rDummy2 = TraverseWorkflows(dummy2, "in");
							if (rDummy != rDummy2) {
								throw new Exception(
									$"{dummy}={rDummy} does not equal {dummy2}={rDummy2}"); //if the ranges are set correctly, both start & end of range should be the same result either A or R
							}

							//TODO: shortinput works, testinput fails:
							//Part { x = 881, m = 1609, a = 1, s = 2010 }= A does not equal Part { x = 902, m = 1610, a = 98, s = 2039 }= R
							//Part { x = 1251, m = 1609, a = 1, s = 2010 }=A does not equal Part { x = 1269, m = 1610, a = 98, s = 2039 }=R
							if (rDummy == Results.A)
								result += (a.to - a.from + 1) * (m.to - m.from + 1) * (x.to - x.from + 1) *
								          (s.to - s.from + 1);
						}
					}
				}
			}

			Console.WriteLine($"Result: {result}");
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
	}

	public class SolverBase(string inputFileName) {
		protected Dictionary<string, List<Rule>> workflows;
		protected List<Part> parts;
		protected long result;

		protected enum Results {
			A,
			R
		}

		protected void CheckPart(Part part) {
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

					//there are tons of dummy rules eg.: ld{x<1281:R,R} this is always a reject and can be replaced with ld{R}
					string next = rules[0].NextStep;
					if (rules.All(x => x.NextStep == next))
						rules = [new Rule(Rule.Relations.Fallback, Rule.TargetFields.a, 1, next)];

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

		protected record struct Part(long x, long m, long a, long s);

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