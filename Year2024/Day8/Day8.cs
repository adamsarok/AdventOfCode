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

namespace Year2024.Day8 {
	public class Day8 : Solver {
		public Day8() : base(2024, 8) {
		}
		record Frequency(char frequency, List<Point> nodes);  
		List<Frequency> input;
		int xMax, yMax;
		List<Point> antiNodes;
		string[] inputFile;
		protected override void ReadInputPart1(string fileName) {
			var p = new  System.Drawing.Point();
			input = new();
			inputFile = File.ReadAllLines(fileName);
			xMax = inputFile[0].Length;
			yMax = inputFile.Length;
			antiNodes = new();
			for (int y = 0; y < yMax; y++) {
				for (int x = 0; x < xMax; x++) {
					var c = inputFile[y][x];
					if (c != '.') {
						var fr = input.FirstOrDefault(x => x.frequency == c);
						var node = new Point(x, y);
						if (fr != null) fr.nodes.Add(node);
						else input.Add(new Frequency(c, new List<Point> { node }));
					}
				}
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			//antinodes can occur at antenna locs! find all UNIQUE antinode locs!
			foreach (var freq in input) {
				for (int i = 0; i < freq.nodes.Count - 1; i++) {
					for (int j = i + 1; j < freq.nodes.Count; j++) {
						GetAntinodes(freq.nodes[i], freq.nodes[j]);
					}
				}
			}
			//Debug();
			return antiNodes.Count;
		}

		private void Debug() {
			for (int y = 0; y < yMax; y++) {
				for (int x = 0; x < xMax; x++) {
					if (antiNodes.Contains(new Point(x, y))) Console.Write("#");
					else Console.Write(inputFile[y][x]);
				}
				Console.WriteLine();
			}
		}

		private void GetAntinodes(Point n1, Point n2) {
			Point a1 = n1 + (n1 - n2);
			Point a2 = n2 + (n2 - n1);
			AddAntinode(a1);
			AddAntinode(a2);
		}
		private bool AddAntinode(Point n) {
			if (IsInBounds(n.x, n.y)) {
				TryAdd(n);
				return true;
			}
			return false;
		}
		private void TryAdd(Point n) {
			if (!antiNodes.Contains(n)) antiNodes.Add(n);
		}
		private bool IsInBounds(int x, int y) {
			if (x >= 0 && x < xMax && y >= 0 && y < yMax) return true;
			return false;
		}
		private void GetAntinodesPart2(Point n1, Point n2) {
			Project(n1, n1 - n2);
			Project(n2, n2 - n1);
		}
		private void Project(Point start, Point direction) {
			Point last = start;
			Point next;
			while (true) {
				next = last + direction;
				if (!IsInBounds(next.x, next.y)) break;
				TryAdd(next);
				last = next;
			}
		}
		protected override long SolvePart2() {
			foreach (var freq in input) {
				foreach (var node in freq.nodes) TryAdd(node);
				for (int i = 0; i < freq.nodes.Count - 1; i++) {
					for (int j = i + 1; j < freq.nodes.Count; j++) {
						GetAntinodesPart2(freq.nodes[i], freq.nodes[j]);
					}
				}
			}
			//Debug();
			return antiNodes.Count;
		}
	}
}
