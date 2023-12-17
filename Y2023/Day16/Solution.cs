namespace aoc_runner.Y2023.Day16;

using System;

using Map = Dictionary<(int x, int y), char>;
using Position = (int x, int y);
using Direction = (int x, int y);

[PuzzleName("The Floor Will Be Lava")]
class Solution : ISolver
{
    public object PartOne(string input) 
        => Solve(input, PartOneConfigurations);

    public object PartTwo(string input) 
        => Solve(input, PartTwoConfigurations);

    Configuration[] PartOneConfigurations(int width, int height)
        => [new Configuration((0,0), (0, 1))];

    Configuration[] PartTwoConfigurations(int width, int height)
    {
        List<Configuration> candidates = [];
        for (var x = 0; x < width; x++)
        {
            candidates.Add(new Configuration((x, 0), (0, 1)));
            candidates.Add(new Configuration((x, height - 1), (0, -1)));
        }
        for (var y = 0; y < width; y++)
        {
            candidates.Add(new Configuration((0, y), (1, 0)));
            candidates.Add(new Configuration((width - 1, y), (-1, 0)));
        }
        return [..candidates];
    }

    int Solve(string input, Func<int, int, Configuration[]> configurations)
    {
        var (map, width, height) = Map(input);
        var best = 0;

        foreach (var config in configurations(width, height))
        {
            
            var closed = new HashSet<(Position at, Direction dir)>([(config.StartAt, config.Direction)]); // Track the 'closed' loops 
            var beams = new List<Beam>([new Beam(config.StartAt, config.Direction)]); // Active beams
            var seen = new HashSet<Position>([config.StartAt]); // Visited positions

            while (beams.Count != 0)
            {
                beams = beams.SelectMany(beam =>
                {
                    var nextPosition = NextPosition(beam.At, beam.Direction);
                    if (!InBounds(nextPosition, width, height))
                    {
                        beam.Disable();
                        return [beam];
                    }

                    seen.Add(nextPosition);

                    var charAtNext = map[nextPosition];
                    var nextDirections = NextDirections(beam.Direction, charAtNext)
                        .Where(direction => InBounds(NextPosition(nextPosition, direction), width, height))
                        .ToArray();

                    if (nextDirections.Length > 1)
                    {
                        beam.Disable();
                        return nextDirections.Select(dir => new Beam(nextPosition, dir));
                    }
                    if (nextDirections.Length == 1)
                        beam.Move(nextPosition, nextDirections[0], closed);
                    if (nextDirections.Length == 0)
                        beam.Disable();

                    return [beam];
                }).Where(x => x != null && x.Active).ToList();
            }
            best = Math.Max(best, seen.Count);
        }

        return best;
    }

    bool InBounds(Position at, int width, int height) =>
        at.x >= 0 && at.x <= width - 1 && at.y >= 0 && at.y <= height - 1;

    Position NextPosition(Position at, Direction dir) =>
        (at.x + dir.x, at.y + dir.y);

    IEnumerable<Direction> NextDirections(Direction dir, char charAt)
    {
        return charAt switch
        {
            '.' => [dir],
            '\\' => dir switch
            {
                { x: 0, y: -1 } => [(-1, 0)], // North to West
                { x: 1, y: 0 } => [(0, 1)], // East to South
                { x: 0, y: 1 } => [(1, 0)], // South to East
                { x: -1, y: 0 } => [(0, -1)], // West to North
                _ => throw new InvalidOperationException("Invalid direction")
            },
            '/' => dir switch
            {
                { x: 0, y: -1 } => [(1, 0)], // North to East
                { x: 1, y: 0 } => [(0, -1)], // East to North
                { x: 0, y: 1 } => [(-1, 0)], // South to West
                { x: -1, y: 0 } => [(0, 1)], // West to South
                _ => throw new InvalidOperationException("Invalid direction")
            },
            '|' => dir switch
            {
                { x: 0, y: -1 } or { x: 0, y: 1 } => [dir], // North or South continues
                { x: 1, y: 0 } or { x: -1, y: 0 } => [(0, 1), (0, -1)], // East or West splits
                _ => throw new InvalidOperationException("Invalid direction")
            },
            '-' => dir switch
            {
                { x: 0, y: -1 } or { x: 0, y: 1 } => [(1, 0), (-1, 0)], // North or South splits
                { x: 1, y: 0 } or { x: -1, y: 0 } => [dir], // East or West continues
                _ => throw new InvalidOperationException("Invalid direction")
            },
            _ => throw new InvalidOperationException($"Encountered unhandled character in map: {charAt}"),
        };
    }

    (Map map, int width, int height) Map(string input)
    {
        Map map = [];
        var lines = input.Split('\n');
        foreach (var (line, y) in lines.Select((line, y) => (line, y)))
        {
            foreach (var (pos, x) in line.Select((pos, x) => (pos, x))) 
            {
                map[(x, y)] = pos;
            }
        }
        return (map, lines[0].Length, lines.Length);
    }

    record Configuration(Position StartAt, Direction Direction);

    class Beam(Position at, Direction direction)
    {
        public Position At { get; set; } = at;
        public Direction Direction { get; set; } = direction;
        public void Move(Position nextAt, Direction dir, HashSet<(Position at, Direction dir)> seen)
        {
            if (seen.Contains((nextAt, dir)))
            {
                Disable();
                return;
            }

            At = nextAt;
            Direction = dir;
            seen.Add((nextAt, dir));
        }

        public bool Active { get; set; } = true;
        public void Disable() => Active = false;
    }
}
