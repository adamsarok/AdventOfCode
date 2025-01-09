using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day24 {
	public class Day24 : Solver {
		public Day24() : base(2019, 24) {
		}
		bool[,] bugs;
		HashSet<long> states;
		int height, width;
		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			var lines =  File.ReadAllLines(fileName);
			height = lines.Length;
			width = lines[0].Length;
			bugs = new bool[width, height];
			for (int y = 0; y < lines.Length; y++) {
				for (int x = 0; x < lines[0].Length; x++) {
					bugs[x, y] = lines[y][x] == '#'; 
				}
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			long state = -1;
			states = new HashSet<long>();
			//Debug();
			while (!states.Contains(state)) {
				states.Add(state);
				bool[,] next = new bool[width, height];
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						var bug = bugs[x, y];
						var adj = GetAdjBugs(x,y);
						if (bug) {
							if (adj == 1) next[x, y] = true;
						} else {
							if (adj == 1 || adj == 2) next[x, y] = true;
						}
					}
				}
				bugs = next;
				//Debug();
				state = GetState();
			}
			return state;
		}

		private void Debug() {
			Console.Clear();
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					Console.Write(bugs[x, y] ? "#" : ".");
				}
				Console.WriteLine();
			}
		}

		int GetAdjBugs(int x, int y) {
			int cnt = 0;
			if (x > 0 && bugs[x - 1, y]) cnt++;
			if (y > 0 && bugs[x, y - 1]) cnt++;
			if (x < width - 1 && bugs[x + 1, y]) cnt++;
			if (y < height - 1 && bugs[x, y + 1]) cnt++;
			return cnt;
		}

		private long GetState() {
			long state = 0;
			int bitPosition = 0;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (bugs[x, y]) {
						state |= (1L << bitPosition);
					}
					bitPosition++;
				}
			}
			return state;
		}

		LinkedList<bool[,]> levels;

		int GetAdjBugs2(int x, int y, Layer layer) {
			int cnt = 0;
			if (x > 0 && bugs[x - 1, y]) cnt++;
			if (y > 0 && bugs[x, y - 1]) cnt++;
			if (x < width - 1 && bugs[x + 1, y]) cnt++;
			if (y < height - 1 && bugs[x, y + 1]) cnt++;
			if (layer.Outer != null) {
				if (x == 0 && layer.Outer.Value[1, 2]) cnt++;
				if (y == 0 && layer.Outer.Value[2, 1]) cnt++;
				if (x == width && layer.Outer.Value[3, 2]) cnt++;
				if (y == height && layer.Outer.Value[2, 3]) cnt++;
			}
			if (layer.Inner != null) {
				if (x == 1 && y == 2) {
					if (layer.Inner.Value[0, 0]) cnt++;
					if (layer.Inner.Value[0, 1]) cnt++;
					if (layer.Inner.Value[0, 2]) cnt++;
					if (layer.Inner.Value[0, 3]) cnt++;
					if (layer.Inner.Value[0, 4]) cnt++;
				}
				if (x == 3 && y == 2) {
					if (layer.Inner.Value[4, 0]) cnt++;
					if (layer.Inner.Value[4, 1]) cnt++;
					if (layer.Inner.Value[4, 2]) cnt++;
					if (layer.Inner.Value[4, 3]) cnt++;
					if (layer.Inner.Value[4, 4]) cnt++;
				}
				if (x == 2 && y == 1) {
					if (layer.Inner.Value[0, 0]) cnt++;
					if (layer.Inner.Value[1, 0]) cnt++;
					if (layer.Inner.Value[2, 0]) cnt++;
					if (layer.Inner.Value[3, 0]) cnt++;
					if (layer.Inner.Value[4, 0]) cnt++;
				}
				if (x == 2 && y == 3) {
					if (layer.Inner.Value[0, 4]) cnt++;
					if (layer.Inner.Value[1, 4]) cnt++;
					if (layer.Inner.Value[2, 4]) cnt++;
					if (layer.Inner.Value[3, 4]) cnt++;
					if (layer.Inner.Value[4, 4]) cnt++;
				}
			}
			return cnt;
		}
		class Layer {
			public bool[,] Value { get; set; }
			public Layer Outer { get; set; }
			public Layer Inner { get; set; }
		}
		protected override long SolvePart2() {
			long result = 0;
			//we are going down, each timestep should populate a new recursive tile?

			//An empty space becomes infested with a bug if exactly one or two bugs are adjacent to it.
			//spawn new tile up: bugs exist on the outside ring
			//spawn new tile down: bugs exist on the inside ring (1,1) -> (3,3)

			return -1;

			var first = new Layer() { Value = bugs };
			Layer next;
			Stopwatch sw = Stopwatch.StartNew();
			for (int i = 0; i < 200; i++) {
				//TODO: this is incorrect! we need to store a next state of all layers

				Console.WriteLine($"Iteration {i} in {sw.ElapsedMilliseconds} ms");
				next = first;
				Calc(next);
				while (next.Inner != null) {
					Calc(next.Inner);
					next = next.Inner;
				}
				next = first;
				while (next.Outer != null) {
					Calc(next.Outer);
					next = next.Outer;
				}
			}
			result += Sum(first.Value);
			next = first;
			while (next.Inner != null) {
				result += Sum(next.Inner.Value);
				next = next.Inner;
			}
			next = first;
			while (next.Outer != null) {
				result += Sum(next.Outer.Value);
				next = next.Outer;
			}
			return result;
		}
		private long Sum(bool[,] bugs) {
			long result = 0;
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (bugs[x, y]) result++;
				}
			}
			return result;
		}

		private void Calc(Layer layer) {
			bool[,] next = new bool[width, height];
			bool spawnOuter = false;
			bool spawnInner = false;
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					var bug = bugs[x, y];
					var adj = GetAdjBugs2(x, y, layer);
					if (bug) {
						if (adj == 1) next[x, y] = true;
						if (layer.Outer == null && IsSpawnOuter(x, y)) spawnOuter = true;
						if (layer.Inner == null && IsSpawnInner(x, y)) spawnInner = true;
					} else {
						if (adj == 1 || adj == 2) next[x, y] = true;
					}
				}
			}
			if (layer.Outer != null) 
			if (spawnInner) {
				layer.Inner = new Layer() { Outer = layer, Value = new bool[width, height] };
			}
			if (spawnOuter) {
				layer.Outer = new Layer() { Inner = layer, Value = new bool[width, height] };
			}
			bugs = next; //TODO: this is incorrect! we need to store a next state of all layers
		}
		private bool IsSpawnOuter(int x, int y) {
			return x == 0 || x == width - 1 || y == 0 || y == height - 1;
		}
		private bool IsSpawnInner(int x, int y) {
			return (x == 2 && y == 1) || (x == 2 && y == 3) || (x == 1 && y == 2) || (x == 3 && y == 2);
		}
	}
}
