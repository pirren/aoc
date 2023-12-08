using System.Text.RegularExpressions;

namespace aoc_runner.Y2023.Day08;

[PuzzleName("Haunted Wasteland")]
class Solution : ISolver
{
    public object PartOne(string input) 
    {
        return Solve(input);
    }

    public object PartTwo(string input) 
    {
        return SolveTwo(input);
    }

    int Solve(string input)
    {
        var (directions, nodes) = Parse(input);

        var steps = 0;
        var currPos = "AAA";
        while(currPos != "ZZZ")
        {
            var direction = directions[steps % directions.Length];
            steps++;
            currPos = direction == 'L' ? nodes[currPos][0] : nodes[currPos][1];
        }

        return steps;
    }

    int SolveTwo(string input)
    {
        var (directions, nodes) = Parse(input);

        Queue<string> positions = new(nodes.Keys.Where(position => position.EndsWith('A')));
        var tracks = positions.Count;

        for(var steps = 0; steps < int.MaxValue; steps++)
        {
            var direction = directions[steps % directions.Length];
            for (var i = 0; i < positions.Count; i++)
            {
                var current = positions.Dequeue();
                var next = direction == 'L' ? nodes[current][0] : nodes[current][1];
                if(next.EndsWith('Z')) 
                    Console.WriteLine("Z at " + steps);
                positions.Enqueue(next);
            }
            if(positions.All(pos => pos.EndsWith('Z'))) return steps + 1;
        }

        throw new Exception("Not found");

        //foreach(var startPosition in nodes.Keys.Where(position => position.EndsWith('A')))
        //{
        //    var steps = 0;
        //    var currPos = startPosition;
        //    while (!currPos.EndsWith('Z'))
        //    {
        //        var direction = directions[steps % directions.Length];
        //        steps++;
        //        currPos = direction == 'L' ? nodes[currPos][0] : nodes[currPos][1];
        //    }
        //}

    }

    (string directions, Dictionary<string, string[]> map) Parse(string input)
    {
        Dictionary<string, string[]> map = [];
        foreach (var line in input.Split('\n').Skip(2))
        {
            var positions = (from m in Regex.Matches(line, @"(\w+)") select m.Value).ToArray();
            map.Add(positions[0], positions.Skip(1).ToArray());
        }

        return (input.Split('\n')[0], map);
    }

    IEnumerable<int> Directions(string input)
        => from m in Regex.Matches(input, @"\w") select m.Value[0] == 'L' ? 0 : 1;

    record Node(string Self, string Left, string Right);
}
