using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day04;

[PuzzleName("Scratchcards")]
class Solution : ISolver
{
    public object PartOne(string input)
        => Solve(input, CountPoints);

    public object PartTwo(string input)
        => Solve(input, CountCards);

    int Solve(string input, Func<Dictionary<Card, int>, int> count)
    {
        var cards = Cards(input);
        return count(cards);
    }

    int CountPoints(Dictionary<Card, int> cards)
        => cards.Keys.Sum(card => card.Points());

    int CountCards(Dictionary<Card, int> cards)
    {
        var orderedCards = cards.Keys.ToArray();
        for (var i = 0; i < orderedCards.Length; i++)
        {
            var current = orderedCards[i];
            var points = current.Matched.Length;
            for (var j = current.Id + 1; j <= current.Id + points && j < orderedCards.Length; j++)
            {
                var nextCard = orderedCards[j];
                cards[nextCard] += cards[current];
            }
        }

        return cards.Sum(tcard => tcard.Value);
    }

    Dictionary<Card, int> Cards(string input)
        => (from line in input.Split('\n').Select((card, idx) => (card, idx))
            let allNumbers = line.card.Split(':')[^1].Split('|')
            let cardNumbers = Numbers(allNumbers[0])
            let winningNumbers = Numbers(allNumbers[^1])
            let matched = cardNumbers.Intersect(winningNumbers).ToArray()
            select new KeyValuePair<Card, int>(new Card(line.idx, matched), 1))
        .ToDictionary();

    IEnumerable<int> Numbers(string indata)
        => from m in Regex.Matches(indata, @"(\d+)") select int.Parse(m.Value);

    record Card(int Id, int[] Matched)
    {
        public int Points() => (int)Math.Pow(2, Matched.Length - 1);
    }
}
