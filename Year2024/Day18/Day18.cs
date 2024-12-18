using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Year2024.Day18 {
	public class Day18 : Solver {
		public Day18() : base(2024, 18) {
		}
		private bool[,] corruptedMemory; //x, y !!!
		private int size;
		string[] file;
		private int byteLimit;
		protected override void ReadInputPart1(string fileName) {
			if (fileName.Contains("short")) {
				size = 7;
				byteLimit = 12;
			} else {
				size = 71;
				byteLimit = 1024;
			}
			corruptedMemory = new bool[size, size];
			int b = 0;
			file = File.ReadAllLines(fileName);
			foreach (var l in File.ReadAllLines(fileName)) {
				AddByte(l);
				b++;
				if (b >= byteLimit) break;
			}
		}

		private void AddByte(string line) {
			var s = line.Split(",");
			corruptedMemory[int.Parse(s[0]), int.Parse(s[1])] = true;
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		PriorityQueue<int, Point> queue;
		//works on short input but way too long queue on final input
		protected long SolvePriorityQueue() {
			long result = 0;
			costs = new long[size, size];
			for (int x = 0; x < size; x++) {
				for (int y = 0; y < size; y++) {
					costs[x, y] = long.MaxValue;
				}
			}
			costs[0, 0] = 0;
			queue = new PriorityQueue<int, Point>();
			queue.Enqueue(0, new Point(0, 0));
			Point next;
			int prio;
			while (queue.TryDequeue(out prio, out next)) {
				TryQueue(new Point(next.x - 1, next.y), prio + 1);
				TryQueue(new Point(next.x + 1, next.y), prio + 1);
				TryQueue(new Point(next.x, next.y - 1), prio + 1);
				TryQueue(new Point(next.x, next.y + 1), prio + 1);
			}
			return costs[size-1, size-1];
		}

		private void TryQueue(Point point, int prio) {
			if (point.x < 0 || point.y < 0 || point.x >= size || point.y >= size) return;
			if (corruptedMemory[point.x, point.y]) return;
			if (costs[point.x, point.y] < prio) return;
			costs[point.x, point.y] = prio;
			queue.Enqueue(prio, point);
			//Debug();
		}

		private long[,] costs;
		bool[,] dirty;
		override protected long SolvePart1() {
			dirty = new bool[size, size];
			costs = new long[size, size];
			for (int x = 0; x < size; x++) {
				for (int y = 0; y < size; y++) {
					costs[x, y] = long.MaxValue;
				}
			}
			costs[0, 0] = 0;
			dirty[0, 0] = true;
			while (true) {
				bool wasDirty = false;
				for (int x = 0; x < size; x++) {
					for (int y = 0; y < size; y++) {
						if (dirty[x, y]) {
							dirty[x, y] = false;
							var cost = costs[x, y] + 1;
							Process(x - 1, y, cost);
							Process(x + 1, y, cost);
							Process(x, y - 1, cost);
							Process(x, y + 1, cost);
							wasDirty = true;
						}
					}
				}
				if (!wasDirty) return costs[size - 1, size - 1];
			}
		}
		private void Process(int x, int y, long cost) {
			if (x < 0 || y < 0 || x >= size || y >= size) return;
			if (corruptedMemory[x, y]) return;
			if (costs[x, y] < cost) return;
			costs[x, y] = cost;
			dirty[x, y] = true;
		}

		private void Debug() {
			Console.Clear();
			for (int y = 0; y < size; y++) {
				for (int x = 0; x < size; x++) {
					if (corruptedMemory[x, y]) Console.Write("  X  ");
					else Console.Write(costs[x, y] == long.MaxValue ? " 000 " : $" {costs[x, y].ToString("000")} ");
				}
				Console.WriteLine();
			}
		}

		protected override long SolvePart2() {
			long result = 0;
			for (int i = byteLimit; i < file.Length; i++) {
				var line = file[i];
				AddByte(line);
				var path = SolvePart1();
				if (path == long.MaxValue) {
					Console.WriteLine(line);
					return 1;
				}
			}
			throw new Oopsie("Didn't find blocking byte");
		}
	}
}
