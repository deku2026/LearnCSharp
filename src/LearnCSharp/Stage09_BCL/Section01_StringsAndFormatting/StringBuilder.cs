// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第1部分-字符串与格式化.md
// Stage    : Stage09_BCL
// Section  : Section01_StringsAndFormatting
// Item     : StringBuilder
// Topic id : stage09/section01/string_builder
//
// 步骤 2：StringBuilder 可变缓冲；循环 += 的 O(n²) 陷阱

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section01;

internal static class StringBuilderDemo
{
    [LearnTopic("stage09/section01/string_builder")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== StringBuilder ===");
        DemoQuadraticVsBuilder();
        DemoApi();
        DemoWhenNotToUse();
        return 0;
    }

    private static void DemoQuadraticVsBuilder()
    {
        Console.WriteLine("-- loop += vs StringBuilder --");
        const int n = 2000;
        Stopwatch sw = Stopwatch.StartNew();
        string slow = "";
        for (int i = 0; i < n; i++)
            slow += i + ",";
        sw.Stop();
        long msPlus = sw.ElapsedMilliseconds;

        sw.Restart();
        StringBuilder sb = new StringBuilder(capacity: n * 4);
        for (int i = 0; i < n; i++)
            sb.Append(i).Append(',');
        string fast = sb.ToString();
        sw.Stop();
        Debug.Assert(slow.Length == fast.Length);
        Console.WriteLine($"  n={n}: += {msPlus}ms, StringBuilder {sw.ElapsedMilliseconds}ms");
    }

    private static void DemoApi()
    {
        Console.WriteLine("-- Append / Insert / Replace / Clear --");
        StringBuilder sb = new StringBuilder();
        sb.Append("text");
        sb.AppendLine("line");
        sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0:N2}", 12.3);
        sb.Insert(0, "prefix-");
        sb.Replace("text", "TXT");
        Debug.Assert(sb.ToString().Contains("prefix-", StringComparison.Ordinal));
        Debug.Assert(sb.ToString().Contains("TXT", StringComparison.Ordinal));
        int len = sb.Length;
        sb.Clear();
        Debug.Assert(sb.Length == 0 && len > 0);
        Console.WriteLine($"  cleared after Length={len}");
    }

    private static void DemoWhenNotToUse()
    {
        Console.WriteLine("-- fixed few pieces: + / Concat is fine --");
        string a = "a", b = "b", c = "c";
        string concat = a + b + c; // compiler → String.Concat once
        Debug.Assert(concat == "abc");
        Console.WriteLine("  a+b+c → single Concat; skip StringBuilder for tiny cases");
    }
}
