using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day15 {
	public class Day15 : Solver {
		public Day15() : base(2024, 15) {
		}
		char[,] warehouses;
		List<char> moves;
		int height, width;
		XY robot;
		protected override void ReadInputPart1(string fileName) {
			var f = File.ReadAllLines(fileName);
			List<char[]> whtemp = new();
			for (int y = 0; y < f.Length; y++) {
				var l = f[y];
				if (string.IsNullOrWhiteSpace(l)) {
					height = y;
					width = whtemp[0].Length;
					break;
				}
				whtemp.Add(l.ToCharArray());
			}
			warehouses = new char[height, width];
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (whtemp[y][x] == '@') robot = new XY(x, y);
					warehouses[y, x] = whtemp[y][x];
				}
			}
			moves = new();
			for (int y = height + 1; y < f.Length; y++) {
				moves.AddRange(f[y].ToCharArray());
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var move in moves) {
				var r = Move(move);
				if (r.isValid) warehouses = r.newState;
			}
			return result;
		}

		record MoveResult(bool isValid, char[,]? newState);

		private MoveResult Move(char move) {
			char[,] next = new char[height, width];
			Array.Copy(warehouses, next, warehouses.Length);
			switch (move) {
				case '<':
					return Push(robot, new XY(-1, 0), next);
				case '>':
					return Push(robot, new XY(1, 0), next);
				case '^':
					return Push(robot, new XY(0, -1), next);
				case 'v':
					return Push(robot, new XY(0, 1), next);
				default:
					throw new Oopsie("Invalid move");
			}
		}

		private MoveResult Push(char pushChar, XY pushAt, XY vector, char[,] next) {
			char destination = next[pushAt.y + vector.y, pushAt.x + vector.x];
			switch (destination) {
				case '#':
					return new MoveResult(false, null);
				case '.':
					next[pushAt.y + vector.y, pushAt.x + vector.x] = pushChar;
					next[pushAt.y, pushAt.x] = '.';
					robot += vector;
					return new MoveResult(true, next);
			}
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
