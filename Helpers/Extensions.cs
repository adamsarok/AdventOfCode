using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public static class Extensions {
		public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> sequence) {
			if (sequence == null) {
				yield break;
			}

			var list = sequence.ToList();

			if (!list.Any()) {
				yield return Enumerable.Empty<T>();
			} else {
				var startingElementIndex = 0;

				foreach (var startingElement in list) {
					var index = startingElementIndex;
					var remainingItems = list.Where((e, i) => i != index);

					foreach (var permutationOfRemainder in remainingItems.Permute()) {
						yield return permutationOfRemainder.Prepend(startingElement);
					}

					startingElementIndex++;
				}
			}
		}

		public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> sequence, int length) {
			if (sequence == null) {
				yield break;
			}

			var arr = sequence.ToArray();

			if (!arr.Any()) {
				yield return Enumerable.Empty<T>();
			} else {
				T[] result = new T[length];
				int combinations = arr.Length;
				long all = (long)Math.Pow(combinations, length);
				for (long i = 0; i < all; i++) {
					long temp = i;
					for (int pos = 0; pos < length; pos++) {
						result[pos] = arr[temp % combinations];
						temp /= combinations;
					}
					yield return result;
				}
			}
		}


	}
}
