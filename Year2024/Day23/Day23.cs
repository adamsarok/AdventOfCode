using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day23 {
	public class Day23 : Solver {
		public Day23() : base(2024, 23) {
		}
		record Computer(string name, List<Computer> connections);
		Dictionary<string, Computer> computers;
		protected override void ReadInputPart1(string fileName) {
			computers = new();
			//ah connected to the other 2 computers
			foreach (var l in File.ReadAllLines(fileName)) {
				var s = l.Split('-');
				var s1 = s[0];
				var s2 = s[1];
				Computer c1;
				if (!computers.TryGetValue(s1, out c1)) {
					c1 = new(s1, new());
					computers.Add(s1, c1);
				}
				Computer c2;
				if (!computers.TryGetValue(s2, out c2)) {
					c2 = new(s2, new());
					computers.Add(s2, c2);
				}
				c1.connections.Add(c2);
				c2.connections.Add(c1);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			HashSet<string> result = new HashSet<string>();
			foreach (var computer in computers.Where(x => x.Key.StartsWith("t"))) {
				var connections = computer.Value.connections;
				for (int i = 0; i < connections.Count; i++) {
					for (int j = i + 1; j < connections.Count; j++) {
						if (connections[i].connections.Contains(connections[j])) {
							var l = new List<string> { computer.Key, connections[i].name, connections[j].name };
							result.Add(string.Join(',', l.OrderBy(x => x)));
						}
					}
				}
			}
			//foreach (var c in result) Console.WriteLine(c);
			return result.Count;
		}

		protected override long SolvePart2() {
			List<HashSet<Computer>> sets = new List<HashSet<Computer>>();
			foreach (var computer in computers) {
				foreach (var conn in computer.Value.connections) {
					sets.Add(new HashSet<Computer> { computer.Value, conn });
				}
			}
			bool foundConn = true;
			while (foundConn) {
				foundConn = false;
				foreach (var set in sets) {
					foreach (var c in computers.Values.Where(x => !set.Contains(x))) {
						if (set.All(x => x.connections.Contains(c))) {
							set.Add(c);
							foundConn = true;
						}
					}
				}
			}
			var max = sets.Max(s => s.Count);
			var s = sets
				.Where(x => x.Count == max)
				.First()
				.ToList();
			Console.WriteLine(string.Join(',', s.Select(x => x.name).OrderBy(o => o)));
			return 0;
		}
	
	}
}
