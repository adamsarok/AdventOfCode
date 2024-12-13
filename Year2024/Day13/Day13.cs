using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day13 {
	public class Day13 : Solver {
		public Day13() : base(2024, 13) {
		}
		record XY(long x, long y);

		List<ClawMachine> input;
		record class ClawMachine(XY buttonA, XY buttonB, XY prize) {
			//a = 3 tokens, b = 1 tokens
			//max 100 press per button
			public long MinimumPrize {
				get {
					long minPrize = long.MaxValue;
					long minPressX = prize.x / Math.Max(buttonA.x, buttonB.x);
					long minPressY = prize.y / Math.Max(buttonA.y, buttonB.y);
					long minPress = Math.Max(minPressX, minPressY);
					for (int pressA = 0; pressA <= 100; pressA++) {
						for (int pressB = 0; pressB <= 100; pressB++) {
							if (pressA+pressB < minPress) continue;
							if (pressA * buttonA.x + pressB * buttonB.x == prize.x &&
								pressA * buttonA.y + pressB * buttonB.y == prize.y) {
								minPrize = Math.Min(minPrize, pressA * 3 + pressB * 1);
								//Console.WriteLine($"Press A: {pressA}, Press B: {pressB}, TotalPres:{pressA+pressB} MinPress: {minPress}, Prize:{prize}");
							}
						}
					}
					if (minPrize == long.MaxValue) return 0;
					return minPrize;
				}
			}
			public long MinimumPrizePart2 {
				get {
					long minPrize = long.MaxValue;
					long pressA = 0, pressB = 0;
					
					//while (pressA * ax) {
						
					//}
					//for (int pressA = 0; pressA <= 100; pressA++) {
					//	for (int pressB = 0; pressB <= 100; pressB++) {
					//		if (pressA * buttonA.x + pressB * buttonB.x == prize.x &&
					//			pressA * buttonA.y + pressB * buttonB.y == prize.y) {
					//			minPrize = Math.Min(minPrize, pressA * 3 + pressB * 1);
					//		}
					//	}
					//}
					if (minPrize == long.MaxValue) return 0;
					return minPrize;
				}
			}
		}

		private void ReadInput(string fileName, long add) {
			var f = File.ReadAllLines(fileName);
			input = new();
			for (int i = 0; i < f.Length; i += 4) {
				if (!f[i].StartsWith("Button A:")) throw new Oopsie();
				var a = f[i].Split(':')[1].Split(',');
				var ax = int.Parse(a[0].Split('+')[1]);
				var ay = int.Parse(a[1].Split('+')[1]);
				var b = f[i + 1].Split(':')[1].Split(',');
				var bx = int.Parse(b[0].Split('+')[1]);
				var by = int.Parse(b[1].Split('+')[1]);
				var prize = f[i + 2].Split(':')[1].Split(',');
				var px = int.Parse(prize[0].Split('=')[1]);
				var py = int.Parse(prize[1].Split('=')[1]);
				input.Add(new ClawMachine(new XY(ax, ay), new XY(bx, by), new XY(px + add, py + add)));
			}
		}

		protected override void ReadInputPart1(string fileName) {
			ReadInput(fileName, 0);
		}

		
		protected override long SolvePart1() {
			long result = 0;
			foreach (var c in input) result += c.MinimumPrize;
			return result;
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInput(fileName, 0); // 10000000000000);
		}


		protected override long SolvePart2() {
			long result = 0;
			foreach (var c in input) result += c.MinimumPrizePart2;
			return result;
		}
	}
}