// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第3部分-时间与日期.md
// Stage    : Stage09_BCL
// Section  : Section03_TimeAndDate
// Item     : TimeZoneInfoDst
// Topic id : stage09/section03/timezone_info_dst
//
// 步骤 5：TimeZoneInfo 转换；IANA ID；DST 陷阱意识

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section03;

internal static class TimeZoneInfoDst
{
    [LearnTopic("stage09/section03/timezone_info_dst")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TimeZoneInfoDst ===");
        DemoFindAndConvert();
        DemoIsDaylightSaving();
        DemoAmbiguousInvalid();
        return 0;
    }

    private static void DemoFindAndConvert()
    {
        Console.WriteLine("-- FindSystemTimeZoneById + ConvertTime --");
        TimeZoneInfo local = TimeZoneInfo.Local;
        TimeZoneInfo utc = TimeZoneInfo.Utc;
        Debug.Assert(utc.BaseUtcOffset == TimeSpan.Zero);

        TimeZoneInfo? tokyo = TryFindZone("Asia/Tokyo", "Tokyo Standard Time");
        DateTime utcNow = DateTime.UtcNow;
        DateTime asLocal = TimeZoneInfo.ConvertTimeFromUtc(utcNow, local);
        Debug.Assert(asLocal.Kind is DateTimeKind.Unspecified or DateTimeKind.Local);
        if (tokyo is not null)
        {
            DateTime asTokyo = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tokyo);
            Console.WriteLine($"  UTC={utcNow:O} → Tokyo={asTokyo:O} ({tokyo.Id})");
        }
        else
        {
            Console.WriteLine($"  local zone={local.Id}; Tokyo zone not found on this OS");
        }
    }

    private static void DemoIsDaylightSaving()
    {
        Console.WriteLine("-- IsDaylightSavingTime depends on zone rules --");
        TimeZoneInfo? eastern = TryFindZone("America/New_York", "Eastern Standard Time");
        if (eastern is null)
        {
            Console.WriteLine("  Eastern zone unavailable; skip DST probe");
            return;
        }

        // mid-summer vs mid-winter (US Eastern typically DST in July, not January)
        var summer = new DateTime(2026, 7, 15, 12, 0, 0, DateTimeKind.Unspecified);
        var winter = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Unspecified);
        bool summerDst = eastern.IsDaylightSavingTime(summer);
        bool winterDst = eastern.IsDaylightSavingTime(winter);
        Console.WriteLine($"  {eastern.Id}: July DST={summerDst}; Jan DST={winterDst}");
        Debug.Assert(summerDst || !summerDst); // platform-dependent but should run
    }

    private static void DemoAmbiguousInvalid()
    {
        Console.WriteLine("-- DST fold/gap: IsAmbiguousTime / IsInvalidTime --");
        TimeZoneInfo? eastern = TryFindZone("America/New_York", "Eastern Standard Time");
        if (eastern is null)
        {
            Console.WriteLine("  skip ambiguous/invalid checks");
            return;
        }

        // These wall times may be invalid/ambiguous around transitions; APIs exist to detect
        var sample = new DateTime(2026, 3, 8, 2, 30, 0); // near US spring-forward historically
        bool invalid = eastern.IsInvalidTime(sample);
        bool ambiguous = eastern.IsAmbiguousTime(sample);
        Console.WriteLine($"  sample local {sample}: invalid={invalid}, ambiguous={ambiguous}");
        Console.WriteLine("  store UTC/DateTimeOffset; convert for display with TimeZoneInfo");
    }

    private static TimeZoneInfo? TryFindZone(string iana, string windows)
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById(iana); }
        catch (TimeZoneNotFoundException) { }
        catch (InvalidTimeZoneException) { }

        try { return TimeZoneInfo.FindSystemTimeZoneById(windows); }
        catch (TimeZoneNotFoundException) { return null; }
        catch (InvalidTimeZoneException) { return null; }
    }
}
