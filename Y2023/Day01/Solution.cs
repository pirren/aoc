using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day01;

[PuzzleName("Trebuchet?!")]
partial class Solution : ISolver
{
    public object PartOne(string input)
        => Solve(input, PartOneValues);

    public object PartTwo(string input)
        => Solve(input, PartTwoValues);

    int Solve(string input, Func<string, IEnumerable<(int, int)>> values)
        => values(input).Sum(values => values.Item1 * 10 + values.Item2);

    IEnumerable<(int, int)> PartOneValues(string input)
        => input.Split('\n').Select(line => Regex.Matches(line, @"\d"))
            .Select(Digits);
    IEnumerable<(int, int)> PartTwoValues(string input)
        => input.Split('\n')
            .Select(ReplaceLiteralNumbers)
            .Select(line => Regex.Matches(line, @"\d"))
            .Select(Digits);

    (int, int) Digits(MatchCollection matches)
        => ((matches[0].Value[0] - '0'), matches[^1].Value[0] - '0');

    string ReplaceLiteralNumbers(string input)
        => remapper.Aggregate(input, (current, pairs) => current.Replace(pairs.Key, pairs.Value));

    Dictionary<string, string> remapper = new() {
        { "one", "on1e" }, { "two", "t2wo" }, { "three", "thr3ee" },
        { "four", "fo4ur" }, { "five", "fi5ve" }, { "six", "si6x"},
        { "seven", "sev7en" }, { "eight", "ei8ght" }, { "nine", "ni9ne" }
    };
}
