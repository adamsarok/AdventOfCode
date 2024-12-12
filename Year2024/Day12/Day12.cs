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
		char[,] input;
		record Polygon(char label, List<Point> vertices, List<Point> squaresInside) {
			public long Area { get; set; }
			public long Circumference { get; set; }
		}
		List<Polygon> polygons;
		//HashSet<char> ids;
		int height, width;
		protected override void ReadInputPart1(string fileName) {
			var file = File.ReadAllLines(fileName);
			//ids = new HashSet<char>();
			polygons = new List<Polygon>();
			height = file.Length;
			width = file[0].Length;
			input = new char[height, width];
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					var c = file[y][x];
					input[y, x] = c;
					var sq = GetPointsOfSquare(x, y);
					var poly = polygons.FirstOrDefault(p => p.label == c && p.vertices.Any(v => sq.Contains(v)));
					//TODO: OXO example is still not expanding correctly
					if (poly != null) {
						ExpandPoly(y, x, poly);
					} else {
						NewPoly(y, x, c);
					}
				}
			}
		}

		private List<Point> GetPointsOfSquare(int x, int y) {
			return new() { new Point(x, y),
				new Point(x + 1, y),
				new Point(x + 1, y + 1),
				new Point(x, y + 1) };
		}

		private void ExpandPoly(int y, int x, Polygon poly) {
			//when we extend a polygon, generate a list of points for the new square x,y
			//points which intersect the old polygon are removed as those would be inside the new polygon
			var points = GetPointsOfSquare(x, y);

			int adjacencies = poly.squaresInside.Where(sq =>
				sq.x == x - 1 && sq.y == y ||
				sq.x == x + 1 && sq.y == y ||
				sq.x == x && sq.y == y - 1 ||
				sq.x == x && sq.y == y + 1).Count();
			if (adjacencies == 1) poly.Circumference += 2;

			foreach (var point in points) {
				if (poly.vertices.Contains(point)) {
					poly.vertices.Remove(point);
				} else {
					poly.vertices.Add(point);
				}
			}
			poly.squaresInside.Add(new Point(x, y));
		}

		private void NewPoly(int y, int x, char c) {
			var poly = new Polygon(c, GetPointsOfSquare(x, y), new() { new Point(x, y) });
			polygons.Add(poly);
			poly.Circumference = 4;
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
				Console.WriteLine($"{poly1.label} area: {poly1.Area} circumference: {poly1.Circumference}");
			}
			return result;
		}

		//private static int GetAreaShoelace(List<Point> vertices) {
		//	long a = 0;
		//	for (int i = 0; i < vertices.Count; i++) {
		//		Point current = vertices[i];
		//		Point next = vertices[(i + 1) % vertices.Count];
		//		a += (current.x * next.y) - (current.y * next.x);
		//	}
		//	return (int)(Math.Abs(a) / 2.0);
		//}



		//private Polygon CreatePolygon(Point s, char id) {
		//	List<Point> vertices = new List<Point>();
		//	for (int y = 0; y < height; y++) {
		//		for (int x = 0; x < width; x++) {
		//			//y of the input = y of the upper bounds of the polygon

		//		}
		//	}
		//	return new Polygon(id, vertices);
		//}

		//private Point? FindStart(char id) {
		//	for (int y = 0; y < height; y++) {
		//		for (int x = 0; x < width; x++) {
		//			if (input[y, x] == id) {
		//				return new Point(x, y);
		//			}
		//		}
		//	}
		//	return null;
		//}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
		