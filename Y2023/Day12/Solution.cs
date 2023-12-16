namespace aoc_runner.Y2023.Day12;

using System;

[PuzzleName("Hot Springs")]
class Solution : ISolver
{
    public object PartOne(string input)
    {
        return Solve(input, 1);
    }

    public object PartTwo(string input)
    {
        return Solve(input, 5);
    }

    long Solve(string input, int timesFolded)
    {
        var records = Records(input, timesFolded);
        long sum = 0L;

        foreach (var (variations, sequence, i) in records.Select((record, idx) => (Variations(record.Parts), record.Groups, idx)))
        {
            sum += variations.Count(variation =>
                variation.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(parts =>
                    parts.Length).SequenceEqual(sequence));
            Console.WriteLine($"Processed {i + 1} row(s). Sum = {sum}.");
        }

        return sum;
    }

    IEnumerable<string> Variations(string row)
    {
        if (row.Length == 0)
        {
            yield return "";
            yield break;
        }

        var (next, rest) = (row[0], row[1..]);

        foreach (var variation in Variations(rest))
        {
            yield return next == '?' ? $".{variation}" : $"{next}{variation}";
            if (next == '?') yield return $"#{variation}";
        }
    }

    Record[] Records(string input, int timesFolded)
    {
        return (from line in input.Split('\n')
                let blocks = line.Split(' ')
                let parts = blocks[0]
                let groups = blocks[1].Split(',').Select(int.Parse)
                select new Record(
                    string.Concat(Enumerable.Repeat(parts, timesFolded)), 
                    Enumerable.Repeat(groups, timesFolded).SelectMany(x => x).ToList()
                    )
                ).ToArray();
    }

    record Record(string Parts, List<int> Groups);
}
