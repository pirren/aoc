using System.Text.RegularExpressions;

namespace aoc_runner;

static partial class Generator
{
    public static Task Generate(int year)
    {
        var basePath = "..\\..\\..\\";
        var yearFolder = $"Y{year}";
        Directory.CreateDirectory(Path.Combine(basePath, yearFolder));
        
        for(var day = 1; day <= 25; day++)
        {
            var dayFolder = $"Day{day:00}";
            var filePath = Path.Combine(basePath, yearFolder, dayFolder, $"Solution.cs");
            Directory.CreateDirectory(Path.Combine(basePath, yearFolder, dayFolder));
            GeneratePuzzle(year, day).WriteToFile(filePath);
        }
        return Task.CompletedTask;
    }

    private static void WriteToFile(this string st, string path)
    {
        File.WriteAllText(path, st);
    }

    public static string GeneratePuzzle(int year, int day)
    {
        return $@"namespace aoc_runner.Y{year}.Day{day:00};
             |
             |[PuzzleName(""Name for puzzle {day}"")]
             |class Solution : ISolver
             |{{
             |    public object PartOne(string input) 
             |    {{
             |        return 0;
             |    }}
             |
             |    public object PartTwo(string input) 
             |    {{
             |        return 0;
             |    }}
             |}}
             |".StripMargin();
    }

    public static string StripMargin(this string st, string margin = "|")
    {
        return string.Join("\n",
            from line in MarginPattern().Split(st)
            select Regex.Replace(line, @"^\s*" + Regex.Escape(margin), "")
        );
    }

    [GeneratedRegex("\r?\n")]
    private static partial Regex MarginPattern();
}
