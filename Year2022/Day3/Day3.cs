using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Year2022.Day3 {
	public class Day3 : Solver {
		public Day3() : base(2022, 3) {
		}
		string[] inputs;
		protected override void ReadInputPart1(string fileName) {
			inputs = File.ReadAllLines(fileName);
			//foreach (var l in File.ReadAllLines(fileName)) {

			//}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			foreach (var input in inputs) {
				var ruck1 = input.Substring(0, input.Length / 2)
					.ToCharArray()
					.Distinct()
					.ToList();
				var ruck2 = input.Substring(input.Length / 2, input.Length / 2)
					.ToCharArray()
					.Distinct()
					.ToList();
				char r = ruck1.Intersect(ruck2).First();
				result += GetPrio(r);
			}
			return result;
		}

		private int GetPrio(char r) {
			return char.IsUpper(r) ? r - 38 : r - 96;
		}

		protected override long SolvePart2() {
			long result = 0;
			//same but first 3 lines 
			for (int i = 0; i < inputs.Length; i += 3) {
				List<char> sames = inputs[i]
						.ToCharArray()
						.Distinct()
						.Intersect(inputs[i + 1].ToCharArray().Distinct())
						.Intersect(inputs[i + 2].ToCharArray().Distinct()).ToList();
				
				result += GetPrio(sames.Single());
			}
			return result;
		}
	}
}
