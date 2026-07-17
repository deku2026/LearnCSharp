// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第3部分-时间与日期.md
// Stage    : Stage09_BCL
// Section  : Section03_TimeAndDate
// Item     : TimeBestPracticesChronoCompare
// Topic id : stage09/section03/time_best_practices_chrono_compare
//
// 步骤 7：最佳实践 + 与 C++ std::chrono 概念对照

using System.Diagnostics;
using System.Globalization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section03;

internal static class TimeBestPracticesChronoCompare
{
    [LearnTopic("stage09/section03/time_best_practices_chrono_compare")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TimeBestPracticesChronoCompare ===");
        DemoStoreUtcDisplayLocal();
        DemoRoundTripFormat();
        DemoChronoMapping();
        return 0;
    }

    private static void DemoStoreUtcDisplayLocal()
    {
        Console.WriteLine("-- store UTC / DateTimeOffset; convert for UI --");
        DateTimeOffset stored = DateTimeOffset.UtcNow;
        DateTimeOffset display = stored.ToLocalTime();
        Debug.Assert(stored.UtcTicks == display.UtcTicks);
        Console.WriteLine($"  stored UTC={stored:O}");
        Console.WriteLine($"  display local={display:O}");
    }

    private static void DemoRoundTripFormat()
    {
        Console.WriteLine("-- round-trip: O / o format + InvariantCulture --");
        DateTimeOffset original = new(2026, 7, 16, 8, 30, 0, TimeSpan.FromHours(8));
        string text = original.ToString("O", CultureInfo.InvariantCulture);
        DateTimeOffset parsed = DateTimeOffset.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        Debug.Assert(parsed == original);
        Console.WriteLine($"  round-trip OK: {text}");
    }

    private static void DemoChronoMapping()
    {
        Console.WriteLine("-- C++20 chrono conceptual map --");
        Console.WriteLine("  TimeSpan ≈ duration; DateTimeOffset ≈ zoned/sys time + offset");
        Console.WriteLine("  DateOnly ≈ year_month_day; TimeOnly ≈ hh_mm_ss; TimeZoneInfo ≈ time_zone");
        TimeSpan d = TimeSpan.FromMilliseconds(500);
        Debug.Assert(d.TotalMilliseconds == 500);
        Console.WriteLine($"  sample duration 500ms → {d}");
    }
}
