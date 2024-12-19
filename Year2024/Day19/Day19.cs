using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Year2024.Day19.Day19;

namespace Year2024.Day19 {
	public class Day19 : Solver {
		public Day19() : base(2024, 19) {
		}
		string[] input;
		private Trie trie;
		private HashSet<string> towels;

		//HashSet<string> towels;
		List<string> designs;


		public class TrieNode {
			public Dictionary<char, TrieNode> Children { get; set; }
			public bool IsEndOfWord { get; set; }
			public TrieNode() {
				Children = new Dictionary<char, TrieNode>();
				IsEndOfWord = false;
			}
		}

		public class Trie {
			private readonly TrieNode root;

			public Trie() {
				root = new TrieNode();
			}

			public void Insert(string word) {
				var node = root;
				foreach (var ch in word) {
					if (!node.Children.ContainsKey(ch)) {
						node.Children[ch] = new TrieNode();
					}
					node = node.Children[ch];
				}
				node.IsEndOfWord = true;
			}

			//public bool StartsWith(string prefix) {
			//	var node = root;
			//	foreach (var ch in prefix) {
			//		if (!node.Children.ContainsKey(ch)) {
			//			return false;
			//		}
			//		node = node.Children[ch];
			//	}
			//	return true;
			//}

			public bool Search(string word) {
				var node = root;
				foreach (var ch in word) {
					if (!node.Children.ContainsKey(ch)) {
						return false;
					}
					node = node.Children[ch];
				}
				return true;
			}
		}


		protected override void ReadInputPart1(string fileName) {
			input = File.ReadAllLines(fileName);
			trie = new Trie();
			towels = new HashSet<string>();
			foreach (var t in input[0].Split(',')) {
				//trie.Insert(t.Trim());
				towels.Add(t.Trim());
			}
			designs = new List<string>();
			for (int i = 2; i < input.Length; i++) {
				designs.Add(input[i]);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		private void FillTrie() {
			int maxLen = designs.Max(x => x.Length);
			GenerateWords("", maxLen);
		}
		private void GenerateWords(string s, int maxLen) {
			if (s.Length > 0 && s.Length <= maxLen && designs.Contains(s)) trie.Insert(s);
			if (s.Length >= maxLen) return;
			foreach (var towel in towels) GenerateWords(s + towel, maxLen);
		}


		//naive: 7ms
		//with pregenerated trie: fail
		protected override long SolvePart1() {
			long result = 0;
			FillTrie();
			for (int i = 0; i < designs.Count; i++) {
				var design = designs[i];
				//var usableTowels = towels
				//	.Where(t => design.Contains(t))
				//	.GroupBy(g => g.Length)
				//	.ToDictionary(k => k.Key, v => v.ToList());
				if (trie.Search(design)) {
					Console.WriteLine($"{i+1}/{designs.Count} Found design {design}");
					result++;
				} else {
					Console.WriteLine($"{i+1}/{designs.Count} Failed {design}");
				}
			}
			return result;
		}

		//private bool BuildTowels(string design) {
		//	//Console.WriteLine(design);
		//	for (int i = 1; i <= design.Length; i++) {
		//		var prefix = design.Substring(0, i);
		//		if (trie.StartsWith(prefix)) {
		//			if (trie.Search(prefix) && BuildTowels(design.Substring(i))) {
		//				return true;
		//			}
		//		}
		//	}
		//	return false;
		//}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
