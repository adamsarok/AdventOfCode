using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Year2024.Day3 {
	public class Day3 : Solver {
		string input;
		public Day3() : base(2024, 3) {
		}
		protected override void ReadInputPart1(string fileName) {
			input = "";
			foreach (var l in File.ReadAllLines(fileName)) {
				input += l;
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			MatchCollection matches = Regex.Matches(input, "mul\\(");	
			foreach (Match match in matches) {
				int i = match.Index + 4;
				string n1 = "", n2 = null;
				while (i < input.Length) {
					char next = input[i++];
					if (char.IsDigit(next)) {
						if (n2 == null) n1 += next;
						else n2 += next;
					} else if (next == ',') {
						if (n2 == null) n2 = "";
						else break;
					} else if (next == ')') {
						if (!string.IsNullOrWhiteSpace(n1) && !string.IsNullOrWhiteSpace(n2)) {
							//Console.WriteLine($"{input.Substring(match.Index, i - match.Index)} - {n1}*{n2}");
							result += long.Parse(n1) * long.Parse(n2);
						}
						break;
					} else {
						break;
					}
				}
			}
			
			
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;
			MatchCollection dos = Regex.Matches(input, "do\\(\\)");
			MatchCollection donts = Regex.Matches(input, "don't\\(\\)");
			List<(int, int)> doRanges = new List<(int, int)> ();
			var dont1 = donts.Where(x => x.Index > 0).FirstOrDefault();
			doRanges.Add((0, dont1 != null ? dont1.Index : int.MaxValue));
			for (int i = 0; i < dos.Count; i++) {
				Match do1 = dos[i];
				var dont = donts.Where(x => x.Index > do1.Index).FirstOrDefault();
				doRanges.Add((do1.Index, dont != null ? dont.Index : int.MaxValue));
			}
			MatchCollection matches = Regex.Matches(input, "mul\\(");
			foreach (Match match in matches) {
				int i = match.Index + 4;
				var doRange = doRanges.FirstOrDefault(x => x.Item1 < i && x.Item2 > i);
				if (doRange.Item2 <= 0) continue;
				string n1 = "", n2 = null;
				while (i < input.Length) {
					char next = input[i++];
					if (char.IsDigit(next)) {
						if (n2 == null) n1 += next;
						else n2 += next;
					} else if (next == ',') {
						if (n2 == null) n2 = "";
						else break;
					} else if (next == ')') {
						if (!string.IsNullOrWhiteSpace(n1) && !string.IsNullOrWhiteSpace(n2)) {
							//Console.WriteLine($"{input.Substring(match.Index, i - match.Index)} - {n1}*{n2}");
							result += long.Parse(n1) * long.Parse(n2);
						}
						break;
					} else {
						break;
					}
				}
			}
			return result;
		}
	}
}
