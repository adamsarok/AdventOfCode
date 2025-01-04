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

		public static IEnumerable<T[]> SetCombinations<T>(this IEnumerable<T> source) {
			if (null == source)
				throw new ArgumentNullException(nameof(source));

			T[] data = source.ToArray();

			return Enumerable
			  .Range(0, 1 << (data.Length))
			  .Select(index => data
				 .Where((v, i) => (index & (1 << i)) != 0)
				 .ToArray());
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}

			Random rng = new Random();
			T[] elements = source.ToArray();
			for (int i = elements.Length - 1; i > 0; i--) {
				int swapIndex = rng.Next(i + 1);
				T temp = elements[i];
				elements[i] = elements[swapIndex];
				elements[swapIndex] = temp;
			}
			return elements;
		}
	}
}
