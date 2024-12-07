using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public class Snippets {
		private static void Combinations(int length, char[] values) {
			if (length < 1 || values.Length < 1) return;
			char[] result = new char[length];
			int combinations = values.Length;
			long all = (long)Math.Pow(combinations, length);
			for (long i = 0; i < all; i++) {
				long temp = i;
				for (int pos = 0; pos < length; pos++) {
					result[pos] = values[temp % combinations];
					temp /= combinations;
				}
				//do something with result
			}
		}
	}
}
