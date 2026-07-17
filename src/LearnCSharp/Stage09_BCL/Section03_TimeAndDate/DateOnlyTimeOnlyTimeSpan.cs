// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第3部分-时间与日期.md
// Stage    : Stage09_BCL
// Section  : Section03_TimeAndDate
// Item     : DateOnlyTimeOnlyTimeSpan
// Topic id : stage09/section03/dateonly_timeonly_timespan
//
// 步骤 4：DateOnly / TimeOnly 类型安全 + TimeSpan 间隔

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section03;

internal static class DateOnlyTimeOnlyTimeSpan
{
    [LearnTopic("stage09/section03/dateonly_timeonly_timespan")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DateOnlyTimeOnlyTimeSpan ===");
        DemoDateOnly();
        DemoTimeOnly();
        DemoTimeSpan();
        return 0;
    }

    private static void DemoDateOnly()
    {
        Console.WriteLine("-- DateOnly: calendar date without time/zone --");
        DateOnly birthday = new(1990, 5, 20);
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        DateOnly later = birthday.AddYears(36);
        Debug.Assert(later.Year == 2026);
        // DayOfWeek 是 enum，验证其类型归属（运行时判定，避免常量折叠）
        object dow = birthday.DayOfWeek;
        Debug.Assert(dow is DayOfWeek);
        // DateOnly has no ConvertTime / Kind — cannot mis-shift a birthday
        Console.WriteLine($"  birthday={birthday}; today={today}; +36y={later}");
    }

    private static void DemoTimeOnly()
    {
        Console.WriteLine("-- TimeOnly: wall-clock time of day --");
        TimeOnly opening = new(9, 0);
        TimeOnly closing = new(17, 0);
        TimeOnly noon = new(12, 0);
        Debug.Assert(noon.IsBetween(opening, closing));
        TimeOnly fromDt = TimeOnly.FromDateTime(DateTime.UtcNow);
        Debug.Assert(fromDt.Hour is >= 0 and <= 23);
        Console.WriteLine($"  open? noon in [09:00,17:00]={noon.IsBetween(opening, closing)}; now={fromDt}");
    }

    private static void DemoTimeSpan()
    {
        Console.WriteLine("-- TimeSpan is duration, not an instant --");
        TimeSpan duration = TimeSpan.FromHours(2.5);
        TimeSpan timeout = TimeSpan.FromSeconds(30);
        DateTime a = new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        DateTime b = a.AddHours(3);
        TimeSpan diff = b - a;
        Debug.Assert(duration.TotalMinutes == 150);
        Debug.Assert(timeout.TotalSeconds == 30);
        Debug.Assert(diff == TimeSpan.FromHours(3));
        Console.WriteLine($"  2.5h={duration}; timeout={timeout}; diff={diff}");
    }
}
