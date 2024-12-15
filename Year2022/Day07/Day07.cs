using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2022.Day07 {
	public class Day07 : Solver {
		public Day07() : base(2022, 7) {
		}
		record class Directory(string name, Directory? parent) { 
			public List<Directory> Children { get; set; } = new();
			public List<long> Files { get; set; } = new();
			public Directory? Parent => parent;
		}
		Directory root { get; set; }
		protected override void ReadInputPart1(string fileName) {
			root = new Directory("/", null);
			var currDir = root;
			int i = 0;
			var input = File.ReadAllLines(fileName);
			while (i < input.Length) {
				var l = input[i];
				switch (l) {
					case "$ cd /":
						i++;
						currDir = root;
						break;
					case "$ cd ..":
						i++;
						currDir = currDir.Parent;
						break;
					case "$ ls":
						i++;
						ParseLSResults(currDir, input, ref i);
						break;
					default:
						throw new Oopsie($"Unknown command {l}");
				}
			}
		}

		private void ParseLSResults(Directory? currDir, string[] input, ref int i) {
			while (i < input.Length && !input[i].StartsWith("$")) {
				var l = input[i];
				if (l.StartsWith("dir")) {
					currDir.Children.Add(new Directory(l.Split(" ")[1], currDir));
				} else {
					currDir.Files.Add(long.Parse(l.Split(" ")[0]));
				}
				i++;
			}
		}
		protected override void ReadInputPart2(string fileName) {
			//input = new();
			foreach (var l in File.ReadAllLines(fileName)) {

			}
		}

		protected override long SolvePart1() {
			long result = 0;
			//foreach (var dir in)
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
