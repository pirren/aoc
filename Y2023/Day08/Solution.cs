using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day08;

[PuzzleName("Haunted Wasteland")]
class Solution : ISolver
{
    public object PartOne(string input) 
        => Solve(input, (current) => current == "AAA", (current) => current == "ZZZ");

    public object PartTwo(string input) 
        => Solve(input, (current) => current.EndsWith('A'), (current) => current.EndsWith('Z'));

    long Solve(string input, Func<string, bool> starting, Predicate<string> finished)
    {
        var directions = Directions(input);
        var map = Map(input);

        // Each 'track' repeats in length after reaching first end node

        HashSet<long> cycles = [.. map.Keys.Where(starting).Select(start => Traverse(start, directions, map, finished))];
        return cycles.Aggregate(LCM);
    }

    long Traverse(string currentLocation, byte[] directions, Dictionary<string, string[]> map, Predicate<string> finished)
    {
        var length = 0;

        while (!finished(currentLocation))
        {
            var direction = directions[length % directions.Length];
            currentLocation = map[currentLocation][direction];
            length++;
        }

        return length;
    }

    long GFC(long a, long b) => b == 0 ? a : GFC(b, a % b);

    long LCM(long a, long b) => a / GFC(a, b) * b;

    byte[] Directions(string input)
        => (from m in Regex.Matches(input.Split('\n')[0], @"\w") 
            select (byte)(m.Value[0] == 'L' ? 0 : 1))
        .ToArray();

    Dictionary<string, string[]> Map(string input)
        => (from line in input.Split('\n').Skip(2)
            let positions = (from m in Regex.Matches(line, @"(\w+)") select m.Value).ToArray()
            select (positions[0], positions.Skip(1).ToArray()))
        .ToDictionary();
}
