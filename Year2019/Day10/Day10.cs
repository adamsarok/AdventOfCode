using Helpers;
using System;
using System.Collections;
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
			var result = GetBestMonitoringStation();
			return result.AsteroidsSeen;
		}
		record Station(long AsteroidsSeen, LVec Pos);
		private Station GetBestMonitoringStation() {
			long seen = 0; LVec pos = new LVec(0,0);
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
				if (asteroidsByAngleToMonitoringPoint.Count > seen) {
					seen = asteroidsByAngleToMonitoringPoint.Count;
					pos = start;
				}
			}
			return new Station(seen, pos);
		}

		record Asteroid2(LVec Pos, long DistanceFromStart);
		protected override long SolvePart2() {
			var start = GetBestMonitoringStation();
			Dictionary<double, List<Asteroid2>> asteroidsByAngleToMonitoringPoint = new();
			foreach (var other in asteroids.Where(x => x != start.Pos)) {
				double deltaY = other.y - start.Pos.y;
				double deltaX = other.x - start.Pos.x;
				double angleInRadians = Math.Atan2(deltaY, deltaX);
				double angleInDegrees = - angleInRadians * (180.0 / Math.PI);
				List<Asteroid2> group;
				if (!asteroidsByAngleToMonitoringPoint.TryGetValue(angleInDegrees, out group)) {
					group = new();
					asteroidsByAngleToMonitoringPoint.Add(angleInDegrees, group);
				}
				group.Add(new Asteroid2(other, other.ManhattanDistance(start.Pos)));
			}
			//start from 90° go clockwise, go in quadrants
			int cnt = 0; LVec result = new LVec(0,0);
			while (cnt < 200) {
				var q = asteroidsByAngleToMonitoringPoint.Where(x => x.Key >= 0 && x.Key <= 90).OrderByDescending(x => x.Key);
				RemoveFirsts(q, ref cnt, ref result);
				q = asteroidsByAngleToMonitoringPoint.Where(x => x.Key >= -90 && x.Key <= 0).OrderByDescending(x => x.Key);
				RemoveFirsts(q, ref cnt, ref result);
				q = asteroidsByAngleToMonitoringPoint.Where(x => x.Key >= -180 && x.Key < -90).OrderByDescending(x => x.Key);
				RemoveFirsts(q, ref cnt, ref result);
				q = asteroidsByAngleToMonitoringPoint.Where(x => x.Key >= 90 && x.Key <= 180).OrderByDescending(x => x.Key);
				RemoveFirsts(q, ref cnt, ref result);
			}
			return result.x * 100 + result.y;
		}

		private static void RemoveFirsts(IOrderedEnumerable<KeyValuePair<double, List<Asteroid2>>> q, ref int cnt, ref LVec result) {
			foreach (var group in q) {
				var asteroid = group.Value.OrderBy(x => x.DistanceFromStart).FirstOrDefault();
				if (asteroid == null) continue;
				cnt++;
				group.Value.Remove(asteroid);
				Console.WriteLine($"The {cnt}. asteroid to be vaporized is at {asteroid.Pos.x},{asteroid.Pos.y}");
				if (cnt == 200) result = asteroid.Pos;
			}
		}
	}
}
