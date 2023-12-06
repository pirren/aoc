using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day06;

[PuzzleName("Wait For It")]
class Solution : ISolver
{
    public object PartOne(string input) 
    {
        return new RaceCalculator().ParseHistory(input).SeenWaysToWin().Aggregate(1, (a, acc) => a * acc);
    }

    public object PartTwo(string input)
    {
        return new RaceCalculator().ParseCorrectedHistory(input).SeenWaysToWin().Aggregate(1, (a, acc) => a * acc);
    }

    class RaceCalculator
    {
        RaceHistory[] histories = [];

        public RaceCalculator ParseHistory(string input)
        {
            var durations = input.Split('\n')[0].ExtractMultipleRaces();
            var distances = input.Split('\n')[^1].ExtractMultipleRaces();
            histories = durations.Zip(distances).Select(x => new RaceHistory(x.First, x.Second)).ToArray();
            return this;
        }

        public RaceCalculator ParseCorrectedHistory(string input)
        {
            var duration = input.Split('\n')[0].ExtractSingleRaceData();
            var distance = input.Split('\n')[^1].ExtractSingleRaceData();
            histories = [new RaceHistory(duration, distance)];
            return this;
        }

        public IEnumerable<int> SeenWaysToWin()
        {
            foreach(var entry in histories)
            {
                var valid = 0;
                for(int speed = 1; speed <= entry.Duration; speed++)
                {
                    if (speed * (entry.Duration - speed) > entry.RecordTime)
                        valid++;
                }
                yield return valid;
            }
        }
    }

    record RaceHistory(long Duration, long RecordTime);
}

static partial class Ext6
{
    public static IEnumerable<int> ExtractMultipleRaces(this string str)
        => str.NumberMatches().Select(r => int.Parse(r.Value));

    public static long ExtractSingleRaceData(this string str)
    {
        return long.Parse(string.Join("", str.NumberMatches().Select(x => x.Value)));
    }

    private static IEnumerable<Match> NumberMatches(this string str)
        => Numbers().Matches(str).Cast<Match>();

    [GeneratedRegex(@"\d+")]
    private static partial Regex Numbers();
}