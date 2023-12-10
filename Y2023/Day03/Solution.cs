namespace aoc_runner.Y2023.Day03;

[PuzzleName("Gear Ratios")]
class Solution : ISolver
{
    public object PartOne(string input)
        => Solve(input, ValidParts); 

    public object PartTwo(string input)
        => Solve(input, GearRatios); 

    int Solve(string input, Func<Schematic, int> procedure)
        => procedure(Engine(input));

    int GearRatios(Schematic engine) => 
        engine.Symbols
        .Where(symbol => symbol.IsGear)
        .Sum(gear =>
        {
            var edges = SurroundingEdges(gear.Position);
            var numbers = engine.Numbers
                .Where(number => number.Positions.Any(edges.Contains))
                .ToArray();
            return numbers.Length == 2 ? numbers.Select(num => num.Value).Aggregate((a, b) => a * b) : 0;
        });

    int ValidParts(Schematic engine) =>
        engine.Numbers
        .Select(num => (num.Value, Edges: num.Positions.SelectMany(SurroundingEdges).Distinct()))
        .Where(num => engine.Symbols.Any(symbol => num.Edges.Contains(symbol.Position)))
        .Sum(num => num.Value);

    IEnumerable<(int x, int y)> SurroundingEdges((int x, int y) position)
    {
        for (int y = position.y - 1; y <= position.y + 1; y++)
        {
            for (int x = position.x - 1; x <= position.x + 1; x++)
            {
                yield return (x, y);
            }
        }
    }

    Schematic Engine(string input)
    {
        List<Number> numbers = [];
        HashSet<Symbol> symbols = [];
        foreach (var (y, line) in input.Split('\n').Select((line, y) => (y, line)))
        {
            var x = 0;
            while (x < line.Length)
            {
                var c = line[x];
                if (c == '.')
                {
                    x++;
                    continue;
                }

                if (!char.IsNumber(c))
                {
                    symbols.Add(new Symbol(c == '*', (x, y)));
                    x++;
                    continue;
                }

                int start = x;
                while (x < line.Length && char.IsNumber(line[x]))
                    x++;

                numbers.Add(new Number(int.Parse(line[start..x]), [.. Enumerable.Range(start, x - start).Select(x => (x, y))]));
            }
        }
        return new Schematic(numbers, symbols);
    }

    record Schematic(List<Number> Numbers, HashSet<Symbol> Symbols);

    record Number(int Value, (int x, int y)[] Positions);

    record Symbol(bool IsGear, (int x, int y) Position);
}
