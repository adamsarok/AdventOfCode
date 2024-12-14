using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023.Day06 {
	public class Day06 : Solver {
		public Day06() : base(2023, 6) {
		}
		protected override void ReadInputPart1(string fileName) {
			//input = new();
			foreach (var l in File.ReadAllLines(fileName)) {

			}
		}

		protected override void ReadInputPart2(string fileName) {
			//input = new();
			foreach (var l in File.ReadAllLines(fileName)) {

			}
		}

		protected override long SolvePart1() {
			var lines = File.ReadAllLines("day6input.txt");
			List<int> times = new List<int>();
			List<int> distances = new List<int>();
			lines[0]
				.Split(':')[1]
				.Split(' ')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToList()
				.ForEach(x => times.Add(int.Parse(x)));
			lines[1]
				.Split(':')[1]
				.Split(' ')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToList()
				.ForEach(x => distances.Add(int.Parse(x)));
			int result = 1;
			for (int i = 0; i < times.Count; i++) {
				Console.WriteLine($"Race:{i}");
				int waysToBeat = 0;
				var maxTime = times[i];
				var distToBeat = distances[i];
				for (int tPressed = 1; tPressed <= maxTime - 1; tPressed++) { //press for at least 1 ms, max totalTime -1 ms
					var distance = tPressed * (maxTime - tPressed);
					if (distance > distToBeat) {
						Console.WriteLine($"tPressed:{tPressed} tTravel:{maxTime - tPressed} dTraveled:{distance}");
						waysToBeat++;
					}
				}
				result *= waysToBeat;
			}
			if (result == 1) result = 0;
			return result;
		}

		protected override long SolvePart2() {
			var lines = File.ReadAllLines("day6input.txt");
			var maxTime = long.Parse(string.Join("", lines[0]
				.Split(':')[1]
				.Split(' ')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToList()));
			var distToBeat = long.Parse(string.Join("", lines[1]
				.Split(':')[1]
				.Split(' ')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToList()));
			int result = 1;
			int waysToBeat = 0;
			for (long tPressed = 1; tPressed <= maxTime - 1; tPressed++) {
				long distance = tPressed * (maxTime - tPressed);
				if (distance > distToBeat) {
					waysToBeat++;
				}
			}
			result *= waysToBeat;
			if (result == 1) result = 0;
			return result;
		}
	}
}
