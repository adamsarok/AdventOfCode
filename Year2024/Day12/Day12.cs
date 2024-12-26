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
		HashSet<Vec> processed;
		record Polygon(char label, HashSet<Vec> squaresInside) {
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
			processed = new HashSet<Vec>();
			height = input.Length;
			width = input[0].Length;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					var p = new Vec(x, y);
					if (processed.Contains(p)) continue;
					var poly = new Polygon(input[y][x],
						new()
					);
					FloodFill(p, poly);
					polygons.Add(poly);
				}
			}
		}

		private void FloodFill(Vec p, Polygon poly) {
			if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return;
			if (poly.squaresInside.Contains(p) || processed.Contains(p)) return;
			if (input[p.y][p.x] != poly.label) return;
			poly.squaresInside.Add(p);
			processed.Add(p);
			FloodFill(new Vec(p.x + 1, p.y), poly);
			FloodFill(new Vec(p.x - 1, p.y), poly);
			FloodFill(new Vec(p.x, p.y + 1), poly);
			FloodFill(new Vec(p.x, p.y - 1), poly);
		}

		protected override long SolvePart1() {
			long result = 0;
			//1. build polygons from input
			//2. calculate area don't even need shoelace just count squares inside
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

		/// <summary>
		/// XO
		/// OO - 1 vertex
		/// 
		/// XX XO
		/// OO XO - 0 vertex
		/// 
		/// XO
		/// OX - 1 vertex or 2 vertex if X-es are connected
		/// 
		/// XX
		/// XO - 1 vertex
		/// 
		/// XX
		/// XX - 0 vertex
		/// 
		/// </summary>
		private void CountVertices() {
			//if we have 1 cell of a poly -> 1 vertex
			//if we have 2 cells of a poly -> vertical or horizontal = 0 vertex, diagonal = 2 vertices
			//if we have 3 cells of a poly -> 1 vertex
			//if we have 4 cells of a poly -> 0 vertex
			for (int y = 0; y <= input.Length; y++) {
				for (int x = 0; x <= input.Length; x++) {
					var ul = GetPolyContaining(x - 1, y - 1);
					var ur = GetPolyContaining(x, y - 1);
					var dl = GetPolyContaining(x - 1, y);
					var dr = GetPolyContaining(x, y);
					var all = new[] { ul, ur, dl, dr };
					var groups = all.GroupBy(p => p)
						 .Select(g => new { Polygon = g.Key, Count = g.Count() })
						 .ToList();
					if (groups.Count == 1) continue;				//4 cells of a poly -> 0 vertex
					foreach (var group in groups) {
						if (group.Count == 1 || group.Count == 3) { //1 or 3 cells of a poly -> 1 vertex
							group.Polygon.VerticesCount++;
						}
					}
					if (ul != ur && dl != dr) {                     
						if (ul == dr) {
							ul.VerticesCount += 2;                  //diagonal -> 2 vertices
						}
						if (ur == dl) {
							ur.VerticesCount += 2;
						}
					} 
				}
			}
		}
		Polygon dummy = new Polygon('#', new HashSet<Vec>());
		private Polygon GetPolyContaining(int x, int y) {
			if (x < 0 || x >= width || y < 0 || y >= height) return dummy;
			//TODO: would be faster by storing point -> polygon dictionary
			return polygons.First(p => p.squaresInside.Contains(new Vec(x, y)));
		}

		protected override long SolvePart2() {
			long result = 0;
			foreach (var poly in polygons) {
				poly.Area = poly.squaresInside.Count;
			}
			CountVertices();
			for (int p1 = 0; p1 < polygons.Count; p1++) {
				var poly1 = polygons[p1];
				for (int p2 = 0; p2 < polygons.Count; p2++) {
					if (p1 == p2) continue;
					var poly2 = polygons[p2];
					foreach (var sq in poly1.squaresInside.Where(x => poly2.squaresInside.Contains(x))) {
						poly1.Area--;
					}
				}
				result += poly1.Area * poly1.VerticesCount;
				//Console.WriteLine($"{poly1.label} area: {poly1.Area} sides: {poly1.VerticesCount}");
			}
			return result;
		}
	}
}