using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using OrbitsTree = Helpers.Tree<string>;

namespace Year2019.Day06 {
	public class Day06 : Solver {
		public Day06() : base(2019, 6) {
		}
		Dictionary<string, string> orbits;
		OrbitsTree tree;
		Dictionary<string, Tree<string>> treeElements;
		protected override void ReadInputPart1(string fileName) {
			orbits = new();
			foreach (var l in File.ReadAllLines(fileName)) {
				var s = l.Split(')');
				orbits.Add(s[1], s[0]);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var kvp in orbits) {
				var next = kvp.Value;
				result++;
				while (orbits.ContainsKey(next)) {
					next = orbits[next];
					result++;
				}
			}
			return result;
		}

		protected override long SolvePart2() {
			tree = new OrbitsTree("COM");
			treeElements = new Dictionary<string, OrbitsTree>() { { "COM", tree } };
			BuildTree(tree);
			var temp1 = Path(new List<OrbitsTree>() { treeElements["YOU"] }, treeElements["COM"]).Select(x => x.value).ToList();
			var temp2 = Path(new List<OrbitsTree>() { treeElements["SAN"] }, treeElements["COM"]).Select(x => x.value).ToList();
			var intersection = temp1.Where(x => temp2.Contains(x)).First();
			var p1 = Path(new List<OrbitsTree>() { treeElements["YOU"] }, treeElements[intersection]).Select(x => x.value).ToList();
			var p2 = Path(new List<OrbitsTree>() { treeElements["SAN"] }, treeElements[intersection]).Select(x => x.value).ToList();
			return p1.Count + p2.Count - 2;
		}

		private List<OrbitsTree> Path(List<OrbitsTree> from, OrbitsTree to) {
			var first = from.Last();
			if (first.Parent.value == to.value) return from;
			return Path(new List<OrbitsTree>(from).Append(first.Parent).ToList(), to);
		}

		private void BuildTree(OrbitsTree tree) {
			var children = orbits.Where(x => x.Value == tree.value).Select(x => x.Key).ToList();
			foreach (var c in children) {
				var cTree = new Tree<string>(c);
				treeElements.Add(cTree.value, cTree);
				cTree.Parent = tree;
				tree.Children.Add(cTree);
				BuildTree(cTree);
			}
		}

		
	}
}
