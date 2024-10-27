using System;

namespace AdventOfCode;

public class Day7 {
    //A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, 2
    // Five of a kind,
    // Four of a kind,
    // Full house,
    // Three of a kind, 
    // Two pair,
    // One pair, 
    // High card,
    public static void SolvePart1() {
        var lines = File.ReadAllLines("day7input.txt");
        List<string> hands = new List<string>();
        List<int> bids = new List<int>();
        foreach (var line in lines) {
            var s = line.Split(' ');
            hands.Add(s[0]);
            bids.Add(int.Parse(s[1]));
        }
        var buckets = new Buckets(hands);
        int result = 0;
        for (int i = 0; i < buckets.Ranked.Count; i++) {
            int id = buckets.Ranked[i];
            result += (i + 1) * bids[id];
        }
        Console.WriteLine(result);
    }

    public class Buckets {
        List<int> fiveOfAkind = new List<int>();
        List<int> fourOfAkind = new List<int>();
        List<int> fullHouse = new List<int>();
        List<int> threeOfAKind = new List<int>();
        List<int> twoPair = new List<int>();
        List<int> onePair = new List<int>();
        List<int> highCard = new List<int>();
        public List<int> Ranked = new List<int>();
        List<string> hands = new List<string>();
        public Buckets(List<string> hands) {
            this.hands = hands;
            SplitIntoBuckets(hands);
            Rank();
        }
        private void SplitIntoBuckets(List<string> hands) {
            for (int i = 0; i < hands.Count; i++) {
                var hand = hands[i];
                var copies = new Dictionary<char, int>();
                foreach (char card in hand) {
                    if (copies.ContainsKey(card)) copies[card]++;
                    else copies.Add(card, 1);
                }
                var max = copies.Values.Max();
                switch (max) {
                    case 5:
                        fiveOfAkind.Add(i);
                        break;
                    case 4:
                        fourOfAkind.Add(i);
                        break;
                    case 3:
                        if (copies.Values.Any(x => x == 2)) fullHouse.Add(i);
                        else threeOfAKind.Add(i);
                        break;
                    case 2:
                        if (copies.Values.Where(x => x == 2).Count() == 2) twoPair.Add(i);
                        else onePair.Add(i);
                        break;
                    case 1:
                        highCard.Add(i);
                        break;
                }
            }
        }
        private void Rank() {
            //weakest hand = 1
            Ranked.AddRange(RankOneBucket(highCard));
            Ranked.AddRange(RankOneBucket(onePair));
            Ranked.AddRange(RankOneBucket(twoPair));
            Ranked.AddRange(RankOneBucket(threeOfAKind));
            Ranked.AddRange(RankOneBucket(fullHouse));
            Ranked.AddRange(RankOneBucket(fourOfAkind));
            Ranked.AddRange(RankOneBucket(fiveOfAkind));
        }
        Dictionary<char, int> cards = new Dictionary<char, int>() {
            { 'A', 12 },
            { 'K', 11 },
            { 'Q', 10 },
            { 'J', 9 },
            { 'T', 8 },
            { '9', 7 },
            { '8', 6 },
            { '7', 5 },
            { '6', 4 },
            { '5', 3 },
            { '4', 2 },
            { '3', 1 },
            { '2', 0 }
        };
        private List<int> RankOneBucket(List<int> bucket) {
            if (bucket.Count <= 1) return bucket;
            //we have 13 cards, so we can think of cards as digits in base-13
            List<(int, int)> ranksInBucket = new List<(int, int)>();
            foreach (var handId in bucket) {
                var hand = hands[handId];
                int rank = 0;
                int pow = 4;
                foreach (var card in hand) {
                    rank += cards[card] * (int)Math.Pow(13, pow--);
                }
                ranksInBucket.Add((handId, rank));
            }
            var r = ranksInBucket.OrderBy(x => x.Item2);
            return r.Select(x => x.Item1)
                .ToList();
        }
    }
}
