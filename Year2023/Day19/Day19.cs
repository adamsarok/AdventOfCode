using Helpers;


namespace Year2023;
public class Day19 : IAocSolver {
	public long SolvePart1(string[] input) {
		var s = new SolverPart1(input);
		return s.Solve();
	}
	public long SolvePart2(string[] input) {
		long result = 0;
		var s = new SolverPart2(input);
		return s.Solve();
	}
	public class SolverPart2 : SolverBase {
		public SolverPart2(string[] input) : base(input) { }

		public long Solve() {
			return CountAccepted(new Dictionary<Rule.TargetFields, (long min, long max)> {
					{ Rule.TargetFields.x, (1, 4000) },
					{ Rule.TargetFields.m, (1, 4000) },
					{ Rule.TargetFields.a, (1, 4000) },
					{ Rule.TargetFields.s, (1, 4000) }
				}, "in");
		}

		private long CountAccepted(Dictionary<Rule.TargetFields, (long min, long max)> ranges, string workflowName) {
			if (workflowName == "A") {
				return (ranges[Rule.TargetFields.x].max - ranges[Rule.TargetFields.x].min + 1) *
					   (ranges[Rule.TargetFields.m].max - ranges[Rule.TargetFields.m].min + 1) *
					   (ranges[Rule.TargetFields.a].max - ranges[Rule.TargetFields.a].min + 1) *
					   (ranges[Rule.TargetFields.s].max - ranges[Rule.TargetFields.s].min + 1);
			}
			if (workflowName == "R") {
				return 0;
			}

			long total = 0;
			var currentRanges = new Dictionary<Rule.TargetFields, (long min, long max)>(ranges);

			foreach (var rule in workflows[workflowName]) {
				if (rule.Rel == Rule.Relations.Fallback) {
					total += CountAccepted(currentRanges, rule.NextStep);
				} else {
					var (passingRanges, failingRanges) = SplitRanges(currentRanges, rule);

					if (passingRanges != null) {
						total += CountAccepted(passingRanges, rule.NextStep);
					}

					currentRanges = failingRanges;
				}
			}

			return total;
		}

		private (Dictionary<Rule.TargetFields, (long min, long max)>? passing, Dictionary<Rule.TargetFields, (long min, long max)> failing)
			SplitRanges(Dictionary<Rule.TargetFields, (long min, long max)> ranges, Rule rule) {

			var passingRanges = new Dictionary<Rule.TargetFields, (long min, long max)>(ranges);
			var failingRanges = new Dictionary<Rule.TargetFields, (long min, long max)>(ranges);

			var fieldRange = ranges[rule.TargetField];

			if (rule.Rel == Rule.Relations.GreaterThan) {
				if (fieldRange.max > rule.Value) {
					passingRanges[rule.TargetField] = (Math.Max(fieldRange.min, rule.Value + 1), fieldRange.max);
				} else {
					passingRanges = null;
				}

				if (fieldRange.min <= rule.Value) {
					failingRanges[rule.TargetField] = (fieldRange.min, Math.Min(fieldRange.max, rule.Value));
				} else {
					failingRanges = null;
				}
			} else { 
				if (fieldRange.min < rule.Value) {
					passingRanges[rule.TargetField] = (fieldRange.min, Math.Min(fieldRange.max, rule.Value - 1));
				} else {
					passingRanges = null;
				}

				if (fieldRange.max >= rule.Value) {
					failingRanges[rule.TargetField] = (Math.Max(fieldRange.min, rule.Value), fieldRange.max);
				} else {
					failingRanges = null;
				}
			}

			return (passingRanges, failingRanges);
		}
	}

	public class SolverPart1 : SolverBase {
		public SolverPart1(string[] input) : base(input) { }
		public long Solve() {
			result = 0;
			foreach (var part in parts) {
				CheckPart(part);
			}
			return result;
		}
	}

	public class SolverBase {
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
					break;
				case Results.R:
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

		protected void ReadInput(string[] input) {
			workflows = new Dictionary<string, List<Rule>>();
			parts = new List<Part>();
			bool isWorkflows = true;
			foreach (var l in input) {
				if (string.IsNullOrWhiteSpace(l)) isWorkflows = false;
				else if (isWorkflows) {
					var w = l.Split('{');
					List<Rule> rules = new List<Rule>();
					foreach (var rule in w[1].Substring(0, w[1].Length - 1).Split(',')) {
						rules.Add(Rule.ParseRule(rule));
					}
					string next = rules[0].NextStep;
					if (rules.All(x => x.NextStep == next))
						rules = new List<Rule> { new Rule(Rule.Relations.Fallback, Rule.TargetFields.a, 1, next) };
					workflows.Add(w[0], rules);
				} else {
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

		protected class Rule {
			public string NextStep { get; }
			public Relations Rel { get; }
			public TargetFields TargetField { get; }
			public int Value { get; }

			public Rule(Relations rel, TargetFields targetField, int value, string nextStep) {
				Rel = rel;
				TargetField = targetField;
				Value = value;
				NextStep = nextStep;
			}

			public static Rule ParseRule(string str) {
				if (!str.Contains("<") && !str.Contains(">")) {
					return new Rule(Relations.Fallback, TargetFields.a, 0, str);
				}
				var targetField = Enum.Parse<TargetFields>(str.Substring(0, 1));
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
				if (Rel == Relations.Fallback) return true;
				switch (TargetField) {
					case TargetFields.x:
						return (Rel == Relations.GreaterThan && Value < part.x)
							|| (Rel == Relations.LessThan && Value > part.x);
					case TargetFields.m:
						return (Rel == Relations.GreaterThan && Value < part.m)
							|| (Rel == Relations.LessThan && Value > part.m);
					case TargetFields.a:
						return (Rel == Relations.GreaterThan && Value < part.a)
							|| (Rel == Relations.LessThan && Value > part.a);
					case TargetFields.s:
						return (Rel == Relations.GreaterThan && Value < part.s)
							|| (Rel == Relations.LessThan && Value > part.s);
				}
				throw new Exception("shouldnt happen");
			}
		}

		public SolverBase(string[] input) {
			ReadInput(input);
		}
	}
}
