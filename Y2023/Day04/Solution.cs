using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day04;

[PuzzleName("Scratchcards")]
class Solution : ISolver
{
    public object PartOne(string input) 
    {
        return new CardStack(input).CountPoints();
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
            for (int i = 0; i < lines.Length; i++)
            {
                var numbers = lines[i].Split(':')[^1].Split('|');

                var winningNumbers = numbers[0]
                    .ExtractNumbers()
                    .Intersect(numbers[^1].ExtractNumbers());

                stack.Add(i, 1);
                cards.Add(new ScratchCard(i, winningNumbers.ToArray()));
            }
        }

        public CardStack WinMore()
        {
            foreach(var id in stack.Keys)
            {
                var points = cards[id].WinningNumbers.Length;
                for(var idNext = id + 1; idNext <= id + points; idNext++)
                {
                    if (stack.ContainsKey(idNext))
                        stack[idNext] += stack[id];
                }
            }

            return this;
        }

        public int CountPoints() => cards.Sum(card => card.Points());
        
        public int CountCards() => stack.Sum(tcard => tcard.Value);
    }

    record ScratchCard(int Id, int[] WinningNumbers)
    {
        public int Points()
            => WinningNumbers.Length == 0
                ? 0 : (int)Math.Pow(2, WinningNumbers.Length - 1);
    };
}

internal static partial class Ext4
{
    public static IEnumerable<int> ExtractNumbers(this string data)
        => NumberPattern().Matches(data).Select(match => int.Parse(match.Value));

    [GeneratedRegex(@"(\d+)")]
    private static partial Regex NumberPattern();
}
