// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第1部分-字符串与格式化.md
// Stage    : Stage09_BCL
// Section  : Section01_StringsAndFormatting
// Item     : CompositeFormatAndFormatStrings
// Topic id : stage09/section01/composite_format_and_format_strings
//
// 步骤 4：复合格式、标准/自定义格式串、TryFormat 零分配

using System.Diagnostics;
using System.Globalization;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section01;

internal static class CompositeFormatAndFormatStrings
{
    [LearnTopic("stage09/section01/composite_format_and_format_strings")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CompositeFormatAndFormatStrings ===");
        DemoCompositeFormat();
        DemoStandardFormats();
        DemoCustomAndDate();
        DemoTryFormat();
        return 0;
    }

    private static void DemoCompositeFormat()
    {
        Console.WriteLine("-- string.Format / composite {index:format} --");
        CultureInfo inv = CultureInfo.InvariantCulture;
        string s = string.Format(inv, "qty {0:N0}, price {1:F2}, pct {2:P1}", 1234, 9.99, 0.085);
        Debug.Assert(s.Contains("1,234", StringComparison.Ordinal));
        Debug.Assert(s.Contains("9.99", StringComparison.Ordinal));
        Console.WriteLine($"  {s}");
    }

    private static void DemoStandardFormats()
    {
        Console.WriteLine("-- N / D / P / X / G --");
        CultureInfo inv = CultureInfo.InvariantCulture;
        Debug.Assert((1234.5678).ToString("N2", inv) == "1,234.57");
        Debug.Assert(1234.ToString("D6", inv) == "001234");
        Debug.Assert(255.ToString("X", inv) == "FF");
        Console.WriteLine($"  N2={(1234.5678).ToString("N2", inv)}; D6={1234.ToString("D6", inv)}; X={255.ToString("X", inv)}");
    }

    private static void DemoCustomAndDate()
    {
        Console.WriteLine("-- custom numeric + date format --");
        CultureInfo inv = CultureInfo.InvariantCulture;
        string num = 42.ToString("0000", inv);
        Debug.Assert(num == "0042");
        var dt = new DateTime(2026, 7, 16, 14, 30, 0, DateTimeKind.Utc);
        string iso = dt.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", inv);
        Debug.Assert(iso == "2026-07-16T14:30:00Z");
        Console.WriteLine($"  custom num={num}; ISO-ish={iso}");
    }

    private static void DemoTryFormat()
    {
        Console.WriteLine("-- TryFormat into stack buffer (zero alloc path) --");
        Span<char> buf = stackalloc char[32];
        bool ok = 12345.TryFormat(buf, out int written, "D8", CultureInfo.InvariantCulture);
        Debug.Assert(ok && written == 8);
        string s = buf[..written].ToString();
        Debug.Assert(s == "00012345");
        Console.WriteLine($"  TryFormat → '{s}'");
        // .NET 8+ CompositeFormat for repeated formats
        CompositeFormat fmt = CompositeFormat.Parse("id={0}, v={1:N1}");
        string composed = string.Format(CultureInfo.InvariantCulture, fmt, 7, 3.14);
        Debug.Assert(composed.Contains("id=7", StringComparison.Ordinal));
        Console.WriteLine($"  CompositeFormat → {composed}");
    }
}
