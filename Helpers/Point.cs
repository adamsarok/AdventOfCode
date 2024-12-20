using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public record Point(int x, int y) : IComparable<Point> {
		public static Point operator +(Point a, Point b) =>
			new Point(a.x + b.x, a.y + b.y);

		public static Point operator -(Point a, Point b) =>
			new Point(a.x - b.x, a.y - b.y);

		public static bool operator >=(Point a, Point b) =>
			a.x >= b.x && a.y >= b.y;

		public static bool operator <=(Point a, Point b) =>
			a.x <= b.x && a.y <= b.y;
		public static Point operator *(Point a, int v) =>
			new Point(a.x * v, a.y * v);

		public int CompareTo(Point? other) {
			if (other == null) return 1;
			if (x == other.x && y == other.y) return 0;
			return (x * x + y * y).CompareTo(other.x * other.x + other.y * other.y);
		}
	}
}
