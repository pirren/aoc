using System.Text.RegularExpressions;

namespace aoc_runner;

public static partial class AppExtensions
{

    public static string StripMargin(this string st, string margin = "|")
    {
        return string.Join("\n",
            from line in MarginPattern().Split(st)
            select Regex.Replace(line, @"^\s*" + Regex.Escape(margin), "")
        );
    }

    public static void WriteToFile(this string st, string path)
    {
        File.WriteAllText(path, st);
    }

    [GeneratedRegex("\r?\n")]
    private static partial Regex MarginPattern();
}