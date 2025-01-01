using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day03 {
	public class Day03 : Solver {
		public Day03() : base(2019, 3) {
		}
		List<string[]> wires;
		protected override void ReadInputPart1(string fileName) {
			wires = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				wires.Add(l.Split(','));
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			List<Vertex> wire1 = new();
			List<Vertex> wire2 = new();
			wire1 = ParseWire(wires[0]);
			wire2 = ParseWire(wires[1]);
			var start = new LVec(0, 0);
			long result = long.MaxValue;
			foreach (var vert1 in wire1) {
				foreach (var vert2 in wire2) {
					var intersection = vert1.Intersection(vert2);
					if (intersection != null) result = Math.Min(result, intersection.ManhattanDistance(start));
				}
			}
			return result;
		}

		private List<Vertex> ParseWire(string[] input) {
			var start = new LVec(0, 0);
			List<Vertex> result = new(); 
			foreach (var i in input) {
				LVec end;
				var dir = i.Substring(0, 1);
				var len = long.Parse(i.Substring(1));
				switch (dir) {
					case "U":
						end = start + new LVec(0, -len);
						break;
					case "D":
						end = start + new LVec(0, len);
						break;
					case "L":
						end = start + new LVec(-len, 0);
						break;
					case "R":
						end = start + new LVec(len, 0);
						break;
					default: throw new Oopsie("unknown direction");
				}
				result.Add(new Vertex(start, end));
				start = end;
			}
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
