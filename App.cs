using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using aoc_runner;

var tSolvers = Assembly.GetEntryAssembly()!.GetTypes()
    .Where(t => t.GetTypeInfo().IsClass && typeof(ISolver).IsAssignableFrom(t))
    .OrderBy(t => t.FullName)
    .ToArray();

var action = Command(args, Args("put", "([0-9]+)/([0-9]+)"), m =>
    {
        var year = int.Parse(m[1]);
        var day = int.Parse(m[2]);

        if(tSolvers.Any(tsolver =>
            SolverExtensions.Year(tsolver) == year &&
            SolverExtensions.Day(tsolver) == day))
        {
            return () => Console.WriteLine($"Duplicate puzzle. Year = {year}, Day = {day} already exists.");
        }
        else if(day is < 1 or > 25)
        {
            return () => Console.WriteLine($"Calendar is only 1 through 25.");
        }
        return () => Console.WriteLine("TODO: Put new puzzle successfully.");
    }) ??
    Command(args, Args("sample","([0-9]+)/(Day)?([0-9]+)"), m =>
    {
        var year = int.Parse(m[1]);
        var day = int.Parse(m[3]);

        var tSolversSelected = tSolvers.First(tsolver =>
            SolverExtensions.Year(tsolver) == year &&
            SolverExtensions.Day(tsolver) == day);

        return () => Runner.RunAll(GetSolvers(tSolversSelected), useSampleDatta: true);
    }) ??
    Command(args, Args("([0-9]+)/(Day)?([0-9]+)"), m =>
    {
        var year = int.Parse(m[0]);
        var day = int.Parse(m[2]);

        var tSolversSelected = tSolvers.First(tsolver =>
            SolverExtensions.Year(tsolver) == year &&
            SolverExtensions.Day(tsolver) == day);

        return () => Runner.RunAll(GetSolvers(tSolversSelected));
    });

string[] Args(params string[] regex) => regex;

try
{
    action();
}
catch (AggregateException a)
{
    if(a.InnerExceptions.Count == 1 && a.InnerException is AocException)
        throw a.InnerExceptions[0];
    else
        throw;
}

ISolver[] GetSolvers(params Type[] tsolver) 
    => tsolver.Select(t => Activator.CreateInstance(t) as ISolver)
        .Where(x => x != null)
        .ToArray()!;

Action? Command(string[] args, string[] regexes, Func<string[], Action> parse)
{
    if (args.Length != regexes.Length)
        return null;

    var matches = Enumerable.Zip(args, regexes, (arg, regex) => new Regex("^" + regex + "$").Match(arg));
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
