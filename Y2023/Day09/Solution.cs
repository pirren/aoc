using System.Text.RegularExpressions;

using Sequence = System.Collections.Generic.List<long>;
using Sequences = System.Collections.Generic.List<System.Collections.Generic.List<long>>;

namespace aoc_runner.Y2023.Day09;

[PuzzleName("Mirage Maintenance")]
class Solution : ISolver
{
    public object PartOne(string input)
        => Solve(input, Forwards);

    public object PartTwo(string input)
        => Solve(input, Backwards);

    long Solve(string input, Func<Sequences, long> predict)
       => predict(Sequences(input));

    long Forwards(Sequences sequences)
        => sequences
            .Sum(seq => Extrapolate(seq).Select(seq => seq.Last()).Aggregate((a, b) => a + b));

    long Backwards(Sequences sequences)
        => sequences
            .Sum(seq => Extrapolate(seq).Select(seq => seq.First()).Reverse().Aggregate((a, b) => b - a));

    Sequences Extrapolate(Sequence sequence)
    {
        Sequences sequences = [[.. sequence]];
        Sequence differences = [];

        while (differences.Count == 0 || !differences.All(number => number == 0))
        {
            Sequence next = sequences[^1];
            differences = [];
            foreach (var (first, second) in next.Zip(next[1..]))
            {
                differences.Add(second - first);
            }
            sequences.Add(differences);
        } 

        return sequences;
    }

    Sequences Sequences(string input) => 
        [.. input.Split('\n').Select(line => Regex.Matches(line, @"-?(\d+)").Select(m => long.Parse(m.Value)).ToList()) ];
}
