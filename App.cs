using aoc_runner;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

var tSolvers = Assembly.GetEntryAssembly()!.GetTypes()
    .Where(t => t.GetTypeInfo().IsClass && typeof(ISolver).IsAssignableFrom(t))
    .OrderBy(t => t.FullName)
    .ToArray();

var action = Command(args, Args("generate", "([0-9]+)"), m =>
    {
        var year = int.Parse(m[1]);

        if (tSolvers.Any(tsolver => SolverExtensions.Year(tsolver) == year))
        {
            return () => throw new AocException("Calendar already exists", $"Calendar for year {year} already exists.");
        }

        return () =>
        {
            new Generator().Generate(year);
            Console.WriteLine($"Successfully generated year {year}.");
        };
    }) ??
    Command(args, Args("sample", "([0-9]+)/(Day)?([0-9]+)"), m =>
    {
        var year = int.Parse(m[1]);
        var day = int.Parse(m[3]);

        if (day is < 1 or > 25)
        {
            return () => throw new AocException("Invalid date", "Calendar is only 1 through 25");
        }

        var tSolverSelected = tSolvers.FirstOrDefault(tsolver =>
            SolverExtensions.Year(tsolver) == year &&
            SolverExtensions.Day(tsolver) == day);

        if (tSolverSelected == null)
        {
            return () => throw new AocException("Puzzle not found", $"No puzzle found for {year}/{day}.");
        }

        return () => new Runner().RunAll(GetSolvers(tSolverSelected), useSampleDatta: true);
    }) ??
    Command(args, Args("([0-9]+)/(Day)?([0-9]+)"), m =>
    {
        var year = int.Parse(m[0]);
        var day = int.Parse(m[2]);

        if (day is < 1 or > 25)
        {
            return () => throw new AocException("Invalid date", "Calendar is only 1 through 25");
        }

        var tSolverSelected = tSolvers.FirstOrDefault(tsolver =>
            SolverExtensions.Year(tsolver) == year &&
            SolverExtensions.Day(tsolver) == day);

        if (tSolverSelected == null)
        {
            return () => throw new AocException("Puzzle not found", $"No puzzle found for {year}/{day}.");
        }

        return () => new Runner().RunAll(GetSolvers(tSolverSelected));
    }) ?? (() => {
        var dt = DateTime.Now;
        var tSolverDevelopment = tSolvers.FirstOrDefault(tsolver => SolverExtensions.Year(tsolver) == dt.Year &&
            SolverExtensions.Day(tsolver) == dt.Day);
        if (tSolverDevelopment == null)
        {
            Console.WriteLine("No action.");
            return;
        }

        new Runner().RunSolver(GetSolvers(tSolverDevelopment!)[0], false);
    });

string[] Args(params string[] regex) => regex;

try
{
    action();
}
catch (AocException ex)
{
    Console.WriteLine(ex.Message);
}
catch (NullReferenceException)
{
    Console.WriteLine("Action was null.");
    Environment.Exit(1);
}
catch (FileNotFoundException ex)
{
    Console.WriteLine(ex.Message);
}

ISolver[] GetSolvers(params Type[] tsolver)
    => tsolver.Select(t => Activator.CreateInstance(t) as ISolver)
        .Where(x => x != null)
        .ToArray()!;

Action? Command(string[] args, string[] regexes, Func<string[], Action> parse)
{
    if (args.Length != regexes.Length)
        return null;

    var matches = args.Zip(regexes, (arg, regex) => new Regex("^" + regex + "$").Match(arg)).ToArray();
    if (!matches.All(match => match.Success))
        return null;

    try
    {
        return parse(matches.SelectMany(m =>
            m.Groups.Count > 1
                ? m.Groups.Cast<Group>().Skip(1).Select(g => g.Value)
                : [m.Value]
        ).ToArray());
    }
    catch
    {
        return null;
    }
}
