using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Year2019.Day22 {
	public class Day22 : Solver {
		private int deckSize;
		record Move(MoveType MoveType, int value);
		enum MoveType { DealIntoNew, CutN, DealIncrement }
		List<Move> moves;
		public Day22() : base(2019, 22) {
		}
		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			deckSize = IsShort ? 10 : 10007;
			moves = new List<Move>();
			foreach (var l in File.ReadAllLines(fileName)) {
				if (l.StartsWith("deal into")) moves.Add(new Move(MoveType.DealIntoNew, 0));
				else if (l.StartsWith("cut")) moves.Add(new Move(MoveType.CutN, int.Parse(l.Split(" ")[1])));
				else moves.Add(new Move(MoveType.DealIncrement, int.Parse(l.Split(" ")[3])));
			}
		}
		int[] deck;
		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			deck = Enumerable.Range(0, deckSize).ToArray();
			foreach (var move in moves) {
				switch (move.MoveType) {
					case MoveType.DealIntoNew:
						deck = deck.Reverse().ToArray();
						break;
					case MoveType.CutN:
						if (move.value > 0) {
							var top = deck.Take(move.value);
							deck = deck.Skip(move.value).Concat(top).ToArray();
						} else {
							var bott = deck.Skip(deckSize + move.value);
							deck = bott.Concat(deck.Take(deckSize + move.value)).ToArray();
						}
						break;
					case MoveType.DealIncrement:
						var t = new int[deckSize];
						int cnt = 0;
						int index = 0;
						while (cnt < deckSize) {
							t[index] = deck[cnt];
							if (index + move.value < deckSize) index += move.value;
							else {
								index = (move.value + index - deckSize);
							}
							cnt++;
						}
						deck = t;
						break;
				}
			}
			for (int i = 0; i < deckSize; i++) if (deck[i] == 2019) return i;
			return -1;
		}


		protected override long SolvePart2() {
			BigInteger result = 0;
			if (IsShort) return -1;
			//This needed modular inverses which I have not even heard of before
			//Figuring this out by myself seems basically impossible - so this is a translation from a working solution

			BigInteger deckSize = 119315717514047;
			BigInteger shuffles = 101741582076661;
			BigInteger pos = 2020;

			BigInteger incMultiplier = 1;
			BigInteger offsetDiff = 0;

			foreach (var move in moves) {
				switch (move.MoveType) {
					case MoveType.DealIntoNew:
						incMultiplier = -incMultiplier % deckSize;
						offsetDiff = (offsetDiff + incMultiplier) % deckSize;
						break;
					case MoveType.CutN:
						offsetDiff = (offsetDiff + move.value * incMultiplier) % deckSize;
						break;
					case MoveType.DealIncrement:
						incMultiplier = (incMultiplier * ModularArithmetic.ModInverse(move.value, deckSize)) % deckSize;
						break;
				}
			}

			var inc = BigInteger.ModPow(incMultiplier, shuffles, deckSize);

			BigInteger offset =
			(offsetDiff *
			  (1 - inc) *
			  ModularArithmetic.ModInverse((1 - incMultiplier) % deckSize, deckSize)) %
			deckSize;

			BigInteger r2 = (offset + inc * pos) % deckSize;
			return (long)r2;
		}



	}
}
