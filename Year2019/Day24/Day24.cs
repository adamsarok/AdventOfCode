using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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

		int GetAdjBugs2(int x, int y, Layer layer) {
			int cnt = 0;
			if (x > 0 && layer.Value[x - 1, y]) cnt++;
			if (y > 0 && layer.Value[x, y - 1]) cnt++;
			if (x < width - 1 && layer.Value[x + 1, y]) cnt++;
			if (y < height - 1 && layer.Value[x, y + 1]) cnt++;
			var outer = layers[layer.Level - 1];
			if (outer != null) {
				if (x == 0 && outer.Value[1, 2]) {
					cnt++;
				} else if (x == width - 1 && outer.Value[3, 2]) {
					cnt++;
				}
				if (y == 0 && outer.Value[2, 1]) {
					cnt++;
				} else if (y == height - 1 && outer.Value[2, 3]) {
					cnt++;
				}
			}
			var inner = layers[layer.Level + 1];
			if (inner != null) {
				if (x == 1 && y == 2) {
					if (inner.Value[0, 0]) cnt++;
					if (inner.Value[0, 1]) cnt++;
					if (inner.Value[0, 2]) cnt++;
					if (inner.Value[0, 3]) cnt++;
					if (inner.Value[0, 4]) cnt++;
				} else if (x == 3 && y == 2) {
					if (inner.Value[4, 0]) cnt++;
					if (inner.Value[4, 1]) cnt++;
					if (inner.Value[4, 2]) cnt++;
					if (inner.Value[4, 3]) cnt++;
					if (inner.Value[4, 4]) cnt++;
				} else if (x == 2 && y == 1) {
					if (inner.Value[0, 0]) cnt++;
					if (inner.Value[1, 0]) cnt++;
					if (inner.Value[2, 0]) cnt++;
					if (inner.Value[3, 0]) cnt++;
					if (inner.Value[4, 0]) cnt++;
				} else if (x == 2 && y == 3) {
					if (inner.Value[0, 4]) cnt++;
					if (inner.Value[1, 4]) cnt++;
					if (inner.Value[2, 4]) cnt++;
					if (inner.Value[3, 4]) cnt++;
					if (inner.Value[4, 4]) cnt++;
				}
			}
			return cnt;
		}
		class Layer {
			public bool[,] Value { get; set; }
			public bool[,] NextValue { get; set; }
			public bool SpawnOuter { get; set; }
			public bool SpawnInner { get; set; }
			public int Level { get; set; }
		}

		Layer[] layers;

		protected override long SolvePart2() {
			long result = 0;

			int iterations = IsShort ? 10 : 200;

			layers = new Layer[iterations * 2 + 1];
			int center = iterations;

			layers[center] = new Layer() { Value = bugs, Level = center };
			layers[center].Value[2, 2] = false;
			layers[center - 1] = new Layer() { Value = new bool[width, height], Level = center - 1 };
			layers[center + 1] = new Layer() { Value = new bool[width, height], Level = center + 1 };
			//Debug(layers[center]);
			for (int i = 0; i < iterations; i++) {
				for (int l = 0; l < layers.Length; l++) {
					var layer = layers[l];
					if (layer == null) continue;
					Calc(layer);
				}
				var top = layers.First(x => x != null);
				if (top.SpawnOuter) {
					layers[top.Level - 1] = new Layer() { Value = new bool[width, height], Level = top.Level - 1 };
					top.SpawnOuter = false;
					Calc(layers[top.Level - 1]);
				}
				var bott = layers.Last(x => x != null);
				if (bott.SpawnInner) {
					layers[bott.Level + 1] = new Layer() { Value = new bool[width, height], Level = bott.Level + 1 };
					bott.SpawnInner = false;
					Calc(layers[top.Level + 1]);
				}
				for (int l = 0; l < layers.Length; l++) {
					var layer = layers[l];
					if (layer == null || layer.NextValue == null) continue;
					layer.Value = layer.NextValue;
					layer.NextValue = null;
				}

				//if (i == iterations - 1) {
				//	for (int l = 0; l < layers.Length; l++) {
				//		var layer = layers[l];
				//		if (layer == null) continue;
				//		Debug(layer);
				//	}
				//}

			}

			for (int l = 0; l < layers.Length; l++) {
				var layer = layers[l];
				if (layer == null) continue;
				result += Sum(layer);
			}
			return result;
		}

		private void Debug(Layer layer) {
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					Console.Write(layer.Value[x, y] ? "#" : ".");
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		private long Sum(Layer layer) {
			long result = 0;
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (layer.Value[x, y]) result++;
				}
			}
			//Console.WriteLine($"Layer {layer.Level}: {result} bugs");
			return result;
		}

		private void Calc(Layer layer) {
			bool[,] next = new bool[width, height];
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					var bug = layer.Value[x, y];
					var adj = GetAdjBugs2(x, y, layer);
					if (bug) {
						if (adj == 1) next[x, y] = true;
						if (layers[layer.Level - 1] == null && IsSpawnOuter(x, y)) layer.SpawnOuter = true;
						if (layers[layer.Level + 1] == null && IsSpawnInner(x, y)) layer.SpawnInner = true;
					} else {
						if (adj == 1 || adj == 2) next[x, y] = true;
					}
				}
			}
			next[2, 2] = false;
			layer.NextValue = next;
		}
		private bool IsSpawnOuter(int x, int y) {
			return x == 0 || x == width - 1 || y == 0 || y == height - 1;
		}
		private bool IsSpawnInner(int x, int y) {
			return true; //something was not right here, however always spawning an inner layer works and still fast
			//return (x == 2 && y == 1) || (x == 2 && y == 3) || (x == 1 && y == 2) || (x == 3 && y == 2);
		}
	}
}
