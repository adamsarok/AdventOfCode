using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day22 {
	public class Day22 : Solver {
		public Day22() : base(2024, 22) {
		}
		List<long> input;
		protected override void ReadInputPart1(string fileName) {
			input =  new List<long>();
			foreach (var l in File.ReadAllLines(fileName)) {
				input.Add(long.Parse(l));
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		long GetNextNum(long n) {
			long secret = n;
			long temp = secret * 64;
			secret = temp ^ secret;
			secret = secret % 16777216;
			temp = secret / 32;
			secret = temp ^ secret;
			secret = secret % 16777216;
			temp = secret * 2048;
			secret = temp ^ secret;
			secret = secret % 16777216;
			return secret;
		}

		protected override long SolvePart1() {
			long result = 0;
			
			foreach (var n in input) {
				var secret = n;
				for (int i = 0; i < 2000; i++) {
					secret = GetNextNum(secret);
					//Console.WriteLine(secret);
				}
				//Console.WriteLine(secret);
				result += secret;
			}
			
			return result;
		}
		record Monke(long init, long[] lastDigits, long[] secrets, long[] diffs, Dictionary<PriceChangeQuad, long> priceChangesQuads);
		record PriceChangeQuad(long a, long b, long c, long d);
		protected override long SolvePart2() {
			List<Monke> monke = new List<Monke>();
			int iter = 2000;
			foreach (var n in input) {
				var m = new Monke(n, new long[iter + 1], new long[iter + 1], new long[iter + 1], new Dictionary<PriceChangeQuad, long>());
				m.secrets[0] = n;
				m.lastDigits[0] = n % 10;
				m.diffs[0] = 0;
				for (int i = 1; i <= iter; i++) {
					m.secrets[i] = GetNextNum(m.secrets[i-1]);
					m.lastDigits[i] = m.secrets[i] % 10;
					m.diffs[i] = m.lastDigits[i] - m.lastDigits[i - 1];
					//Console.WriteLine($"{m.secrets[i]} => {m.lastDigits[i]}: {m.diffs[i]}");
					if (i >= 4) {
						var q = new PriceChangeQuad(m.diffs[i - 3], m.diffs[i - 2], m.diffs[i - 1], m.diffs[i]);
						if (!m.priceChangesQuads.ContainsKey(q)) {
							m.priceChangesQuads.Add(q, m.lastDigits[i]);
						}
					}
				}
				monke.Add(m);
			}
			Dictionary<PriceChangeQuad, long> totals = new Dictionary<PriceChangeQuad, long>();
			foreach (var m in monke) {
				foreach (var kvp in m.priceChangesQuads) {
					if (!totals.ContainsKey(kvp.Key)) {
						totals.Add(kvp.Key, kvp.Value);
					} else {
						totals[kvp.Key] += kvp.Value;
					}
				}
			}
			return totals.Values.OrderByDescending(x => x).First();
		}
	}
}
