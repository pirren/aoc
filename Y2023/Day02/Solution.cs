using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day02;

[PuzzleName("Cube Conundrum")]
class Solution : ISolver
{
    public object PartOne(string input)
        => Solve(input, PossibleGame);

    public object PartTwo(string input)
        => ParseGames(input).Sum(CubePower);

    int Solve(string input, Func<Game, int> parse)
        => ParseGames(input).Sum(parse);

    int PossibleGame(Game g)
        => g.Red <= 12 && g.Green <= 13 && g.Blue <= 14 ? g.Id : 0;

    int CubePower(Game g) => g.Red * g.Green * g.Blue;

    Game[] ParseGames(string rounds)
        => rounds.Split('\n').Select(set =>
                new Game(
                    Id(set),
                    Color(set, "red"),
                    Color(set, "green"),
                    Color(set, "blue")
                )).ToArray();

    int Color(string indata, string color)
        => (from m in Regex.Matches(indata, @"([0-9]+)(?= " + color + ")") select int.Parse(m.Value)).Max();

    int Id(string indata)
        => (from m in Regex.Matches(indata, @"(\d+)(?=:)") select int.Parse(m.Value)).Single();

    record Game(int Id, int Red, int Green, int Blue);
}
