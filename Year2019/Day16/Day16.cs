using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day16 {
	public class Day16 : Solver {
		public Day16() : base(2019, 16) {
		}
		int[] input;
		int[][] muls;
		protected override void ReadInputPart1(string fileName) {
			var l = File.ReadAllLines(fileName)[0];
			input = new int[l.Length];
			muls = new int[l.Length][];
			for (int i = 0; i < l.Length; i++) {
				input[i] = (int)(l[i] - '0');
				var p = (Enumerable.Repeat(GetPattern(i), l.Length/((i+1)*4) + 1).SelectMany(x => x)).Skip(1).ToArray();
				muls[i] = p; //this is bad but works
			}
		}
		IEnumerable<int> GetPattern(int depth) {
			foreach (int i in Enumerable.Repeat(0, depth + 1)) {
				yield return i;
			}
			foreach (int i in Enumerable.Repeat(1, depth + 1)) {
				yield return i;
			}
			foreach (int i in Enumerable.Repeat(0, depth + 1)) {
				yield return i;
			}
			foreach (int i in Enumerable.Repeat(-1, depth + 1)) {
				yield return i;
			}
		}

		protected override void ReadInputPart2(string fileName) {
			var l = File.ReadAllLines(fileName)[0];
			int multiply = 10000;
			input = new int[l.Length * multiply];
			muls = new int[l.Length * multiply][];
			var temp = new int[l.Length];
			for (int i = 0; i < l.Length; i++) {
				temp[i] = (int)(l[i] - '0');
			}
			input = Enumerable.Repeat(temp, multiply).SelectMany(x => x).ToArray();
			//for (int i = 0; i < l.Length * multiply; i++) {
			//	var p = (Enumerable.Repeat(GetPattern(i), l.Length * multiply / ((i + 1) * 4) + 1).SelectMany(x => x)).Skip(1).ToArray();
			//	muls[i] = p; //this is bad but works
			//}
		}

		protected override long SolvePart1() {
			long result = 0;
			for (int i = 0; i < 100; i++) {
				var temp = Iterate(input);
				input = temp;
			}
			for (int i = 0; i < 8; i++) result += input[i] * (int)Math.Pow(10, 7 - i);
			return result;
		}

		private int[] Iterate(int[] input) {
			var temp = new int[input.Length];
			for (int i = 0; i < input.Length; i++) {
				long acc = 0;
				for (int j = 0; j < input.Length; j++) {
					acc += input[j] * muls[i][j];
					//Console.WriteLine($"{i}:{j}:->{input[j]}*{muls[i][j]}");
				}
				temp[i] = (int)(Math.Abs(acc) % 10);
				//Console.WriteLine();
			}
			return temp;
		}

		protected override long SolvePart2() {
			return -1;
			long result = 0;
			for (int i = 0; i < 100; i++) {
				var temp = Iterate(input);
				input = temp;
			}
			for (int i = 0; i < 8; i++) result += input[i] * (int)Math.Pow(10, 7 - i); //todo this is not it!
			return result;
		}
	}
}
