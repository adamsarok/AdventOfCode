using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day10 {
	public class Day10 : Solver {
		public Day10() : base(2024, 10) {
		}
		int[,] input;
		int height;
		int width;
		protected override void ReadInputPart1(string fileName) {
			var file = File.ReadAllLines(fileName);
			height = file.Length;
			width = file[0].Length;
			input = new int[height, width];
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					var s = file[y][x].ToString();
					if (s == ".") input[y, x] = -1;
					else input[y, x] = int.Parse(file[y][x].ToString());
				}
			}
		}


		protected override long SolvePart1() {
			long result = 0;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (input[y, x] == 0) {
						HashSet<Point> ninesReachable = new HashSet<Point>();
						RunTrail(x, y, 1, ninesReachable);
						result += ninesReachable.Count;
					}
				}
			}
			return result;
		}

		private void RunTrail(int x, int y, int next, HashSet<Point> ninesReachable) {
			if (input[y, x] == 9) {
				ninesReachable.Add(new Point(x, y));
				return;
			}
			RunNextStep(x + 1, y, next, ninesReachable);
			RunNextStep(x - 1, y, next, ninesReachable);
			RunNextStep(x, y + 1, next, ninesReachable);
			RunNextStep(x, y - 1, next, ninesReachable);
		}

		private void RunNextStep(int x, int y, int next, HashSet<Point> ninesReachable) {
			if (x < 0 || x >= width || y < 0 || y >= height) return;
			if (input[y, x] == next) RunTrail(x, y, next + 1, ninesReachable);
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart2() {
			long result = 0;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (input[y, x] == 0) {
						long acc = 0;
						RunTrail2(x, y, 1, ref acc);
						result += acc;
					}
				}
			}
			return result;
		}

		private void RunTrail2(int x, int y, int next, ref long acc) {
			if (input[y, x] == 9) {
				acc++;
				return;
			}
			RunNextStep2(x + 1, y, next, ref acc);
			RunNextStep2(x - 1, y, next, ref acc);
			RunNextStep2(x, y + 1, next, ref acc);
			RunNextStep2(x, y - 1, next, ref acc);
		}

		private void RunNextStep2(int x, int y, int next, ref long acc) {
			if (x < 0 || x >= width || y < 0 || y >= height) return;
			if (input[y, x] == next) RunTrail2(x, y, next + 1, ref acc);
		}

	}
}
		