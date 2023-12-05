using System.Collections.Frozen;
using System.Data;

namespace aoc_runner.Y2023.Day05;

[PuzzleName("If You Give A Seed A Fertilizer")]
class Solution : ISolver
{
    List<string> MapOrder = ["seed", "soil", "fertilizer", "water", "light", "temperature", "humidity", "location"];
    FrozenSet<long[]> Seeds = null!;
    FrozenDictionary<(string Source, string Destination), FrozenSet<Range>> Map = null!;

    public object PartOne(string input)
    {
        (Seeds, Map) = ParseMapping(input);
        return Seeds.SelectMany(s => s.Select(x => GetLocation(x))).Min();
    }

    public object PartTwo(string input)
    {
        (Seeds, Map) = ParseMapping(input);
        MapOrder.Reverse(); // Reverse order of mapping

        int lowestLocation = 0;

        // Test from location = 0 to find first location within seed range and brute force
        Parallel.ForEach(Enumerable.Range(0, int.MaxValue), (location, state) => 
        {
            var nextSeed = GetSeedFromLocation(location);

            if (Seeds.Any(seed => seed[0] <= nextSeed && nextSeed <= seed[0] + seed[1]))
            {
                lowestLocation = location;
                state.Break();
            }
        });

        return lowestLocation;
    }

    long GetLocation(long value, string from = "seed")
    {
        if (from == "humidity")
            return GetMappedValue(value, (from, MapOrder[^1]), r => r.SourceMin <= value && value < r.SourceMin + r.Length);

        var nextProperty = MapOrder[MapOrder.IndexOf(from) + 1]!;
        value = GetMappedValue(value, (from, nextProperty), r => r.SourceMin <= value && value < r.SourceMin + r.Length);

        return GetLocation(value, nextProperty);
    }

    long GetSeedFromLocation(long value, string from = "location")
    {
        if (from == "soil")
            return GetMappedValue(value, (MapOrder[^1], from), r => r.DestMin <= value && value < r.DestMin + r.Length, true);

        var nextProperty = MapOrder[MapOrder.IndexOf(from) + 1]!;
        value = GetMappedValue(value, (nextProperty, from), r => r.DestMin <= value && value < r.DestMin + r.Length, true);

        return GetSeedFromLocation(value, nextProperty);
    }

    long GetMappedValue(long value, (string, string) mapKey, Func<Range, bool> rangeSelector, bool reverse = false)
    {
        var ranges = Map[mapKey]
            .Where(rangeSelector)
            .ToArray();

        if (ranges.Length == 0)
            return value;
        var (DestMin, SourceMin, _) = ranges[0];

        return value + (reverse ? SourceMin - DestMin : DestMin - SourceMin);
    }

    (FrozenSet<long[]> Seeds, FrozenDictionary<(string Source, string Destination), FrozenSet<Range>> Map) ParseMapping(string input) 
    {
        var seeds = input.Split("\n\n")[0][7..].Split().Select(long.Parse).Chunk(2).ToFrozenSet();

        Dictionary<(string source, string destination), FrozenSet<Range>> dict = [];
        foreach (var line in input.Split("\n\n").Skip(1))
        {
            var data = line.Split('\n');
            var source = data[0].Split('-')[0];
            var destination = data[0].Split('-')[^1].Replace(" map:", string.Empty);

            dict.Add((source, destination), data.Skip(1).Select(d => d.Split(" "))
                .Select(r => new Range(
                    DestMin: long.Parse(r[0]),
                    SourceMin: long.Parse(r[1]),
                    Length: long.Parse(r[2])
                )).ToFrozenSet());
        }

        return (seeds, dict.ToFrozenDictionary());
    }

    record Range(long DestMin, long SourceMin, long Length);
}
