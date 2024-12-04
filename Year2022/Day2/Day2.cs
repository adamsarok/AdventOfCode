using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2022.Day2 {
	public class Day2 : Solver {
		public Day2() : base(2) {
		}
		List<(string, string)> input;
		protected override void ReadInputPart1(string fileName) {
			input = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				input.Add((l.Split(' ')[0], l.Split(' ')[1]));
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			//(1 for Rock, 2 for Paper, and 3 for Scissors)
			//outcome of the round (0 if you lost, 3 if the round was a draw, and 6 if you won).
			foreach (var l in input) {
				long sub = 0;
				switch (l.Item2) {
					case "X":
						sub += 1;
						switch (l.Item1) {
							case "A": //rock = rock
								sub += 3;
								break;
							case "B": 
								sub += 0;
								break;
							case "C": //rock beats scissors
								sub += 6;
								break;
						}
						break;
					case "Y":
						sub += 2;
						switch (l.Item1) {
							case "A": 
								sub += 6;
								break;
							case "B": //paper = paper
								sub += 3;
								break;
							case "C": //scissors beat paper
								sub += 0;
								break;
						}
						break;
					case "Z":
						sub += 3;
						switch (l.Item1) {
							case "A": //rock beats scissors
								sub += 0;
								break;
							case "B": //scissors beats paper
								sub += 6;
								break;
							case "C": //paper = paper
								sub += 3;
								break;
						}
						break;
				}
				result += sub;
			}
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;
			foreach (var l in input) {
				long sub = 0;
				switch (l.Item2) {
					case "X":
						sub += 0;
						switch (l.Item1) { 
							case "A": //lose against rock = scissors
								sub += 3;
								break;
							case "B": 
								sub += 1;
								break;
							case "C": 
								sub += 2;
								break;
						}
						break;
					case "Y":
						sub += 3;
						switch (l.Item1) {
							case "A": //rock = rock
								sub += 1;
								break;
							case "B":
								sub += 2;
								break;
							case "C": 
								sub += 3;
								break;
						}
						break;
					case "Z":
						sub += 6;
						switch (l.Item1) {
							case "A": //win against rock = paper
								sub += 2;
								break;
							case "B":
								sub += 3;
								break;
							case "C":
								sub += 1;
								break;
						}
						break;
				}
				result += sub;
			}
			return result;
		}
	}
}
