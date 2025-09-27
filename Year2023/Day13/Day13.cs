using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023 {
	public class Day13 : IAocSolver {
		public long SolvePart1(string[] input) {
			long result = 0;
			List<string> act = new List<string>();
			result = 0;
			foreach (var line in input) {
				if (string.IsNullOrWhiteSpace(line)) {
					result += ProcessOne(act);
					act = new List<string>();
				} else {
					act.Add(line);
				}
			}
			result += ProcessOne(act);
			return result;
		}

		public long SolvePart2(string[] input) {
			long result = 0;
			List<string> act = new List<string>();
			result = 0;
			foreach (var line in input) {
				if (string.IsNullOrWhiteSpace(line)) {
					result += ProcessPart2(act);
					act = new List<string>();
				} else {
					act.Add(line);
				}
			}
			result += ProcessPart2(act);
			return result;
		}

		private static int ProcessPart2(List<string> act) {
			int result = FindHorizontal(act) * 100;
			if (result == 0) result = FindVertical(act);
			return Smudge(act, result);
		}
		private static int Smudge(List<string> input, int currentValue) {
			for (int y = 0; y < input.Count; y++) {
				for (int x = 0; x < input[0].Length; x++) {
					var act = new List<string>(input);
					var arr = act[y].ToCharArray();
					arr[x] = (arr[x] == '.' ? '#' : '.');
					act[y] = string.Join("", arr);
					List<int> result = FindHorizontals(act);
					var diff = result.FirstOrDefault(x => x != currentValue / 100);
					if (diff > 0) return diff * 100;
					result = FindVerticals(act);
					diff = result.FirstOrDefault(x => x != currentValue);
					if (diff > 0) return diff;
				}
			}
			return 0;
		}
		private static List<int> FindHorizontals(List<string> act) {
			List<int> result = new List<int>();
			for (int y = 1; y < act.Count; y++) {
				//y = count of lines above symmetry line
				var yAbove = y - 1;
				var yBelow = y;
				bool symm = true;
				while (yAbove >= 0 && yBelow < act.Count) {
					if (act[yAbove] != act[yBelow]) {
						symm = false;
						break;
					}
					yAbove--;
					yBelow++;
				}
				if (symm) result.Add(y);
			}
			return result;
		}
		private static List<int> FindVerticals(List<string> act) {
			//easier to transpose & call horizontal
			List<string> transposed = Enumerable.Repeat("", act[0].Length).ToList();
			for (int y = 0; y < act.Count; y++) {
				var line = act[y];
				for (int x = 0; x < line.Length; x++) {
					transposed[x] += line[x];
				}
			}
			return FindHorizontals(transposed);
		}
		private static int ProcessOne(List<string> act) {
			int result = FindHorizontal(act) * 100;
			if (result > 0) return result;
			return FindVertical(act);
		}
		private static int FindHorizontal(List<string> act) {
			for (int y = 1; y < act.Count; y++) {
				//y = count of lines above symmetry line
				var yAbove = y - 1;
				var yBelow = y;
				bool symm = true;
				while (yAbove >= 0 && yBelow < act.Count) {
					if (act[yAbove] != act[yBelow]) {
						symm = false;
						break;
					}
					yAbove--;
					yBelow++;
				}
				if (symm) return y;
			}
			return 0;
		}
		private static int FindVertical(List<string> act) {
			//easier to transpose & call horizontal
			List<string> transposed = Enumerable.Repeat("", act[0].Length).ToList();
			for (int y = 0; y < act.Count; y++) {
				var line = act[y];
				for (int x = 0; x < line.Length; x++) {
					transposed[x] += line[x];
				}
			}
			return FindHorizontal(transposed);
		}
	}
}
