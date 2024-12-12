using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day12 {
	public class Day12 : Solver {
		public Day12() : base(2024, 12) {
		}
		string[] input;
		HashSet<Point> processed;
		record Polygon(char label, HashSet<Point> squaresInside) {
			public long Area { get; set; }
			public long Circumference {
				get {
					long circumference = 0;
					foreach (var sq in squaresInside) {
						int adjacencies = squaresInside.Where(adj =>
							adj.x == sq.x - 1 && adj.y == sq.y ||
							adj.x == sq.x + 1 && adj.y == sq.y ||
							adj.x == sq.x && adj.y == sq.y - 1 ||
							adj.x == sq.x && adj.y == sq.y + 1).Count();
						circumference += 4 - adjacencies;
					}
					return circumference;
				}
			}
			public long VerticesCount { get; set; }
		}

		List<Polygon> polygons;

		int height, width;
		protected override void ReadInputPart1(string fileName) {
			input = File.ReadAllLines(fileName);
			polygons = new List<Polygon>();
			processed = new HashSet<Point>();
			height = input.Length;
			width = input[0].Length;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					var p = new Point(x, y);
					if (processed.Contains(p)) continue;
					var poly = new Polygon(input[y][x],
						new()
					);
					FloodFill(p, poly);
					polygons.Add(poly);
				}
			}
		}

		private void FloodFill(Point p, Polygon poly) {
			if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return;
			if (poly.squaresInside.Contains(p) || processed.Contains(p)) return;
			if (input[p.y][p.x] != poly.label) return;
			poly.squaresInside.Add(p);
			processed.Add(p);
			FloodFill(new Point(p.x + 1, p.y), poly);
			FloodFill(new Point(p.x - 1, p.y), poly);
			FloodFill(new Point(p.x, p.y + 1), poly);
			FloodFill(new Point(p.x, p.y - 1), poly);
		}

		protected override long SolvePart1() {
			long result = 0;
			//1. build polygons from input
			//2. calculate area and circum with shoelace - we don't even need shoelace just count square inside
			//3. remove areas of polygons that are inside other polygons
			foreach (var poly in polygons) {
				poly.Area = poly.squaresInside.Count;
			}
			for (int p1 = 0; p1 < polygons.Count; p1++) {
				var poly1 = polygons[p1];
				for (int p2 = 0; p2 < polygons.Count; p2++) {
					if (p1 == p2) continue;
					var poly2 = polygons[p2];
					foreach (var sq in poly1.squaresInside.Where(x => poly2.squaresInside.Contains(x))) {
						poly1.Area--;
					}
				}
				result += poly1.Area * poly1.Circumference;
				//Console.WriteLine($"{poly1.label} area: {poly1.Area} circumference: {poly1.Circumference}");
			}
			return result;
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart2() {
			long result = 0;
			foreach (var poly in polygons) {
				poly.Area = poly.squaresInside.Count;
			}
			for (int p1 = 0; p1 < polygons.Count; p1++) {
				var poly1 = polygons[p1];
				for (int p2 = 0; p2 < polygons.Count; p2++) {
					if (p1 == p2) continue;
					var poly2 = polygons[p2];
					foreach (var sq in poly1.squaresInside.Where(x => poly2.squaresInside.Contains(x))) {
						poly1.Area--;
					}
				}
				result += poly1.Area * poly1.VerticesCount; //TODO: count vertices
				//Console.WriteLine($"{poly1.label} area: {poly1.Area} sides: {poly1.VerticesCount}");
			}
			return result;
		}
	}
}