namespace aoc_runner.Y2023.Day13;

[PuzzleName("Point of Incidence")]
class Solution : ISolver
{
    public object PartOne(string input) => 
        Solve(input, IsMirror);

    public object PartTwo(string input) =>
        Solve(input, IsSmudge);

    int Solve(string input, Func<string[], string[], bool> condition)
    {
        var total = 0;
        foreach (var block in input.Split("\n\n"))
        {
            var mirror = block.Split('\n');

            var row = FindReflection(mirror, condition);
            total += row * 100;

            var col = FindReflection(Rotate90(mirror), condition);
            total += col;
        }
        return total;
    }

    int FindReflection(string[] mirror, Func<string[], string[], bool> condition)
    {
        for (var i = 1; i < mirror.Length; i++)
        {
            var above = mirror[..i];
            var below = mirror[i..];

            above = above.Skip(above.Length - below.Length).ToArray();
            below = below.Reverse().Skip(below.Length - above.Length).ToArray();

            if (condition(above, below))
                return i;
        }
        return 0;
    }

    bool IsMirror(string[] above, string[] below) => 
        above.SequenceEqual(below);

    bool IsSmudge(string[] above, string[] below) =>
        above.Zip(below)
            .SelectMany(pair => pair.First.Zip(pair.Second))
            .Where(c => c.First != c.Second)
            .Count() == 1;

    string[] Rotate90(string[] mirror) =>
        Enumerable.Range(0, mirror[0].Length)
            .Select(x => new string(mirror.Select(y => y[x]).ToArray()))
            .ToArray();
}
