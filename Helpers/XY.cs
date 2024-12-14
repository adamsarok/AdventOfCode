using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public record XY(long x, long y) {
		public static XY operator +(XY a, XY b) =>
			new XY(a.x + b.x, a.y + b.y);

		public static XY operator -(XY a, XY b) =>
			new XY(a.x - b.x, a.y - b.y);

		public static bool operator >=(XY a, XY b) =>
			a.x >= b.x && a.y >= b.y;

		public static bool operator <=(XY a, XY b) =>
			a.x <= b.x && a.y <= b.y;
	}
}
