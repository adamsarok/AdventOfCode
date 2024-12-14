using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2022.Day5 {
	public class Day5 : Solver {
		public Day5() : base(2022, 5) {
		}
		List<Stack<char>> crates;
		List<Rule> rules;
		record Rule(int count, int from, int to);
		protected override void ReadInputPart1(string fileName) {
			crates = new();
			rules = new();
			var file = File.ReadAllLines(fileName);
			int lastCrateRow = 0;
			for (int y = 0; y < file.Length; y++) {
				if (string.IsNullOrWhiteSpace(file[y])) {
					lastCrateRow = y - 1;
					break;
				}
			}
			for (int x = 1; x < file[0].Length; x += 4) {
				var stack = new Stack<char>();
				for (int y = 0; y <= lastCrateRow; y++) {
					if (y == lastCrateRow) {
						crates.Add(new Stack<char>(stack));
						stack = new();
					} else {
						char crate = file[y][x];
						if (crate != 32) stack.Push(crate);
					}
				}
			}

			for (int y = lastCrateRow + 2; y < file.Length; y++) {
				var cnt = int.Parse(file[y].Split("from")[0].Split("move")[1]);
				var from = int.Parse(file[y].Split("from")[1].Split("to")[0]);
				var to = int.Parse(file[y].Split("from")[1].Split("to")[1]);
				rules.Add(new Rule(cnt, from, to));
			}		
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var rule in rules) {
				for (int i = 0; i < rule.count; i++) {
					var c = crates[rule.from - 1].Pop();
					crates[rule.to - 1].Push(c);
				}
			}
			Console.WriteLine();
			foreach (var c in crates) {
				Console.Write(c.Peek());
			}
			Console.WriteLine();
			return 0;
		}

		protected override long SolvePart2() {
			long result = 0;
			foreach (var rule in rules) {
				List<char> k = new();
				for (int i = 0; i < rule.count; i++) {
					var c = crates[rule.from - 1].Pop();
					k.Add(c);
				}
				k.Reverse();
				foreach (var c in k) {
					crates[rule.to - 1].Push(c);
				}
			}
			Console.WriteLine();
			foreach (var c in crates) {
				Console.Write(c.Peek());
			}
			Console.WriteLine();
			return 0;
		}
	}
}
