using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
			public XY Pos2 { get; set; }
			public XY? PushVector { get; set; }
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
					int actX = 0;
					for (int x = 0; x < l.Length; x++) {
						switch (l[x]) {
							case '@': robot = new WhObj(Types.Robot) { Pos = new XY(actX, y) }; break;
							case '#': walls.Add(new WhObj(Types.Wall) { Pos = new XY(actX, y), Pos2 = new XY(actX + 1, y) }); break;
							case 'O': boxes.Add(new WhObj(Types.Box) { Pos = new XY(actX, y), Pos2 = new XY(actX + 1, y) }); break;
						}
						actX += 2;
					}
				} else {
					moves.AddRange(f[y].ToCharArray());
				}
			}
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var move in moves) Move(move);
			//Debug();
			foreach (var box in boxes) result += box.Pos.x + box.Pos.y * 100;
			return result;
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

		private bool PushPart2(WhObj toPush, XY vector) {
			XY dest = new(toPush.Pos.x + vector.x, toPush.Pos.y + vector.y);
			XY? dest2 = toPush.Pos2 == null ? null : new(toPush.Pos2.x + vector.x, toPush.Pos2.y + vector.y);
			if (walls.Any(w => w.Pos == dest || w.Pos2 == dest)
				|| (dest2 != null && walls.Any(w => w.Pos == dest2 || w.Pos2 == dest2))) {
				return false;
			}
			bool canPush = true;
			List<WhObj> toPushBoxes = new();
			toPushBoxes.AddRange(boxes.Where(w => w.Pos == dest || w.Pos2 == dest).ToList());
			if (dest2 != null) toPushBoxes.AddRange(boxes.Where(w => w.Pos == dest2 || w.Pos2 == dest2).ToList());
			foreach (var box in toPushBoxes.Where(x => x != toPush)) {
				//If a box has Pos1, Pos2, and I apply a vector of(-1, 0) to Pos2
				//I get back the box itself as a box to push - so I have to check if the destination is a different object
				canPush = canPush && PushPart2(box, vector);
			}
			if (canPush) toPush.PushVector = vector;
			else toPush.PushVector = null;
			return canPush;
		}

		private void DebugPart2() {
			Console.Clear();
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width * 2; x++) {
					var pos = new XY(x, y);
					if (walls.Any(w => w.Pos == pos || w.Pos2 == pos)) Console.Write('#');
					else if (robot.Pos == pos) Console.Write('@');
					else if (boxes.Any(b => b.Pos == pos)) Console.Write('[');
					else if (boxes.Any(b => b.Pos2 == pos)) Console.Write(']');
					else Console.Write('.');
				}
				Console.WriteLine();
			}
		}

		protected override long SolvePart2() {
			long result = 0;
			foreach (var move in moves) MovePart2(move);
			//DebugPart2();
			foreach (var box in boxes) result += box.Pos.x + box.Pos.y * 100;
			return result;
		}
		private void MovePart2(char move) {
			//DebugPart2();
			bool canPush;
			switch (move) {
				case '<':
					canPush = PushPart2(robot, new XY(-1, 0)); break;
				case '>':
					canPush = PushPart2(robot, new XY(1, 0)); break;
				case '^':
					canPush = PushPart2(robot, new XY(0, -1)); break;
				case 'v':
					canPush = PushPart2(robot, new XY(0, 1)); break;
				default:
					throw new Oopsie("Invalid move");
			}
			foreach (var box in boxes) { //moves are committed only if all boxes can be moved
				if (canPush && box.PushVector != null) {
					box.Pos += box.PushVector;
					box.Pos2 += box.PushVector;
				}
				box.PushVector = null;
			}
			if (canPush && robot.PushVector != null) robot.Pos += robot.PushVector;
			robot.PushVector = null;

		}
		private void Move(char move) {	//moves are committed instantly
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

	}
}
