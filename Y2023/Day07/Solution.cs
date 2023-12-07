namespace aoc_runner.Y2023.Day07;

[PuzzleName("Camel Cards")]
class Solution : ISolver
{
    public object PartOne(string input) 
    {
        var hands = PartOneHands(input);
        return RankHands(hands);
    }

    public object PartTwo(string input)
    {
        var hands = PartTwoHands(input);
        return RankHands(hands); 
    }

    int RankHands(List<Hand> hands) 
    {
        hands.Sort(); 
        return hands.Select((hand, idx) => hand.Bid * (idx + 1)).Sum();
    }

    List<Hand> PartOneHands(string input) => (
                from line in input.Split('\n')
                let parts = line.Split(' ')
                let hand = parts[0]
                let bid = int.Parse(parts[^1])

                let cardGroups = GroupCards(hand)

                select new Hand(hand, bid, cardGroups)
            ).ToList();

    List<Hand> PartTwoHands(string input) => (
                from line in input.Split('\n')
                let parts = line.Split(' ')
                let hand = parts[0]
                let bid = int.Parse(parts[^1])

                let cardGroups = GroupCardsAndJokers(hand)

                select new Hand(hand, bid,  cardGroups, true)
            ).ToList();

    List<int> GroupCards(string indata)
        => (from card in indata
           group card by card into cg
           select cg.Count()).ToList();

    List<int> GroupCardsAndJokers(string indata)
    {
        var groups = (from card in indata
                      group card by card into cg
                      select cg.Where(g => g != 'J').Count()).ToList();

        // Add jokers to the largest seen group
        var largestGroup = groups.Max();

        groups.Remove(largestGroup);
        groups.Add(largestGroup + indata.Count(c => c == 'J'));

        return groups;
    }

    class Hand : IComparable<Hand>
    {
        Dictionary<char, int> CardValue = new()
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

            if (groups.Contains(5))
                PatternValue = PatternValue.Five;
            else if (groups.Contains(4))
                PatternValue = PatternValue.Four;
            else if (groups.Contains(3) && groups.Contains(2))
                PatternValue = PatternValue.FullHouse;
            else if (groups.Contains(3))
                PatternValue = PatternValue.Three;
            else if (groups.Where(count => count == 2).Count() == 2)
                PatternValue = PatternValue.TwoPair;
            else if (groups.Contains(2))
                PatternValue = PatternValue.Pair;
        }

        IEnumerable<int> CardValues => Cards.Select(c => CardValue[c]);
        public string Cards { get; init; }
        public int Bid { get; init; }
        public PatternValue PatternValue { get; init; } = PatternValue.High;

        public int CompareTo(Hand? other)
        {
            if (other is null) return 1;
            if (PatternValue > other.PatternValue) return 1;
            if (other.PatternValue > PatternValue) return -1;

            var values = CardValues.ToArray();
            var otherValues = other.CardValues.ToArray();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > otherValues[i]) return 1;
                if (values[i] < otherValues[i]) return -1;
            }
            return 0;
        }
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
