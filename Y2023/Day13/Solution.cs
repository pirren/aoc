using System.Runtime.ExceptionServices;
using System.Text;

namespace aoc_runner.Y2023.Day13;

[PuzzleName("Point of Incidence")]
class Solution : ISolver
{
    public object PartOne(string input)
    {
        // Too low 19918
        return Solve(input, LargestSymmetry);
    }

    public object PartTwo(string input)
    {
        return Solve(input, LargestSymmetry);
    }

    int Solve(string input, Func<string[], int> parse)
    {
        var mirrors = Mirrors(input).ToArray();
        var colScore = 0;
        var rowScore = 0;
        foreach (var mirror in mirrors)
        {
            var row = parse(mirror.Rows().ToArray());
            var col = parse(mirror.Cols().ToArray());

            if (row > col)
                rowScore += row;
            if (col > row)
                colScore += col;
        }

        return rowScore * 100 + colScore;
    }


    int LargestSymmetry(string[] lines)
        => Enumerable.Range(1, lines.Length - 1).Max(i => Reflects(lines, i - 1, i) ? i : 0);

    bool Reflects(string[] lines, int indexA, int indexB)
    {
        if (!lines[indexA].SequenceEqual(lines[indexB]))
            return false;

        if (indexA == 0 || indexB == lines.Length - 1)
            return true;

        return Reflects(lines, indexA - 1, indexB + 1);
    }

    IEnumerable<Mirror> Mirrors(string input)
    {
        var blocks = input.Split("\n\n");
        foreach (var block in blocks)
        {
            var lines = block.Split('\n');
            var height = lines.Length;
            var width = lines.Max(line => line.Length);

            var map = new char[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    map[x, y] = lines[y][x];
                }
            }

            yield return new Mirror(map, width, height, lines);
        }
    }

    record Mirror(char[,] Map, int Width, int Height, string[] Lines)
    {

        public IEnumerable<string> Rows() => Lines;

        public IEnumerable<string> Cols() {
            StringBuilder sb = new();
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    sb.Append(Lines[y][x]);
                }
                yield return sb.ToString();
                sb.Clear();
            }
        }
    };
}
