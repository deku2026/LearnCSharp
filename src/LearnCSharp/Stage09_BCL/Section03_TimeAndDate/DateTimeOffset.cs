// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第3部分-时间与日期.md
// Stage    : Stage09_BCL
// Section  : Section03_TimeAndDate
// Item     : DateTimeOffset
// Topic id : stage09/section03/datetime_offset
//
// 步骤 3：DateTimeOffset 无歧义时间点；offset ≠ 时区

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section03;

internal static class DateTimeOffsetDemo
{
    [LearnTopic("stage09/section03/datetime_offset")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DateTimeOffset ===");
        DemoCreateAndConvert();
        DemoEqualityAcrossOffsets();
        DemoOffsetIsNotTimeZone();
        return 0;
    }

    private static void DemoCreateAndConvert()
    {
        Console.WriteLine("-- Now / UtcNow / explicit offset --");
        DateTimeOffset now = DateTimeOffset.Now;
        DateTimeOffset utc = DateTimeOffset.UtcNow;
        var dto = new DateTimeOffset(2026, 6, 14, 15, 30, 0, TimeSpan.FromHours(8));
        Debug.Assert(utc.Offset == TimeSpan.Zero);
        Debug.Assert(dto.Offset == TimeSpan.FromHours(8));
        DateTime utcDt = dto.UtcDateTime;
        Debug.Assert(utcDt.Kind == DateTimeKind.Utc);
        Console.WriteLine($"  dto={dto:O}; UtcDateTime={utcDt:O}; local offset={now.Offset}");
    }

    private static void DemoEqualityAcrossOffsets()
    {
        Console.WriteLine("-- comparison normalizes to UTC --");
        var a = new DateTimeOffset(2026, 6, 14, 15, 0, 0, TimeSpan.FromHours(8));
        var b = new DateTimeOffset(2026, 6, 14, 7, 0, 0, TimeSpan.Zero); // same instant
        Debug.Assert(a == b);
        Debug.Assert(a.UtcTicks == b.UtcTicks);
        Console.WriteLine($"  +08:00 15:00 == UTC 07:00 → {a == b}");
    }

    private static void DemoOffsetIsNotTimeZone()
    {
        Console.WriteLine("-- offset is a snapshot; not DST rules --");
        // +08:00 could be Beijing, Singapore, etc. — no zone identity retained
        var snap = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.FromHours(8));
        Debug.Assert(snap.Offset.TotalHours == 8);
        Console.WriteLine("  for future local wall times + DST, use TimeZoneInfo");
    }
}
