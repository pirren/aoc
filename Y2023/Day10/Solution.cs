namespace aoc_runner.Y2023.Day10;

using Map = Dictionary<(int x, int y), char>;

[PuzzleName("Pipe Maze")]
class Solution : ISolver
{
    public object PartOne(string input) 
    {
        return Solve(input);
    }

    public object PartTwo(string input)
    {
        // Todo
        return 0; 
    }

    Dictionary<char, (int x, int y)[]> Directions = new() { 
        { '|', [(0, 1), (0, -1)] }, 
        { '-', [(1, 0), (-1, 0)] },
        { 'L', [(0, -1), (1, 0)] },
        { 'J', [(-1, 0), (0, -1)] },
        { '7', [(-1, 0), (0, 1)] },
        { 'F', [(1, 0), (0, 1)] },
        { '.', [] }
    };

    int Solve(string input)
    {
        var data = input.Split('\n');
        var map = GetMap(data);

        var start = map.FirstOrDefault(position => position.Value == 'S').Key;
        var direction = StartingDirections(start, data[0].Length, data.Length, map).First();

        var at = start;
        var steps = 0;
        do
        {
            steps++;
            var next = Next(at, direction);
            var charAt = map[next];

            if (charAt == 'S') break;

            if (charAt != '|' && charAt != '-')
            {
                var ndir = Directions[charAt].First(dir => (-dir.x, -dir.y) != direction);
                direction = ndir;
            }
            at = next;
        } while (map[at] != 'S');

        return steps / 2;
    }

    (int x, int y) Next((int x, int y) at, (int x, int y) dir)
        => (at.x + dir.x, at.y + dir.y);

    (int x, int y)[] StartingDirections((int x, int y) start, int width, int height, Map map) =>
        Edges(start, width, height)
            .Where(edge => map[edge] != '.')
            .Select(edge => Directions[map[edge]].FirstOrDefault(pos => (edge.x + pos.x, edge.y + pos.y) == start))
            .Where(found => found != default)
            .Select(found => (-found.x, -found.y))
            .ToArray();

    IEnumerable<(int x, int y)> Edges((int x, int y) start, int width, int height)
    {
        if (start.x > 0) yield return (start.x - 1, start.y);
        if (start.x < width - 1) yield return (start.x + 1, start.y);
        if (start.y > 0) yield return (start.x, start.y - 1);
        if (start.y < height - 1) yield return (start.x, start.y + 1);
    }

    Map GetMap(IEnumerable<string> indata) =>
        indata
         .SelectMany((line, y) => line.Select((pipe, x) => ((x, y), pipe)))
         .ToDictionary(pair => pair.Item1, pair => pair.pipe); 
}
