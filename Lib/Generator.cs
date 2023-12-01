namespace aoc_runner;

class Generator
{
    public Task Generate(int year)
    {
        var basePath = "..\\..\\..\\";
        Directory.CreateDirectory(Path.Combine(basePath, SolverExtensions.WorkingDir(year)));

        for (var day = 1; day <= 3; day++)
        {
            var workingDir = Path.Combine(basePath, SolverExtensions.WorkingDir(year, day));
            Directory.CreateDirectory(workingDir);
            GetIndataTemplate().WriteToFile(Path.Combine(workingDir, $"indata.in"));
            GetIndataTemplate().WriteToFile(Path.Combine(workingDir, $"sample.in"));
            GetPuzzleTemplate(year, day).WriteToFile(Path.Combine(workingDir, $"Solution.cs"));
        }
        return Task.CompletedTask;
    }

    private string GetPuzzleTemplate(int year, int day)
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

    private static string GetIndataTemplate()
        => string.Empty;
}
