// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第4部分-IO与正则.md
// Stage    : Stage09_BCL
// Section  : Section04_IOAndRegex
// Item     : RegexBasics
// Topic id : stage09/section04/regex_basics
//
// 步骤 4：Regex 基础 Match / Groups / Replace / 静态 API

using System.Diagnostics;
using System.Text.RegularExpressions;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section04;

internal static class RegexBasics
{
    [LearnTopic("stage09/section04/regex_basics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== RegexBasics ===");
        DemoIsMatchAndMatch();
        DemoGroups();
        DemoReplaceAndSplit();
        return 0;
    }

    private static void DemoIsMatchAndMatch()
    {
        Console.WriteLine("-- IsMatch / Match / Matches --");
        const string emailish = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        Debug.Assert(Regex.IsMatch("ada@example.com", emailish));
        Debug.Assert(!Regex.IsMatch("not-an-email", emailish));
        Match m = Regex.Match("order-42 ready", @"order-(\d+)");
        Debug.Assert(m.Success && m.Groups[1].Value == "42");
        MatchCollection all = Regex.Matches("a1 b22 c3", @"\d+");
        Debug.Assert(all.Count == 3);
        Console.WriteLine($"  groups[1]={m.Groups[1].Value}; digit matches={all.Count}");
    }

    private static void DemoGroups()
    {
        Console.WriteLine("-- named groups --");
        var re = new Regex(@"(?<area>\d{3})-(?<num>\d{4})", RegexOptions.CultureInvariant);
        Match m = re.Match("555-1212");
        Debug.Assert(m.Success);
        Debug.Assert(m.Groups["area"].Value == "555");
        Debug.Assert(m.Groups["num"].Value == "1212");
        Console.WriteLine($"  area={m.Groups["area"].Value}, num={m.Groups["num"].Value}");
    }

    private static void DemoReplaceAndSplit()
    {
        Console.WriteLine("-- Replace / Split --");
        string masked = Regex.Replace("card 1234-5678-9012", @"\d{4}-\d{4}-\d{4}", "****-****-****");
        Debug.Assert(masked.Contains("****", StringComparison.Ordinal));
        string[] parts = Regex.Split("one,two;three", "[,;]");
        Debug.Assert(parts is ["one", "two", "three"]);
        Console.WriteLine($"  masked={masked}; split=[{string.Join("|", parts)}]");
    }
}
