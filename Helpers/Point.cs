using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public record Point(int x, int y) {
		public static Point operator +(Point a, Point b) =>
			new Point(a.x + b.x, a.y + b.y);

		public static Point operator -(Point a, Point b) =>
			new Point(a.x - b.x, a.y - b.y);
	}
}
