using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day02;

[PuzzleName("Cube Conundrum")]
class Solution : ISolver
{
    public object PartOne(string input)
    {
        return ParseGames(input).Sum(GamePossible);
    }

    public object PartTwo(string input) 
    {
        return ParseGames(input).Sum(CubePower);
    }

    private int GamePossible(Game g)
    {
        return g.HighestRed <= 12 && g.HighestGreen <= 13 && g.HighestBlue <= 14 ? g.Id : 0;
    }

    private int CubePower(Game g)
    {
        return g.HighestRed * g.HighestGreen * g.HighestBlue;
    }

    private IEnumerable<Game> ParseGames(string rounds)
    {
        foreach (var set in rounds.Split('\n'))
        {
            yield return new Game(
                    int.Parse(set.Split(':')[0].Split(' ')[^1]), 
                    HighestSeenColor(set, "red"), 
                    HighestSeenColor(set, "green"), 
                    HighestSeenColor(set, "blue")
                );
        }
    }

    private int HighestSeenColor(string set, string color)
        => Regex.Matches(set, @"([0-9]+)(?= " + color + ")").Max(cubes => int.Parse(cubes.Value));

    record Game(int Id, int HighestRed, int HighestGreen, int HighestBlue);
}
