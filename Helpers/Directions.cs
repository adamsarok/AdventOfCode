using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public class Directions {
		public enum Direction {
			North,
			South,
			West,
			East
		}
		public static Dictionary<Direction, LVec> Compass = new Dictionary<Direction, LVec>() {
			{ Direction.North, new LVec(0, -1) },
			{ Direction.South, new LVec(0, 1) },
			{ Direction.West, new LVec(-1, 0) },
			{ Direction.East, new LVec(1, 0) }
		};
		public static Direction Opposite(Direction dir) {
			return dir switch {
				Direction.North => Direction.South,
				Direction.South => Direction.North,
				Direction.West => Direction.East,
				Direction.East => Direction.West,
				_ => throw new Exception("Unknown direction")
			};
		}
	}
}
