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
	}
}
