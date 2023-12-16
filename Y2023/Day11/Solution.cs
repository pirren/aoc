namespace aoc_runner.Y2023.Day11;

using Expansions = (int[] x, int[] y, int expandBy);
using Node = (long x, long y);

[PuzzleName("Cosmic Expansion")]
class Solution : ISolver
{
    public object PartOne(string input)
        => Solve(input, 1);

    public object PartTwo(string input)
        => Solve(input, 999_999); 

    long Solve(string input, int expandBy)
    {
        var expansion = Expansion(input, expandBy);
        var nodes = Nodes(input, expansion);

        List<long> distances = [];
        Queue<Node> queue = new();

        foreach (var (source, idx) in nodes.Select((node, idx) => (node, idx)))
        {
            foreach (var node in nodes.Skip(idx + 1))
            {
                queue.Enqueue(node);
            }
            
            while (queue.Count != 0)
            {
                var dest = queue.Dequeue();
                distances.Add(Manhattan(source, dest));
            }
        }

        return distances.Sum();
    }

    long Manhattan(Node p1, Node p2) => Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);

    List<Node> Nodes(string indata, Expansions exp)
    {
        var graph = new List<Node>();
        var lines = indata.Split('\n').ToList();

        for (var y = 0; y < lines.Count; y++)
        {
            for (var x = 0; x < lines[0].Length; x++)
            {
                var charAt = lines[y][x];
                if (charAt == '#')
                {
                    var node = new Node(
                            x + (exp.expandBy * exp.x.Count(num => num < x)), 
                            y + (exp.expandBy * exp.y.Count(num => num < y))
                        );
                    graph.Add(node);
                }
            }
        }

        return graph;
    }

    Expansions Expansion(string indata, int expandBy)
    {
        var data = indata.Split('\n');

        var rows = Enumerable.Range(0, data.Length).Where(y => data[y].All(ch => ch == '.'));
        var cols = Enumerable.Range(0, data[0].Length).Where(x => data.All(row => row[x] == '.'));

        return new Expansions(cols.ToArray(), rows.ToArray(), expandBy);
    }
}
