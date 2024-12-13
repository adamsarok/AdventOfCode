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
		record class ClawMachine(XY A, XY B, XY p) {
			//a = 3 tokens, b = 1 tokens
			//max 100 press per button
			public long MinimumPrize {
				get {
					long minPrize = long.MaxValue;
					long minPressX = p.x / Math.Max(A.x, B.x);
					long minPressY = p.y / Math.Max(A.y, B.y);
					long minPress = Math.Max(minPressX, minPressY);
					for (int pressA = 0; pressA <= 100; pressA++) {
						for (int pressB = 0; pressB <= 100; pressB++) {
							if (pressA+pressB < minPress) continue;
							if (pressA * A.x + pressB * B.x == p.x &&
								pressA * A.y + pressB * B.y == p.y) {
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
				get { //this is much faster then brute force but still not enough :-(
					long minPrize = long.MaxValue;
					long minPressX = p.x / Math.Max(A.x, B.x);
					long minPressY = p.y / Math.Max(A.y, B.y);
					long minPress = Math.Max(minPressX, minPressY);
					long maxPressX = p.x / Math.Min(A.x, B.x);
					long maxPressY = p.y / Math.Min(A.y, B.y);
					long maxPress = Math.Min(maxPressX, maxPressY);
					for (long pressA = 0; pressA <= maxPress; pressA++) {
						//if prize.X - (pressA * buttonA.x) % buttonB.x != 0 rest is not divisible by buttonB = invalid we don't need to check!
						if (((p.x - (pressA * A.x)) % B.x != 0) 
							|| (p.y - (pressA * A.y)) % B.y != 0) continue;
						long pressBx = (p.x - (pressA * A.x)) / B.x;
						long pressBy = (p.y - (pressA * A.y)) / B.y;
						if (pressBx == pressBy) {
							return pressA * 3 + pressBx;
						}
					}
					if (minPrize == long.MaxValue) return 0;
					return minPrize;
				}
			}
			public long MinimumPrizePart22 {
				get {
					//Ax * S + Bx * T = Px  // * By
					//Ay * S + By * T = Py  // * Bx

					//Ax * S * By + Bx * By * T = Px * By 
					//Ay * S * Bx + By * Bx * T = Py * Bx

					//Px * By - Ax * S * By = Bx * By * T
					//Py * Bx - Ay * S * Bx = By * Bx * T

					//Px * By - Ax * S * By = Py * Bx - Ay * S * Bx //no more T
					//PxBy - AxByS = PyBx - AyBxS
					//PxBy - PyBx = AxByS - AyBxS
					//PxBy - PyBx = S(AxBy - AyBx)
					//S = (PxBy - PyBx) / (AxBy - AyBx) //= A press

					//Px * By - Ax * S * By = Bx * By * T
					//T = (PxBy - AxByS) / BxBy			//= B press
					decimal Ax = A.x, Ay = A.y, Bx = B.x, By = B.y;
					decimal px = p.x, py = p.y;

					var s = (px * By - py * Bx) / (Ax * By - Ay * Bx);
					var t = (px * By - Ax * By * s) / (Bx * By);

					if (s < 0 || t < 0 || s % 1 != 0 || t % 1 != 0) {
						return 0;       //aaaaah only integer presses are valid
					}
					return (long)s * 3 + (long)t;
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
				long px = long.Parse(prize[0].Split('=')[1]);
				long py = long.Parse(prize[1].Split('=')[1]);
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
			ReadInput(fileName, 10000000000000);
		}


		protected override long SolvePart2() {
			long result = 0;
			foreach (var c in input) result += c.MinimumPrizePart22;
			return result;
		}
	}
}