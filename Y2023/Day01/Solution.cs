using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day01;

[PuzzleName("Trebuchet?!")]
partial class Solution : ISolver
{
    public object PartOne(string input)
    {
        return Calibrate(input, line => DigitsPattern().Matches(line));
    }

    public object PartTwo(string input)
    {
        return Calibrate(input, line => DigitsSpelledOutPattern().Matches(line));
    }

    private int GetValueFromMatch(Match match)
    {
        // Return the int value
        if (char.IsDigit(match.Value[0])) 
            return match.Value[0] - '0';

        return literalMap.TryGetValue(match.Value, out var result)
            ? result 
            : throw new Exception();
    }

    private Dictionary<string, int> literalMap = new() {
        { "n", 1 }, { "w", 2 }, { "hre", 3 },
        { "ou", 4 }, { "iv", 5 }, { "i", 6},
        { "eve", 7 }, { "igh", 8 }, { "in", 9 }
    };

    private int Calibrate(string input, Func<string, MatchCollection> matchPattern)
    {
        return input.Split('\n')
            .Select(matchPattern)
            .Sum(m => m.Any(x => x.Success) ? (GetValueFromMatch(m[0]) * 10) + GetValueFromMatch(m[^1]) : 0);
    }

    [GeneratedRegex(@"\d")]
    private static partial Regex DigitsPattern();

    [GeneratedRegex(@"(\d)|((?<=n)in(?=e)|(?<=e)igh(?=t)|(?<=s)eve(?=n)|(?<=s)i(?=x)|(?<=f)iv(?=e)|(?<=f)ou(?=r)|(?<=t)hre(?=e)|(?<=t)w(?=o)|(?<=o)n(?=e))")]
    private static partial Regex DigitsSpelledOutPattern();
}
