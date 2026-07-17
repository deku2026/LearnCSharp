// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第3部分-时间与日期.md
// Stage    : Stage09_BCL
// Section  : Section03_TimeAndDate
// Item     : DateTimeAndKind
// Topic id : stage09/section03/datetime_and_kind
//
// 步骤 2：DateTime + DateTimeKind（Local/Utc/Unspecified 陷阱）

using System.Diagnostics;
using System.Globalization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section03;

internal static class DateTimeAndKind
{
    [LearnTopic("stage09/section03/datetime_and_kind")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DateTimeAndKind ===");
        DemoBasics();
        DemoKinds();
        DemoSpecifyKind();
        return 0;
    }

    private static void DemoBasics()
    {
        Console.WriteLine("-- Now / UtcNow / AddDays --");
        DateTime now = DateTime.Now;
        DateTime utc = DateTime.UtcNow;
        DateTime today = DateTime.Today;
        DateTime dt = new DateTime(2026, 6, 14, 15, 30, 0);
        DateTime next = dt.AddDays(7);
        Debug.Assert(next.Day == 21);
        Debug.Assert(now.Kind == DateTimeKind.Local);
        Debug.Assert(utc.Kind == DateTimeKind.Utc);
        Debug.Assert(today.TimeOfDay == TimeSpan.Zero);
        Console.WriteLine($"  local Kind={now.Kind}; utc Kind={utc.Kind}; +7d day={next.Day}");
    }

    private static void DemoKinds()
    {
        Console.WriteLine("-- Kind tri-state; Unspecified is ambiguous --");
        Debug.Assert(DateTime.Now.Kind == DateTimeKind.Local);
        Debug.Assert(DateTime.UtcNow.Kind == DateTimeKind.Utc);
        Debug.Assert(new DateTime(2026, 1, 1).Kind == DateTimeKind.Unspecified);
        DateTime parsed = DateTime.Parse("2026-01-01", CultureInfo.InvariantCulture);
        Debug.Assert(parsed.Kind is DateTimeKind.Unspecified or DateTimeKind.Local);
        Console.WriteLine($"  new DateTime → Unspecified; Parse Kind={parsed.Kind}");
        Console.WriteLine("  only UTC DateTime is an unambiguous instant");
    }

    private static void DemoSpecifyKind()
    {
        Console.WriteLine("-- SpecifyKind changes tag only, not ticks --");
        DateTime raw = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);
        DateTime asUtc = DateTime.SpecifyKind(raw, DateTimeKind.Utc);
        Debug.Assert(raw.Ticks == asUtc.Ticks);
        Debug.Assert(asUtc.Kind == DateTimeKind.Utc);
        Console.WriteLine($"  same ticks={raw.Ticks}; Kind now {asUtc.Kind}");
    }
}
