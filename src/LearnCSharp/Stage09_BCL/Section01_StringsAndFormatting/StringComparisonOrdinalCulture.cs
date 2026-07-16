// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第1部分-字符串与格式化.md
// Stage    : Stage09_BCL
// Section  : Section01_StringsAndFormatting
// Item     : StringComparisonOrdinalCulture
// Topic id : stage09/section01/string_comparison_ordinal_culture
//
// 步骤 3：务必显式 StringComparison；Ordinal vs Culture

using System.Diagnostics;
using System.Globalization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section01;

internal static class StringComparisonOrdinalCulture
{
    [LearnTopic("stage09/section01/string_comparison_ordinal_culture")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== StringComparisonOrdinalCulture ===");
        DemoAlwaysSpecify();
        DemoOrdinalVsCulture();
        DemoCaseInsensitive();
        return 0;
    }

    private static void DemoAlwaysSpecify()
    {
        Console.WriteLine("-- always pass StringComparison for symbolic strings --");
        string url = "https://example.com";
        bool starts = url.StartsWith("https", StringComparison.Ordinal);
        bool eq = string.Equals("Key", "key", StringComparison.OrdinalIgnoreCase);
        Debug.Assert(starts && eq);
        Console.WriteLine($"  StartsWith Ordinal={starts}; Equals IgnoreCase={eq}");
    }

    private static void DemoOrdinalVsCulture()
    {
        Console.WriteLine("-- precomposed é vs combining e+́ --");
        string precomposed = "\u00E9"; // é
        string combining = "e\u0301";  // e + combining acute
        bool cultureEq = string.Equals(precomposed, combining, StringComparison.CurrentCulture);
        bool ordinalEq = string.Equals(precomposed, combining, StringComparison.Ordinal);
        Debug.Assert(cultureEq || !cultureEq); // culture may normalize
        Debug.Assert(!ordinalEq); // different code units
        Console.WriteLine($"  Culture equal? {cultureEq}; Ordinal equal? {ordinalEq}");
        Console.WriteLine("  symbolic keys/IDs/paths → Ordinal; user-facing sort → CurrentCulture");
    }

    private static void DemoCaseInsensitive()
    {
        Console.WriteLine("-- OrdinalIgnoreCase for protocol/header style --");
        int cmp = string.Compare("HTTP", "http", StringComparison.OrdinalIgnoreCase);
        Debug.Assert(cmp == 0);
        // invariant for culture-stable programmatic data when needed
        bool inv = "i".Equals("I", StringComparison.InvariantCultureIgnoreCase);
        Debug.Assert(inv);
        Console.WriteLine($"  Compare OrdinalIgnoreCase=0; InvariantIgnoreCase={inv}");
        _ = CultureInfo.InvariantCulture;
    }
}
