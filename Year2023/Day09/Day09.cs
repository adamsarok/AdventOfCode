using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023.Day09 {
	public class Day09 : Solver {
		public Day09() : base(2023, 9) {
		}
		protected override void ReadInputPart1(string fileName) {
			//input = new();
			foreach (var l in File.ReadAllLines(fileName)) {

			}
		}

		protected override void ReadInputPart2(string fileName) {
			//input = new();
			foreach (var l in File.ReadAllLines(fileName)) {

			}
		}

		protected override long SolvePart1() {
			var inputs = ReadInput();
			int result = 0;
			foreach (var input in inputs) {
				var solver = new Day9Solver(input);
				result += solver.Result;
			}

			return result;
		}

		protected override long SolvePart2() {
			var inputs = ReadInput();
			int result = 0;
			foreach (var input in inputs) {
				var s2 = new SolverBackwards(input);
				result += s2.Result;
			}
			return result;
		}

		class Day9Solver {
			private List<int> input;
			public int Result { get; private set; }
			public Day9Solver(List<int> input) {
				this.input = input;
				List<List<int>> levels = new List<List<int>>();
				List<int> actLevel = input;
				while (actLevel.Any(x => x != 0)) {
					Console.WriteLine(string.Join(' ', actLevel));
					var l = SolveOneLevel(actLevel);
					levels.Add(l);
					actLevel = l;
				}
				int lastVal = 0;
				levels.Reverse();
				levels.Add(input);
				foreach (var l in levels) {
					lastVal = l.Last() + lastVal;
				}
				Result = lastVal;
				Console.WriteLine(Result);
			}
			private List<int> SolveOneLevel(List<int> level) {
				List<int> result = new List<int>();
				for (int i = 0; i < level.Count - 1; i++) {
					result.Add(level[i + 1] - level[i]);
				}
				return result;
			}
		}
		class SolverBackwards {
			private List<int> input;
			public int Result { get; private set; }
			public SolverBackwards(List<int> input) {
				this.input = input;
				List<List<int>> levels = new List<List<int>>();
				List<int> actLevel = input;
				while (actLevel.Any(x => x != 0)) {
					Console.WriteLine(string.Join(' ', actLevel));
					var l = SolveOneLevel(actLevel);
					levels.Add(l);
					actLevel = l;
				}
				int firstVal = 0;
				levels.Reverse();
				levels.Add(input);
				foreach (var l in levels) {
					firstVal = l.First() - firstVal;
				}
				Result = firstVal;
				Console.WriteLine(Result);
			}
			private List<int> SolveOneLevel(List<int> level) {
				List<int> result = new List<int>();
				for (int i = level.Count - 1; i > 0; i--) {
					result.Add(level[i] - level[i - 1]);
				}
				result.Reverse();
				return result;
			}
		}
		private static List<List<int>> ReadInput() {
			var lines = File.ReadAllLines("testinput.txt");
			List<List<int>> result = new List<List<int>>();
			foreach (var line in lines) {
				var r = new List<int>();
				result.Add(r);
				foreach (var c in line.Split(' ')) {
					r.Add(int.Parse(c));
				}
			}
			return result;
		}
	}
}
