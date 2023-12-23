namespace aoc_runner.Y2023.Day10;

using Map = Dictionary<(int x, int y), char>;
using Point = (int x, int y);

[PuzzleName("Pipe Maze")]
class Solution : ISolver
{
    public object PartOne(string input) => 
        Solve(input, (steps, _) => steps / 2);

    public object PartTwo(string input) => 
        Solve(input, EnclosedArea); 
    
    long Solve(string input, Func<int, List<Point>, long> returnCase)
    {
        var data = input.Split('\n');
        var map = GetMap(data);

        var start = map.FirstOrDefault(position => position.Value == 'S').Key;
        var direction = StartingDirections(start, data[0].Length, data.Length, map).First();

        var at = start;
        var steps = 0;

        var (sx, sy) = new Point(start.x, start.y); // Used to order vertices
        List<Point> vertices = [new Point(0, 0)];
        do
        {
            steps++;
            var next = Next(at, direction);
            var charAt = map[next];

            if (charAt == 'S') break;

            if (charAt != '|' && charAt != '-')
            {
                vertices.Add(new Point(next.x - sx, next.y - sy));
                var ndir = Directions[charAt].First(dir => (-dir.x, -dir.y) != direction);
                direction = ndir;
            }
            at = next;
        } while (map[at] != 'S');

        return returnCase(steps, vertices);
    }

    long EnclosedArea(int steps, List<Point> vertices)
    {
        var area = PolygonArea(vertices);
        var interior = PicksTheorem(area, steps);

        return interior;
    }

    readonly Dictionary<char, Point[]> Directions = new() {
        { '|', [(0, 1), (0, -1)] },
        { '-', [(1, 0), (-1, 0)] },
        { 'L', [(0, -1), (1, 0)] },
        { 'J', [(-1, 0), (0, -1)] },
        { '7', [(-1, 0), (0, 1)] },
        { 'F', [(1, 0), (0, 1)] },
        { '.', [] }
    };

    // Use altered Picks
    // A = i + b / 2 + h - 1
    long PicksTheorem(long area, long perimeter) => 
        area - perimeter / 2 + 1;

    long PolygonArea(IReadOnlyList<Point> vertices) =>
        Math.Abs(vertices.SkipLast(1).Select((p, i) =>
        {
            var (x1, y1) = p;
            var (x2, y2) = vertices[i + 1];
            return x1 * y2 - x2 * y1;
        }).Sum()) / 2;

    Point Next(Point at, Point dir)
        => (at.x + dir.x, at.y + dir.y);

    Point[] StartingDirections(Point start, int width, int height, Map map) =>
        Edges(start, width, height)
            .Where(edge => map[edge] != '.')
            .Select(edge => Directions[map[edge]].FirstOrDefault(pos => (edge.x + pos.x, edge.y + pos.y) == start))
            .Where(found => found != default)
            .Select(found => (-found.x, -found.y))
            .ToArray();

    IEnumerable<Point> Edges(Point start, int width, int height)
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
