namespace aoc_runner.Y2023.Day07;

[PuzzleName("Camel Cards")]
class Solution : ISolver
{
    public object PartOne(string input) 
        => Solve(input, false);

    public object PartTwo(string input)
        => Solve(input, true);

    int Solve(string input, bool jokers)
    {
        var hands = GetHands(input, jokers);
        return Winnings(hands);
    }

    int Winnings(List<Hand> hands)
    {
        hands.Sort();
        return hands.Select((hand, idx) => hand.Bid * (idx + 1)).Sum();
    }

    List<Hand> GetHands(string input, bool jokers) =>
        (from line in input.Split('\n')
            let parts = line.Split(' ')
            let hand = parts[0]
            let bid = int.Parse(parts[^1])
            let cardGroups = jokers ? JokerRules(hand) : StandardRules(hand)
            select new Hand(hand, bid, cardGroups, jokers))
        .ToList();

    List<int> StandardRules(string indata) => 
        indata.GroupBy(card => card).Select(cg => cg.Count()).ToList();

    List<int> JokerRules(string indata)
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
        private static Dictionary<char, int> Values = new() { { 'T', 10 }, { 'J', 11 }, { 'Q', 12 }, { 'K', 13 }, { 'A', 14 } };

        public Hand(string cards, int bid, List<int> groups, bool jokerRules = false)
        {
            Cards = cards;
            Bid = bid;

            if (jokerRules) Values['J'] = -1;

            PatternValue = groups switch
            {
                _ when groups.Contains(5) => 6,
                _ when groups.Contains(4) => 5,
                _ when groups.Contains(3) && groups.Contains(2) => 4,
                _ when groups.Contains(3) => 3,
                _ when groups.Count(count => count == 2) == 2 => 2,
                _ when groups.Contains(2) => 1,
                _ => 0
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

        IEnumerable<int> CardValues => 
            Cards.Select(c => char.IsNumber(c) ? c - '0' : Values[c]);

        public string Cards { get; init; }
        public int Bid { get; init; }
        public int PatternValue { get; init; } = 0;
    }
}
