namespace aoc_runner.Y2023.Day12;

using Record = (string springs, IEnumerable<int> groups);

[PuzzleName("Hot Springs")]
class Solution : ISolver
{
    public object PartOne(string input)
    {
        return Solve(input);
    }

    public object PartTwo(string input)
    {
        return 0;
    }

    long Solve(string input)
    {
        // (.) operational
        // (#) damaged
        var records = Records(input);

        return 0L;
    }

    Record[] Records(string input) =>
        (from line in input.Split('\n')
         let parts = line.Split(' ')
         let springs = parts[0]
         let groups = parts[1].Split(',').Select(int.Parse)
         select new Record(springs, groups)).ToArray();
}
