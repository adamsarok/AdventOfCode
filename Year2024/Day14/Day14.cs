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

namespace Year2024.Day14 {
	public class Day14 : Solver {
		public Day14() : base(2024, 14) {
		}

		public const int HEIGHT = 103;
		public const int WIDTH = 101;

		List<Robot> input;
		class Robot(Vec pos, Vec vector) {

			public void Step() {
				pos += vector;
				if (pos.x >= WIDTH) {
					pos = new Vec(pos.x - WIDTH, pos.y);
				} else if (pos.x < 0) {
					pos = new Vec(pos.x + WIDTH, pos.y);
				}
				if (pos.y >= HEIGHT) {
					pos = new Vec(pos.x, pos.y - HEIGHT);
				} else if (pos.y < 0) {
					pos = new Vec(pos.x, pos.y + HEIGHT);
				}
			}
			public Vec Pos => pos;
			public Vec Vector => vector;
		}


		private void ReadInput(string fileName) {
			var f = File.ReadAllLines(fileName);
			input = new();
			for (int i = 0; i < f.Length; i++) {
				if (!f[i].StartsWith("p=")) throw new Oopsie();
				var p = f[i].Split(' ')[0].Split('=');
				var px = int.Parse(p[1].Split(',')[0]);
				var py = int.Parse(p[1].Split(',')[1]);

				var v = f[i].Split(' ')[1].Split('=');
				var vx = int.Parse(v[1].Split(',')[0]);
				var vy = int.Parse(v[1].Split(',')[1]);
				input.Add(new Robot(new Vec(px, py), new Vec(vx, vy)));
			}
		}

		protected override void ReadInputPart1(string fileName) {
			ReadInput(fileName);
		}

		private void Debug() {
			Console.Clear();
			for (int y = 0; y < HEIGHT; y++) {
				for (int x = 0; x < WIDTH; x++) {
					var c = input.Where(r => r.Pos == new Vec(x, y)).Count();
					Console.Write(c == 0 ? "." : c.ToString());
				}
				Console.WriteLine();
			}
		}
		
		protected override long SolvePart1() {
			for (int i = 0; i < 100; i++) {
				//Debug();
				foreach (var r in input) r.Step();
			}
			var q1s = new Vec(0, 0);
			var q1e = new Vec(WIDTH / 2 - 1, HEIGHT / 2 - 1);
			var q2s = new Vec(WIDTH / 2 + 1, 0);
			var q2e = new Vec(WIDTH - 1, HEIGHT / 2 - 1);
			var q3s = new Vec(0, HEIGHT / 2 + 1);
			var q3e = new Vec(WIDTH / 2 - 1, HEIGHT - 1);
			var q4s = new Vec(WIDTH / 2 + 1, HEIGHT / 2 + 1);
			var q4e = new Vec(WIDTH - 1, HEIGHT - 1);

			long q1 = input.Where(r => r.Pos >= q1s && r.Pos <= q1e).Count();
			long q2 = input.Where(r => r.Pos >= q2s && r.Pos <= q2e).Count();
			long q3 = input.Where(r => r.Pos >= q3s && r.Pos <= q3e).Count();
			long q4 = input.Where(r => r.Pos >= q4s && r.Pos <= q4e).Count();

			var err = input.Where(r => r.Pos.x < 0 || r.Pos.y < 0 || r.Pos.x >= WIDTH || r.Pos.y >= HEIGHT)
				.ToList();
			if (err.Count > 0) throw new Oopsie($"{err.Count} robot(s) out of bounds");

			Debug();
			return q1 * q2 * q3 * q4;
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInput(fileName);
		}


		protected override long SolvePart2() {
			long steps = 1;
			while (true) { 
				//Debug();
				foreach (var r in input) r.Step();
				if (!input.GroupBy(r => r.Pos).Any(g => g.Count() > 1)) {
					//Debug();
					return steps;
				}
				steps++;
			}
		}
	}
}