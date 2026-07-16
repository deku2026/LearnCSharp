// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第1部分-字符串与格式化.md
// Stage    : Stage09_BCL
// Section  : Section01_StringsAndFormatting
// Item     : StringInterpolationCulture
// Topic id : stage09/section01/string_interpolation_culture
//
// 步骤 5：内插、处理器、显示用 CurrentCulture / 持久化用 InvariantCulture

using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section01;

internal static class StringInterpolationCulture
{
    [LearnTopic("stage09/section01/string_interpolation_culture")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== StringInterpolationCulture ===");
        DemoInterpolationBasics();
        DemoDisplayVsPersistence();
        DemoHandlerWithStringBuilder();
        return 0;
    }

    private static void DemoInterpolationBasics()
    {
        Console.WriteLine("-- $\"...\" with format clauses --");
        int n = 42;
        double p = 0.125;
        string s = $"n={n:D3}, p={p:P1}";
        Debug.Assert(s.Contains("042", StringComparison.Ordinal));
        Console.WriteLine($"  {s}");
    }

    private static void DemoDisplayVsPersistence()
    {
        Console.WriteLine("-- display: culture; persist: InvariantCulture --");
        double value = 1234.5;
        FormattableString fs = $"amount={value:N2}";
        string display = fs.ToString(CultureInfo.GetCultureInfo("de-DE"));
        string persist = FormattableString.Invariant($"amount={value:N2}");
        Debug.Assert(persist.Contains("1,234.50", StringComparison.Ordinal)
                     || persist.Contains("1234.50", StringComparison.Ordinal));
        // culture-specific may use different separators
        Debug.Assert(display.Length > 0);
        Console.WriteLine($"  de-DE display: {display}");
        Console.WriteLine($"  Invariant persist: {persist}");
        Console.WriteLine("  UI → CurrentCulture; files/APIs/logs keys → InvariantCulture");
    }

    private static void DemoHandlerWithStringBuilder()
    {
        Console.WriteLine("-- StringBuilder.Append($\"...\") uses interpolated string handler --");
        var sb = new StringBuilder();
        int x = 7;
        sb.Append($"value={x}");
        Debug.Assert(sb.ToString() == "value=7");
        // DefaultInterpolatedStringHandler is what compiler uses for string interpolation
        DefaultInterpolatedStringHandler h = new(literalLength: 6, formattedCount: 1);
        h.AppendLiteral("hello ");
        h.AppendFormatted(x);
        string built = h.ToStringAndClear();
        Debug.Assert(built == "hello 7");
        Console.WriteLine($"  handler result: {built}");
    }
}
