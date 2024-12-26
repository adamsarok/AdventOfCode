using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public record Vec(int x, int y) : IComparable<Vec> {
		public static Vec operator +(Vec a, Vec b) =>
			new Vec(a.x + b.x, a.y + b.y);

		public static Vec operator -(Vec a, Vec b) =>
			new Vec(a.x - b.x, a.y - b.y);

		public static bool operator >=(Vec a, Vec b) =>
			a.x >= b.x && a.y >= b.y;

		public static bool operator <=(Vec a, Vec b) =>
			a.x <= b.x && a.y <= b.y;
		public static Vec operator *(Vec a, int v) =>
			new Vec(a.x * v, a.y * v);

		public int ManhattanDistance(Vec b) =>
			Math.Abs(x - b.x) + Math.Abs(y - b.y);

		public int CompareTo(Vec? other) {
			if (other == null) return 1;
			if (x == other.x && y == other.y) return 0;
			return (x * x + y * y).CompareTo(other.x * other.x + other.y * other.y);
		}
	}
}
