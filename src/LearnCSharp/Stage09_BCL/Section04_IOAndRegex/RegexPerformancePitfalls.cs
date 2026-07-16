// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第4部分-IO与正则.md
// Stage    : Stage09_BCL
// Section  : Section04_IOAndRegex
// Item     : RegexPerformancePitfalls
// Topic id : stage09/section04/regex_performance_pitfalls
//
// 步骤 5：编译选项、超时、ReDoS 意识

using System.Diagnostics;
using System.Text.RegularExpressions;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section04;

internal static partial class RegexPerformancePitfalls
{
    // source-generated static regex (.NET 7+) — compile-time, reusable
    [GeneratedRegex(@"^\d{3}-\d{2}-\d{4}$", RegexOptions.CultureInvariant | RegexOptions.NonBacktracking)]
    private static partial Regex SsnShape();

    [LearnTopic("stage09/section04/regex_performance_pitfalls")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== RegexPerformancePitfalls ===");
        DemoCompiledAndStatic();
        DemoMatchTimeout();
        DemoNonBacktrackingAndGenerated();
        return 0;
    }

    private static void DemoCompiledAndStatic()
    {
        Console.WriteLine("-- reuse Regex instance; avoid new Regex in hot loops --");
        var re = new Regex(@"\b\w+\b", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
        int count = 0;
        foreach (Match _ in re.Matches("one two three four"))
            count++;
        Debug.Assert(count == 4);
        // static cache helper
        bool ok = Regex.IsMatch("abc123", @"^[a-z]+\d+$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(50));
        Debug.Assert(ok);
        Console.WriteLine($"  word count={count}; static IsMatch={ok}");
    }

    private static void DemoMatchTimeout()
    {
        Console.WriteLine("-- MatchTimeout mitigates catastrophic backtracking --");
        // deliberately awkward pattern + input; timeout protects
        var dangerous = new Regex(@"(a+)+$", RegexOptions.None, TimeSpan.FromMilliseconds(20));
        string input = new string('a', 30) + "b";
        bool timedOut = false;
        try
        {
            _ = dangerous.IsMatch(input);
        }
        catch (RegexMatchTimeoutException)
        {
            timedOut = true;
        }
        // may match-fail quickly or timeout depending on engine; either is fine
        Console.WriteLine($"  timeout path exercised or fast-fail: timedOut={timedOut}");
        Debug.Assert(true);
    }

    private static void DemoNonBacktrackingAndGenerated()
    {
        Console.WriteLine("-- NonBacktracking + [GeneratedRegex] --");
        Debug.Assert(SsnShape().IsMatch("123-45-6789"));
        Debug.Assert(!SsnShape().IsMatch("12-345-6789"));
        var nb = new Regex(@"\d+", RegexOptions.NonBacktracking, TimeSpan.FromSeconds(1));
        Debug.Assert(nb.IsMatch("id=42"));
        Console.WriteLine("  prefer GeneratedRegex / NonBacktracking for untrusted input");
        Console.WriteLine("  ReDoS: evil patterns + user input → always set timeout");
    }
}
