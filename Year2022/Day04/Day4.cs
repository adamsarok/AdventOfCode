using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Year2022.Day4 {
	public class Day4 : Solver {
		public Day4() : base(2022, 4) {
		}
		List<(XY, XY)> pairs;
		protected override void ReadInputPart1(string fileName) {
			pairs = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				var s = l.Split(",");
				pairs.Add((Split(s[0]), Split(s[1])));
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);	
		}
		
		private bool FullyContains(XY a, XY b) {
			return 
				(a.x <= b.x && a.y >= b.y)
				|| (b.x <= a.x && b.y >= a.y);
		}

		private bool Overlaps(XY a, XY b) {
			return  a.x <= b.y && b.x <= a.y;
		}

		private XY Split(string s) {
			var p = s.Split("-");
			return new XY(long.Parse(p[0]), long.Parse(p[1]));
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var p in pairs) {
				if (FullyContains(p.Item1, p.Item2)) {
					result++;
				}
			}
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;
			foreach (var p in pairs) {
				if (Overlaps(p.Item1, p.Item2)) {
					result++;
				}
			}
			return result;
		}
	}
}
