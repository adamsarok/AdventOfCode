using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public record LVec3D(long x, long y, long z) {
		public static LVec3D operator +(LVec3D a, LVec3D b) =>
			new LVec3D(a.x + b.x, a.y + b.y, a.z + b.z);

		public static LVec3D operator -(LVec3D a, LVec3D b) =>
			new LVec3D(a.x - b.x, a.y - b.y, a.z - b.z);

		public static bool operator >=(LVec3D a, LVec3D b) =>
			a.x >= b.x && a.y >= b.y && a.z >= b.z;

		public static bool operator <=(LVec3D a, LVec3D b) =>
			a.x <= b.x && a.y <= b.y && a.z <= b.z;

		public static LVec3D operator *(LVec3D a, long v) =>
			new LVec3D(a.x * v, a.y * v, a.z * v);

		public long ManhattanDistance(LVec3D b) =>
			Math.Abs(x - b.x) + Math.Abs(y - b.y) + Math.Abs(z - b.z);

		public int CompareTo(LVec3D? other) {
			if (other == null) return 1;
			if (x == other.x && y == other.y && z == other.z) return 0;
			return (x * x + y * y + z * z).CompareTo(other.x * other.x + other.y * other.y + other.z * other.z);
		}

	}
}
