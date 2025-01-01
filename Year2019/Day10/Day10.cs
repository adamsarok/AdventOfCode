using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day10 {
	public class Day10 : Solver {
		public Day10() : base(2019, 10) {
		}
		enum Tile { Empty, UnknownAsteroid, BlockedAsteroid, CanSeeAsteroid, MonitoringStation }

		Tile[,] tiles;
		int width, height;
		protected override void ReadInputPart1(string fileName) {
			var input = File.ReadAllLines(fileName);
			width = input[0].Length;
			height = input.Length;
			tiles = new Tile[width, height];
			for (int y = 0; y < height; y++) {
				var l = input[y];
				for (int x = 0; x < width; x++) {
					tiles[x, y] = l[x] == '#' ? Tile.UnknownAsteroid : Tile.Empty;
				}
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (tiles[x, y] != Tile.UnknownAsteroid) continue;
					//int x = 5, y = 8;

					tiles[x, y] = Tile.MonitoringStation;
					int range = 1; //go in square of increasing size
					bool canProcess = false;
					do {
						//Debug();
						canProcess = ProcessSquare(new Vec(x, y), range);
						range++;
					} while (canProcess);
					result = Math.Max(result, GetScore());
					Reset();
				}
			}
			return result;
		}

		long GetScore() {
			long result = 0;
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					result += tiles[x, y] switch {
						Tile.CanSeeAsteroid => 1,
						_ => 0,
					};
				}
			}
			return result;
		}

		private void Debug() {
			Console.Clear();
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					char c = tiles[x, y] switch {
						Tile.Empty => '.',
						Tile.UnknownAsteroid => '#',
						Tile.BlockedAsteroid => 'B',
						Tile.CanSeeAsteroid => 'C',
						Tile.MonitoringStation => 'M',
						_ => throw new NotImplementedException()
					};
					Console.Write(c);
				}
				Console.WriteLine();
			}
		}

		private bool ProcessSquare(Vec pos, int range) {
			bool isChanged = false;
			for (int x = pos.x - range; x <= pos.x + range; x++) {
				if (Process(pos, new Vec(x, pos.y - range))) isChanged = true;
				if (Process(pos, new Vec(x, pos.y + range))) isChanged = true;
			}
			for (int y = pos.y - range; y <= pos.y + range; y++) {
				if (Process(pos, new Vec(pos.x - range, y))) isChanged = true;
				if (Process(pos, new Vec(pos.x + range, y))) isChanged = true;
			}
			return isChanged;
		}

		Tile? TryGet(Vec vec) {
			if (vec.x < 0 || vec.x >= width || vec.y < 0 || vec.y >= height) return null;
			return tiles[vec.x, vec.y];
		}

		private bool Process(Vec startingPoint, Vec target) {
			var tile = TryGet(target);
			if (tile == null) return false;
			if (tile == Tile.UnknownAsteroid) { //set blocked every X tiles away
				tiles[target.x, target.y] = Tile.CanSeeAsteroid;
				var nextStep = target + (target - startingPoint);
				//haha, this is failing because line of sight is not calculated by repeating the vector as in 2024
				//eg M...A## -> the two asteroids behind A are all blocked
				var toBlock = TryGet(nextStep);
				while (toBlock != null) {
					if (toBlock == Tile.UnknownAsteroid) tiles[nextStep.x, nextStep.y] = Tile.BlockedAsteroid;
					nextStep = nextStep + (target - startingPoint);
					toBlock = TryGet(nextStep);
				}
			}
			return true;
		}

		private void Reset() {
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					var t = tiles[x, y];
					if (t == Tile.CanSeeAsteroid || t == Tile.BlockedAsteroid || t == Tile.MonitoringStation) tiles[x, y] = Tile.UnknownAsteroid;
				}
			}
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
