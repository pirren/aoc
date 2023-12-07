using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day06;

[PuzzleName("Wait For It")]
class Solution : ISolver
{
    public object PartOne(string input)
        => Solve(input, PartOneHistories);

    public object PartTwo(string input)
        => Solve(input, PartTwoHistories);

    int Solve(string input, Func<string, RaceHistory[]> parse)
        => parse(input).Select(ValidStrategies).Aggregate(1, (acc, value) => acc * value);

    int ValidStrategies(RaceHistory history)
        => Enumerable.Range(1, (int)history.Time)
            .Count(speed => speed * (history.Time - speed) > history.Distance);

    RaceHistory[] PartOneHistories(string input)
    {
        var parts = input.Split('\n');
        var times = Numbers(parts[0], "Time");
        var distances = Numbers(parts[^1], "Distance");
        return times.Zip(distances).Select(x => new RaceHistory(x.First, x.Second)).ToArray();
    }

    RaceHistory[] PartTwoHistories(string input)
    {
        var parts = input.Split('\n');
        var time = Number(parts[0], "Time");
        var distance = Number(parts[^1], "Distance");
        return [new RaceHistory(time, distance)];
    }

    IEnumerable<int> Numbers(string input, string selector)
        => input.Replace(selector + ": ", string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse);

    long Number(string input, string selector)
        => long.Parse(string.Concat(input.Replace(selector + ": ", string.Empty).Where(char.IsNumber)));

    record RaceHistory(long Time, long Distance);
}
