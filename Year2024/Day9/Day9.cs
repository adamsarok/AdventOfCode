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
using static Year2024.Day7.Day7;

namespace Year2024.Day9 {
	public class Day9 : Solver {
		public Day9() : base(2024, 9) {
		}
		char[] input;
		long[] output;
		long[] fileBlockLengths;
		List<long> fileIdsMoved;
		protected override void ReadInputPart1(string fileName) {
			var file = File.ReadAllLines(fileName);
			if (file.Length != 1) throw new InvalidDataException("Invalid input");
			fileBlockLengths = new long[file[0].Length];
			foreach (var l in file) {
				input = l.ToCharArray();
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			CreateOutput();
			bool done = false;
			for (int s = output.Length - 1; s >= 0; s--) {
				for (int d = 0; d <= s; d++) {
					if (output[d] == -1) {
						output[d] = output[s];
						output[s] = -1;
						break;
					}
					if (d == s) done = true;
				}
				if (done) break;
			}
			return GetResult();
		}

		private long GetResult() {
			long result = 0;
			for (long i = 0; i < output.Length; i++) {
				if (output[i] != -1) result += i * long.Parse(output[i].ToString());
			}
			return result;
		}

		private void CreateOutput() {
			List<long> temp = new();
			bool file = true;
			int fileId = 0;
			for (int i = 0; i < input.Length; i++) {
				int l = int.Parse(input[i].ToString());
				fileBlockLengths[fileId] = l;
				for (int j = 0; j < l; j++) {
					if (file) temp.Add(fileId);
					else temp.Add(-1);
				}
				if (file) fileId++;
				file = !file;
			}
			output = temp.ToArray();
		}

		protected override long SolvePart2() {
			fileIdsMoved = new();
			CreateOutput();
			for (int s = output.Length - 1; s >= 0; s--) {
				var source = output[s];
				if (source == -1 || fileIdsMoved.Contains(source)) continue;
				var l = fileBlockLengths[source];
				int d = FindFirstContiguousSpace(l, s);
				if (d != -1) {
					for (int w = d; w < d + l; w++) output[w] = source;
					for (int w = s; w > s - l; w--) {
						output[w] = -1;
					}
					fileIdsMoved.Add(source);
				}
			}
			return GetResult();
		}
		private int FindFirstContiguousSpace(long length, long beforeIndex) {
			int len = 0;
			for (int i = 0; i < beforeIndex; i++) {
				if (output[i] == -1) len++;
				else if (len >= length) {
					return i - len;
				} else len = 0;
			}
			return -1;
		}
	}
}
		