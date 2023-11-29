using System.Reflection;

namespace aoc_runner;

interface ISolver
{
    object PartOne(string input);
    object PartTwo(string input);
}

[AttributeUsage(AttributeTargets.Class)]
class PuzzleName(string name) : Attribute
{
    public string Name { get; private set; } = name;
}

class AocException(string title, string details) : Exception
{
    public override string Message => $"{title}\n[{details}]";
}

static class SolverExtensions
{
    public static IEnumerable<object> Solve(this ISolver solver, string input)
    {
        yield return solver.PartOne(input);
        var res = solver.PartTwo(input);
        if (res != null)
        {
            yield return res;
        }
    }
    public static string GetName(this ISolver solver)
    {
        try
        {
            return (
                solver
                    .GetType()
                    .GetCustomAttribute(typeof(PuzzleName)) as PuzzleName
            ).Name;
        } 
        catch (ArgumentNullException e) 
        {
            throw new ArgumentNullException(e.Message);
        }
    }

    public static string DayName(this ISolver solver)
    {
        return $"Day {solver.Day()}";
    }

    public static int Year(this ISolver solver)
    {
        return Year(solver.GetType());
    }

    public static int Year(Type t)
    {
        return int.Parse(t.FullName.Split('.')[1].Substring(1));
    }

    public static int Day(this ISolver solver)
    {
        return Day(solver.GetType());
    }

    public static int Day(Type t)
    {
        return int.Parse(t.FullName.Split('.')[2][3..]);
    }

    public static string WorkingDir(int year)
    {
        return Path.Combine(year.ToString());
    }

    public static string WorkingDir(int year, int day)
    {
        return Path.Combine(WorkingDir(year), "Day" + day.ToString("00"));
    }

    public static string WorkingDir(this ISolver solver)
    {
        return WorkingDir(solver.Year(), solver.Day());
    }
}

class Runner
{
    private static string GetNormalizedInput(string file)
    {
        var input = File.ReadAllText(file);

        input = input.Replace("\r", "");

        if (input.EndsWith('\n'))
            input = input[..^1];

        return input;
    }

    public static void RunSolver(ISolver solver, bool useSampleData)
    {
        var workingDir = solver.WorkingDir();
        var indent = "    ";

        Console.WriteLine($"{indent}{solver.DayName()}: {solver.GetName()}");

        var file = Path.Combine(workingDir, useSampleData ? "sample.in" : "input.in");
        var input = GetNormalizedInput(file);

        foreach (var result in solver.Solve(input))
        {
            Console.WriteLine($"{indent}{result}");
        }
    }

    public static void RunAll(ISolver[] solvers, bool useSampleDatta = false)
    {
        foreach (var solver in solvers)
        {
            RunSolver(solver, useSampleDatta);
        }
    }
}