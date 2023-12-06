using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day06;

[PuzzleName("Wait For It")]
class Solution : ISolver
{
    public object PartOne(string input) 
        => PartOneHistories(input).Select(StrategiesForWin).Aggregate(1, CalculateErrorMargin);

    public object PartTwo(string input)
        => PartTwoHistories(input).Select(StrategiesForWin).Aggregate(1, CalculateErrorMargin);

    int CalculateErrorMargin(int accumulator, int value)
        => accumulator * value;

    int StrategiesForWin(RaceHistory history)
    {
        var valid = 0;
        for (int speed = 1; speed <= history.Duration; speed++)
        {
            if (speed * (history.Duration - speed) > history.RecordTime)
                valid++;
        }
        return valid;
    }

    RaceHistory[] PartOneHistories(string input)
    {
        var parts = input.Split('\n');
        var durations = parts[0].MultipleRaceData();
        var distances = parts[^1].MultipleRaceData();
        return durations.Zip(distances).Select(x => new RaceHistory(x.First, x.Second)).ToArray();
    }

    RaceHistory[] PartTwoHistories(string input)
    {
        var parts = input.Split('\n');
        var duration = parts[0].SingleRaceData();
        var distance = parts[^1].SingleRaceData();
        return [new RaceHistory(duration, distance)];
    }

    record RaceHistory(long Duration, long RecordTime);
}

static partial class Ext6
{
    public static IEnumerable<int> MultipleRaceData(this string str)
        => str.Numbers().Select(r => int.Parse(r.Value));

    public static long SingleRaceData(this string str)
        => long.Parse(string.Join("", str.Numbers().Select(x => x.Value)));

    private static IEnumerable<Match> Numbers(this string str)
        => Numbers().Matches(str).Cast<Match>();

    [GeneratedRegex(@"\d+")]
    private static partial Regex Numbers();
}