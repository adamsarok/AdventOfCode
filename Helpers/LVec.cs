using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public record LVec(long x, long y) {
		public static LVec operator +(LVec a, LVec b) =>
			new LVec(a.x + b.x, a.y + b.y);

		public static LVec operator -(LVec a, LVec b) =>
			new LVec(a.x - b.x, a.y - b.y);

		public static bool operator >=(LVec a, LVec b) =>
			a.x >= b.x && a.y >= b.y;

		public static bool operator <=(LVec a, LVec b) =>
			a.x <= b.x && a.y <= b.y;

		public static LVec operator *(LVec a, long v) =>
			new LVec(a.x * v, a.y * v);

		public long ManhattanDistance(LVec b) =>
			Math.Abs(x - b.x) + Math.Abs(y - b.y);

		public int CompareTo(LVec? other) {
			if (other == null) return 1;
			if (x == other.x && y == other.y) return 0;
			return (x * x + y * y).CompareTo(other.x * other.x + other.y * other.y);
		}

		public LVec RotateLeft() { //this only works for (0,1) up,left,right,down
			if (y != 0) return new LVec(y, x);
			if (x != 0) return new LVec(y, -x);
			return new LVec(0, 0);
		}

		public LVec RotateRight() { //this only works for (0,1) up,left,right,down
			if (y != 0) return new LVec(-y, x);
			if (x != 0) return new LVec(y, x);
			return new LVec(0, 0);
		}
	}
}
