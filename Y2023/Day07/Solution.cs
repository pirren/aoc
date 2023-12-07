namespace aoc_runner.Y2023.Day07;

[PuzzleName("Camel Cards")]
class Solution : ISolver
{
    public object PartOne(string input) 
    {
        var hands = ParseHands(input, PartOnePairings, false);
        return Winnings(hands);
    }

    public object PartTwo(string input)
    {
        var hands = ParseHands(input, PartTwoPairings, true);
        return Winnings(hands); 
    }

    List<Hand> ParseHands(string input, Func<string, List<int>> parse, bool useJokers)
    {
        return (
            from line in input.Split('\n')
            let parts = line.Split(' ')
            let hand = parts[0]
            let bid = int.Parse(parts[^1])

            let cardGroups = parse(hand)

            select new Hand(hand, bid, cardGroups, useJokers)
        ).ToList();
    }

    int Winnings(List<Hand> hands) 
    {
        hands.Sort(); 
        return hands.Select((hand, idx) => hand.Bid * (idx + 1)).Sum();
    }

    List<int> PartOnePairings(string indata)
        => indata.GroupBy(card => card).Select(cg => cg.Count()).ToList();

    List<int> PartTwoPairings(string indata)
    {
        var groups = indata.GroupBy(card => card)
            .Select(cg => cg.Where(g => g != 'J').Count()).ToList();

        var largestGroup = groups.Max();
        groups.Remove(largestGroup);
        groups.Add(largestGroup + indata.Count(c => c == 'J'));

        return groups;
    }

    record Hand : IComparable<Hand>
    {
        private static Dictionary<char, int> CardValue = new()
        {
            { '2', 2 }, { '3', 3 }, { '4', 4 }, { '5', 5 }, { '6', 6 }, 
            { '7', 7 }, { '8', 8 }, { '9', 9 }, { 'T', 10 }, { 'J', 11 }, 
            { 'Q', 12 }, { 'K', 13 }, { 'A', 14 }
        };

        public Hand(string cards, int bid, List<int> groups, bool jokerRules = false)
        {
            Cards = cards;
            Bid = bid;

            if (jokerRules)
                CardValue['J'] = -1;

            PatternValue = groups switch
            {
                _ when groups.Contains(5) => PatternValue.Five,
                _ when groups.Contains(4) => PatternValue.Four,
                _ when groups.Contains(3) && groups.Contains(2) => PatternValue.FullHouse,
                _ when groups.Contains(3) => PatternValue.Three,
                _ when groups.Count(count => count == 2) == 2 => PatternValue.TwoPair,
                _ when groups.Contains(2) => PatternValue.Pair,
                _ => PatternValue.High
            };
        }

        public int CompareTo(Hand? other)
        {
            if (other is null) return 1;

            var pc = PatternValue.CompareTo(other.PatternValue);
            if (pc != 0) return pc;

            return CardValues.Zip(other.CardValues, (thisVal, otherVal) => thisVal.CompareTo(otherVal))
                .FirstOrDefault(result => result != 0);
        }

        IEnumerable<int> CardValues => Cards.Select(c => CardValue[c]);
        public string Cards { get; init; }
        public int Bid { get; init; }
        public PatternValue PatternValue { get; init; } = PatternValue.High;
    }

    enum PatternValue
    {
        High,
        Pair,
        TwoPair,
        Three,
        FullHouse,
        Four,
        Five
    }
}
