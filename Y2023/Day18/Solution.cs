namespace aoc_runner.Y2023.Day18;

[PuzzleName("Lavaduct Lagoon")]
class Solution : ISolver
{
    public object PartOne(string input) 
        => Solve(input, InstructionsPartOne);

    public object PartTwo(string input)
        => Solve(input, InstructionsPartTwo);
    
    long Solve(string input, Func<string, Instruction[]> parseInstructions)
    {
        var instructions = parseInstructions(input);
        var vertices = GetVertices(instructions);
        var perimeter = Perimeter(instructions);

        var area = PolygonArea(vertices);
        var interior = PicksTheorem(area, perimeter);

        return interior + perimeter;
    }
    
    // Pick's theorem
    // https://en.wikipedia.org/wiki/Pick%27s_theorem#Generalizations
    long PicksTheorem(long area, long perimeter)
        => area - perimeter / 2 + 1;

    // 'Shoelace' formula
    // only requires vertices (edges)!
    // https://en.wikipedia.org/wiki/Shoelace_formula#Generalization
    long PolygonArea(IReadOnlyList<Point> vertices) => 
        Math.Abs(vertices.SkipLast(1).Select((p, i) =>
        {
            var (x1, y1) = p;
            var (x2, y2) = vertices[i + 1];
            return x1 * y2 - x2 * y1;
        }).Sum()) / 2;

    long Perimeter(Instruction[] instructions) => 
        instructions.Sum(i => i.Steps);

    IReadOnlyList<Point> GetVertices(Instruction[] instructions)
    {
        List<Point> vertices = [];
        var vertex = new Point(0, 0);

        foreach (var ins in instructions)
        {
            vertex = ins.Direction switch
            {
                "U" => vertex with { Y = vertex.Y - ins.Steps },
                "R" => vertex with { X = vertex.X + ins.Steps },
                "D" => vertex with { Y = vertex.Y + ins.Steps },
                "L" => vertex with { X = vertex.X - ins.Steps },
                _ => throw new Exception("Unknown direction")
            };

            vertices.Add(vertex);
        }
        return [.. vertices];
    }

    Instruction[] InstructionsPartTwo(string indata) =>
        indata.Split('\n').Select(line =>
        {
            var tokens = line.Split(" ");
            var hexString = tokens.Last().Substring(2, 6);
            return new Instruction(Convert.ToInt32(hexString[^1].ToString(), fromBase: 16) switch
            {
                0 => "R",
                1 => "D",
                2 => "L",
                3 => "U",
                _ => throw new Exception("Unknown direction")
            }, Convert.ToInt32(hexString[..^1], fromBase: 16));
        }).ToArray();

    Instruction[] InstructionsPartOne(string indata) => 
        indata.Split('\n').Select(line =>
        {
            var tokens = line.Split(" ");
            return new Instruction(tokens.First(), int.Parse(tokens[1]));
        }).ToArray();

    record Instruction(string Direction, long Steps);
    record Point(long X, long Y);
}
