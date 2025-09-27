using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023 {
	public class Day04 : IAocSolver {
		public class Card {
			public int Id { get; set; }
			public int Instances { get; set; }
			public int Matches { get; set; }
			public List<int> WinNums { get; set; } = new List<int>();
			public List<int> MyNums { get; set; } = new List<int>();
		}

		public long SolvePart1(string[] input) {
			long total = 0;
			foreach (var line in input) {
				var s1 = line.Split(':');
				var nums = s1[1];
				var myNums = new List<int>();
				var winNums = new List<int>();
				int matches = 0;
				foreach (var s in nums.Split('|')[0].Split(' ')) {
					if (!string.IsNullOrWhiteSpace(s)) winNums.Add(int.Parse(s));
				}
				foreach (var s in nums.Split('|')[1].Split(' ')) {
					if (!string.IsNullOrWhiteSpace(s)) myNums.Add(int.Parse(s));
				}
				foreach (var win in winNums) {
					if (myNums.Contains(win)) matches++;
				}
				if (matches > 0) total += (int)Math.Pow(2, matches - 1);
			}
			return total;
		}

		public long SolvePart2(string[] input) {
			long total = 0;
			var cards = new Card[input.Length];
			int id = 0;
			foreach (var line in input) {
				var s1 = line.Split(':');
				var nums = s1[1];
				var myNums = new List<int>();
				var winNums = new List<int>();
				int matches = 0;
				foreach (var s in nums.Split('|')[0].Split(' ')) {
					if (!string.IsNullOrWhiteSpace(s)) winNums.Add(int.Parse(s));
				}
				foreach (var s in nums.Split('|')[1].Split(' ')) {
					if (!string.IsNullOrWhiteSpace(s)) myNums.Add(int.Parse(s));
				}
				foreach (var win in winNums) {
					if (myNums.Contains(win)) matches++;
				}
				cards[id] = new Card() {
					Id = id + 1,
					Instances = 1,
					Matches = matches,
					MyNums = myNums,
					WinNums = winNums
				};
				id++;
			}
			for (int i = 0; i < cards.Length; i++) {
				for (int m = 1; m <= cards[i].Matches; m++) {
					cards[i + m].Instances += cards[i].Instances;
				}
			}
			total = cards.Sum(x => x.Instances);
			return total;
		}
	}
}
