using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public record Vertex(LVec start, LVec end) {
		public LVec? Intersection(Vertex other) {
			long A1 = end.y - start.y;
			long B1 = start.x - end.x;
			long C1 = A1 * start.x + B1 * start.y;

			long A2 = other.end.y - other.start.y;
			long B2 = other.start.x - other.end.x;
			long C2 = A2 * other.start.x + B2 * other.start.y;

			long delta = A1 * B2 - A2 * B1;
			if (delta == 0) return null;

			long x = (B2 * C1 - B1 * C2) / delta;
			long y = (A1 * C2 - A2 * C1) / delta;

			if (IsBetween(start.x, end.x, x) && IsBetween(start.y, end.y, y) &&
				IsBetween(other.start.x, other.end.x, x) && IsBetween(other.start.y, other.end.y, y)) {
				return new LVec(x, y);
			}

			return null;
		}

		public double Length => Math.Sqrt(Math.Pow(end.x - start.x, 2) + Math.Pow(end.y - start.y, 2));

		private bool IsBetween(long a, long b, long c) {
			return Math.Min(a, b) <= c && c <= Math.Max(a, b);
		}

		public bool Intersects(LVec point) {
			long crossProduct = (point.y - start.y) * (end.x - start.x) - (point.x - start.x) * (end.y - start.y);
			if (crossProduct != 0) return false;
			return IsBetween(start.x, end.x, point.x) && IsBetween(start.y, end.y, point.y);
		}
	}
}
