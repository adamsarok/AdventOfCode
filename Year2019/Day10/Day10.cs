using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day10 {
	public class Day10 : Solver {
		public Day10() : base(2019, 10) {
		}

		List<LVec> asteroids;

		protected override void ReadInputPart1(string fileName) {
			var input = File.ReadAllLines(fileName);
			asteroids = new List<LVec>();
			for (int y = 0; y < input.Length; y++) {
				var l = input[y];
				for (int x = 0; x < input[0].Length; x++) {
					if (l[x] == '#') asteroids.Add(new LVec(x, y));
				}
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var start in asteroids) {
				Dictionary<double, List<LVec>> asteroidsByAngleToMonitoringPoint = new Dictionary<double, List<LVec>>();
				//1. get the angle from the monitoring point to all other asteroids
				//2. if 2 or more asteroids are on the same angle only take the closest
				//3. which means result = count of angle groups
				foreach (var other in asteroids.Where(x => x != start)) {
					double deltaY = other.y - start.y;
					double deltaX = other.x - start.x;
					double angleInRadians = Math.Atan2(deltaY, deltaX);
					double angleInDegrees = angleInRadians * (180.0 / Math.PI);
					List<LVec> group;
					if (!asteroidsByAngleToMonitoringPoint.TryGetValue(angleInDegrees, out group)) {
						group = new List<LVec>();
						asteroidsByAngleToMonitoringPoint.Add(angleInDegrees, group);
					}
					group.Add(other);
				}
				result = Math.Max(result, asteroidsByAngleToMonitoringPoint.Count);
			}
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
