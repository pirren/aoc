namespace aoc_runner.Y2023.Day04;

[PuzzleName("Scratchcards")]
class Solution : ISolver
{
    public object PartOne(string input) 
    {
        return new CardStack(input).GetCards().Sum(card => card.Points());
    }

    public object PartTwo(string input) 
    {
        return new CardStack(input).WinMore().CountCards();
    }

    class CardStack
    {
        readonly Dictionary<int, int> stack = [];
        readonly List<ScratchCard> cards = [];

        public CardStack(string input)
        {
            var lines = input.Split('\n');
            for(int i = 0; i < lines.Length; i++)
            {
                var winning = lines[i].Split('|')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(2).Select(int.Parse);
                var have = lines[i].Split('|')[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

                stack.Add(i, 1);
                cards.Add(new ScratchCard(i, winning, have.ToArray()));
            }
        }

        public IEnumerable<ScratchCard> GetCards() => cards;

        public CardStack WinMore()
        {
            foreach(var id in stack.Keys)
            {
                var points = cards[id].MatchingNumbers();
                for(var p = id + 1; p <= id + points; p++)
                {
                    if (stack.TryGetValue(p, out int value))
                        stack[p] = value + (1 * stack[id]);
                }
            }

            return this;
        }

        public int CountCards() => stack.Sum(x => x.Value);
    }

    record ScratchCard(int Id, IEnumerable<int> WinningNumbers, int[] Numbers)
    {
        public int Points()
        {
            var points = 0;
            foreach(var target in WinningNumbers)
            {
                if (Numbers.Any(number => number == target))
                    points = points == 0 ? 1 : points * 2;
            }
            return points;
        }

        public int MatchingNumbers()
        {
            var matches = 0;
            foreach (var target in WinningNumbers)
            {
                if (Numbers.Any(number => number == target))
                    matches++;
            }
            return matches;
        }
    };
}
