using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2021.Day02 {
	public class Day02 : Solver {
		public Day02() : base(2021, 2) {
		}
		Dictionary<string, Vec> commandTypes = new Dictionary<string, Vec>() {
			{ "forward", new Vec(1, 0) },
			{ "down", new Vec(0, 1) },
			{ "up", new Vec(0, -1) } };
		List<Vec> commands;
		protected override void ReadInputPart1(string fileName) {
			commands = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				var mul = int.Parse(l.Split(" ")[1]);
				var cmd = commandTypes[l.Split(" ")[0]];
				commands.Add(cmd * mul);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			var pos = new Vec(0, 0);
			foreach (var cmd in commands) {
				pos += cmd;
			}
			return pos.x * pos.y;
		}

		protected override long SolvePart2() {
			var pos = new Vec(0, 0);
			var aim = 0;
			foreach (var cmd in commands) {
				if (cmd.y != 0) {
					aim += cmd.y;
				} else {
					pos = new Vec(pos.x + cmd.x,
					  pos.y + (cmd.x * aim));
				}
			}
			return pos.x * pos.y;
		}
	}
}
