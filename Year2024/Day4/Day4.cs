using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day4 {
	public class Day4 : Solver {
		List<string> input;
		public Day4() : base(2024, 4) {
		}
		protected override void ReadInputPart1(string fileName) {
			input = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				input.Add(l);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			for (int y = 0; y < input.Count; y++) {
				for (int x = 0; x < input[0].Length; x++) {
					result += FindFrom(x, y);
				}
			}
			return result;
		}

		private long FindFrom(int x, int y) {
			var start = input[y][x];
			long res = 0;
			if (start == 'X') { 
				res += CheckDir(x, y, 0, 1);
				res += CheckDir(x, y, 0, -1);
				res += CheckDir(x, y, 1, 0);
				res += CheckDir(x, y, -1, 0);
				res += CheckDir(x, y, 1, 1);
				res += CheckDir(x, y, 1, -1);
				res += CheckDir(x, y, -1, 1);
				res += CheckDir(x, y, -1, -1);
			}
			return res;
		}

		private long CheckDir(int x, int y, int xDir, int yDir) {
			int xNext = x + xDir, yNext = y + yDir; 
			if (InArr(xNext, yNext) && input[yNext][xNext] == 'M') {
				xNext += xDir; 
				yNext += yDir;
				if (InArr(xNext, yNext) && input[yNext][xNext] == 'A') {
					xNext += xDir;
					yNext += yDir;
					if (InArr(xNext, yNext) && input[yNext][xNext] == 'S') {
						return 1;
					}
				}
			}
			return 0;
		}

		private bool InArr(int x, int y) {
			if (y >= 0 && y < input.Count && x >= 0 && x < input[0].Length) return true;
			return false;
		}

		protected override long SolvePart2() {
			long result = 0;
			for (int y = 0; y < input.Count; y++) {
				for (int x = 0; x < input[0].Length; x++) {
					result += FindFromPart2(x, y);
				}
			}
			return result;
		}

		private long FindFromPart2(int x, int y) {
			var start = input[y][x];
			if (start == 'A') {
				var ul = GetChar(x - 1, y - 1);
				var ur = GetChar(x + 1, y - 1);
				var dl = GetChar(x - 1, y + 1);
				var dr = GetChar(x + 1, y + 1);
				if (((ul == 'S' && dr == 'M') || (ul == 'M' && dr == 'S')) 
					&& ((ur == 'S' && dl == 'M') || (ur == 'M' && dl == 'S'))) {
					return 1;
				}
			}
			return 0;
		}

		private char? GetChar(int x, int y) {
			if (!InArr(x, y)) return null;
			return input[y][x];
		}
	}
}
