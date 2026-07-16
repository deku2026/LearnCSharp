// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第1部分-字符串与格式化.md
// Stage    : Stage09_BCL
// Section  : Section01_StringsAndFormatting
// Item     : ParseTryParseIParsable
// Topic id : stage09/section01/parse_tryparse_iparsable
//
// 步骤 6：Parse vs TryParse；IParsable / ISpanFormattable

using System.Diagnostics;
using System.Globalization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section01;

internal static class ParseTryParseIParsable
{
    [LearnTopic("stage09/section01/parse_tryparse_iparsable")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ParseTryParseIParsable ===");
        DemoParseVsTryParse();
        DemoCultureAwareParse();
        DemoIParsable();
        DemoISpanFormattable();
        return 0;
    }

    private static void DemoParseVsTryParse()
    {
        Console.WriteLine("-- Parse throws; TryParse returns bool --");
        int ok = int.Parse("42", CultureInfo.InvariantCulture);
        Debug.Assert(ok == 42);
        bool parsed = int.TryParse("not-a-number", CultureInfo.InvariantCulture, out int value);
        Debug.Assert(!parsed && value == 0);
        bool parsed2 = int.TryParse("99", NumberStyles.Integer, CultureInfo.InvariantCulture, out int v2);
        Debug.Assert(parsed2 && v2 == 99);
        Console.WriteLine($"  TryParse bad → {parsed}; good → {v2}");
    }

    private static void DemoCultureAwareParse()
    {
        Console.WriteLine("-- decimal separators depend on culture --");
        CultureInfo de = CultureInfo.GetCultureInfo("de-DE");
        bool deOk = double.TryParse("1.234,5", NumberStyles.Number, de, out double deVal);
        bool invOk = double.TryParse("1234.5", NumberStyles.Number, CultureInfo.InvariantCulture, out double invVal);
        Debug.Assert(deOk && Math.Abs(deVal - 1234.5) < 0.001);
        Debug.Assert(invOk && Math.Abs(invVal - 1234.5) < 0.001);
        Console.WriteLine($"  de-DE '1.234,5' → {deVal}; Invariant '1234.5' → {invVal}");
    }

    private static void DemoIParsable()
    {
        Console.WriteLine("-- IParsable<T> generic parse (int implements it) --");
        int n = ParseAny<int>("123", CultureInfo.InvariantCulture);
        Debug.Assert(n == 123);
        bool ok = TryParseAny<int>("456", CultureInfo.InvariantCulture, out int m);
        Debug.Assert(ok && m == 456);
        Console.WriteLine($"  ParseAny<int> → {n}; TryParseAny → {m}");
    }

    private static void DemoISpanFormattable()
    {
        Console.WriteLine("-- ISpanFormattable.TryFormat --");
        Guid g = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");
        Span<char> buf = stackalloc char[36];
        bool ok = ((ISpanFormattable)g).TryFormat(buf, out int written, "D", CultureInfo.InvariantCulture);
        Debug.Assert(ok && written == 36);
        Console.WriteLine($"  Guid TryFormat → {buf[..written].ToString()}");
    }

    private static T ParseAny<T>(string s, IFormatProvider? provider)
        where T : IParsable<T>
        => T.Parse(s, provider);

    private static bool TryParseAny<T>(string s, IFormatProvider? provider, out T result)
        where T : IParsable<T>
        => T.TryParse(s, provider, out result!);
}
