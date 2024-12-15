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
		List<char> moves;
		public enum Types { Robot, Wall, Box }
		class WhObj(Types type) {
			public XY Pos { get; set; }
			public Types Type => type;
		}
		int height, width;
		WhObj robot;
		List<WhObj> walls;
		List<WhObj> boxes;
		protected override void ReadInputPart1(string fileName) {
			var f = File.ReadAllLines(fileName);
			moves = new();
			walls = new();
			boxes = new();
			bool readingWh = true;
			for (int y = 0; y < f.Length; y++) {
				var l = f[y];
				if (string.IsNullOrWhiteSpace(l)) {
					height = y;
					readingWh = false;
				} else if (readingWh) {
					width = l.Length;
					for (int x = 0; x < l.Length; x++) {
						switch (l[x]) {
							case '@': robot = new WhObj(Types.Robot) { Pos = new XY(x, y) }; break;
							case '#': walls.Add(new WhObj(Types.Wall) { Pos = new XY(x, y) }); break;
							case 'O': boxes.Add(new WhObj(Types.Box) { Pos = new XY(x, y) }); break;
						}
					}
				} else {
					moves.AddRange(f[y].ToCharArray());
				}
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var move in moves) Move(move);
			//Debug();
			foreach (var box in boxes) result += box.Pos.x + box.Pos.y * 100;
			return result;
		}

		private void Move(char move) {
			//Debug();
			switch (move) {
				case '<':
					Push(robot, new XY(-1, 0)); break;
				case '>':
					Push(robot, new XY(1, 0)); break;
				case '^':
					Push(robot, new XY(0, -1)); break;
				case 'v':
					Push(robot, new XY(0, 1)); break;
				default:
					throw new Oopsie("Invalid move");
			}
		}

		private void Debug() {
			Console.Clear();
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					var pos = new XY(x, y);
					if (walls.Any(w => w.Pos == pos)) Console.Write('#');
					else if (robot.Pos == pos) Console.Write('@');
					else if (boxes.Any(b => b.Pos == pos)) Console.Write('O');
					else Console.Write('.');
				}
				Console.WriteLine();
			}
		}

		//TODO: these searches can be much faster if I maintain a dictionary of what obj is at what position
		//Dictionary<XY, WhObj> map = new(); //536 ms

		private bool Push(WhObj toPush, XY vector) { 
			XY dest = new(toPush.Pos.x + vector.x, toPush.Pos.y + vector.y);
			if (walls.Any(w => w.Pos == dest)) return false; 
			bool canPush = true;
			var box = boxes.FirstOrDefault(b => b.Pos == dest);
			if (box != null) canPush = Push(box, vector); 
			if (canPush) toPush.Pos = dest;
			return canPush;
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
