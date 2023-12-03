namespace aoc_runner.Y2023.Day03;

[PuzzleName("Gear Ratios")]
class Solution : ISolver
{
    public object PartOne(string input)
    {
        return new Engine(input).SumValidParts();
    }

    public object PartTwo(string input)
    {
        return new Engine(input).SumGearRatios();
    }

    class Engine
    {
        readonly List<Number> Numbers = [];
        readonly HashSet<Symbol> Symbols = [];

        public Engine(string input)
        {
            var map = input.Split('\n');

            var maxLength = map.Max(line => line.Length);
            for (int y = 0; y < map.Length; y++)
            {
                Number activeNumber = null!;
                List<char> values = null!;
                for (int x = 0; x < maxLength; x++)
                {
                    var c = map[y][x];
                    if (c == '.')
                    {
                        AddAndFlushNumber(ref activeNumber, ref values);
                        activeNumber = null!;
                        continue;
                    }

                    if(!char.IsNumber(c))
                    {
                        AddAndFlushNumber(ref activeNumber, ref values);
                        Symbols.Add(new Symbol(c == '*', x, y));
                        continue;
                    }

                    if (activeNumber == null)
                    {
                        activeNumber = new Number(-1, [(x, y)]);
                        values = [ c ];
                        continue;
                    }

                    values.Add(c);
                    activeNumber.Positions.Add((x, y));
                }
                AddAndFlushNumber(ref activeNumber, ref values);
            }
        }

        private void AddAndFlushNumber(ref Number activeNumber, ref List<char> values)
        {
            if(activeNumber == null) 
                return;
            Numbers.Add(activeNumber with { Value = int.Parse(new string(values.ToArray())) });
            activeNumber = null!;
            values = null!;
        }

        public int SumValidParts()
        {
            return Numbers.Sum(GetSumFromPart);
        }

        public int SumGearRatios()
        {
            return Symbols.Where(symbol => symbol.IsGearSymbol).Sum(GetGearRatio);
        }

        private int GetGearRatio(Symbol s)
        {
            var edges = GetSurroundingEdges(s.Position);
            var numbers = Numbers.Where(number => number.Positions.Any(edges.Contains)).ToArray();

            return numbers.Length == 2 
                ? numbers.Select(num => num.Value).Aggregate((a, b) => a * b)
                : 0;
        }

        private int GetSumFromPart(Number n)
        {
            foreach (var edge in n.Positions.SelectMany(GetSurroundingEdges).Distinct())
            {
                if (Symbols.Select(symbol => symbol.Position).Contains(edge))
                {
                    return n.Value;
                }
            }
            return 0;
        }

        private IEnumerable<(int x, int y)> GetSurroundingEdges((int x, int y) position)
        {
            for(int y = position.y - 1; y <= position.y + 1; y++)
            {
                for(int x = position.x - 1; x <= position.x + 1; x++)
                {
                    yield return (x, y);
                }
            }
        }
    }

    record Number(int Value, HashSet<(int x, int y)> Positions);

    record Symbol(bool IsGearSymbol, int X, int Y)
    {
        public (int x, int y) Position => (X, Y);
    }
}
