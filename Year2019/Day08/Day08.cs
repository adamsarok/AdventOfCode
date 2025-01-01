using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer = (byte[,], System.Collections.Generic.Dictionary<byte, long>);

namespace Year2019.Day08 {
	public class Day08 : Solver {
		public Day08() : base(2019, 8) {
		}
		List<byte> input;
		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			input = new List<byte>();
			foreach (var c in File.ReadAllLines(fileName)[0]) {
				input.Add(byte.Parse(c.ToString()));
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			if (IsShort) return -1;
			var layers = ParseInput();
			var fewestZeros = layers.Where(x => x.Item2[0] == layers.Select(x => x.Item2[0]).Min()).First();
			return fewestZeros.Item2[1] * fewestZeros.Item2[2];
		}

		private List<Layer> ParseInput() {
			List<Layer> layers = new List<Layer>();
			int act = 0;
			int width = IsShort ? 2 : 25;
			int height = IsShort ? 2 : 6;
			while (act < input.Count) {
				var bytes = new byte[width, height];
				var layer = new Layer(bytes, new Dictionary<byte, long>());
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						var c = input[act++];
						layer.Item1[x, y] = c;
						if (layer.Item2.ContainsKey(c)) layer.Item2[c]++;
						else layer.Item2.Add(c, 1);
					}
				}
				layers.Add(layer);
			}
			return layers;
		}

		protected override long SolvePart2() {
			if (IsShort) return -1;
			int width = IsShort ? 2 : 25;
			int height = IsShort ? 2 : 6;
			long result = 0;
			List<List<int>> layers = new List<List<int>>();
			int act = 1;
			List<int> layer = new List<int>();
			foreach (var c in input) {
				layer.Add(c);
				if (act % (width * height) == 0) {
					layers.Add(layer);
					layer = new List<int>();
				}
				act++;
			}
			List<int> image = new List<int>();
			for (int i = 0; i < width * height; i++) {
				for (int l = 0; l < layers.Count; l++) {
					var c = layers[l][i];
					if (c != 2) {
						image.Add(c);
						break;
					}
				}
			}
			for (int i = 1; i < width * height; i++) {
				if (image[i - 1] == 1) {
					Console.BackgroundColor = ConsoleColor.White;
				} else {
					Console.BackgroundColor = ConsoleColor.Black;
				}
				Console.Write(' ');
				if (i % width == 0) Console.WriteLine();
			}
			Console.BackgroundColor = ConsoleColor.Black;
			Console.WriteLine();
			return result;
		}
	}
}
