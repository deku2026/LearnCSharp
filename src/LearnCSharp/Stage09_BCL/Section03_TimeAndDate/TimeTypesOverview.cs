// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第3部分-时间与日期.md
// Stage    : Stage09_BCL
// Section  : Section03_TimeAndDate
// Item     : TimeTypesOverview
// Topic id : stage09/section03/time_types_overview
//
// 步骤 1：六种类型总览 + 选哪个

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section03;

internal static class TimeTypesOverview
{
    [LearnTopic("stage09/section03/time_types_overview")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TimeTypesOverview ===");
        DemoEachType();
        DemoDecisionTable();
        return 0;
    }

    private static void DemoEachType()
    {
        Console.WriteLine("-- six BCL time concepts --");
        DateTime dt = DateTime.UtcNow;
        DateTimeOffset dto = DateTimeOffset.UtcNow;
        DateOnly date = DateOnly.FromDateTime(dt);
        TimeOnly time = TimeOnly.FromDateTime(dt);
        TimeSpan span = TimeSpan.FromMinutes(30);
        TimeZoneInfo tz = TimeZoneInfo.Utc;
        Debug.Assert(dto.Offset == TimeSpan.Zero);
        Debug.Assert(date.Year >= 2020);
        Debug.Assert(time.Hour is >= 0 and <= 23);
        Debug.Assert(span.TotalMinutes == 30);
        Debug.Assert(tz.Id.Length > 0);
        Console.WriteLine($"  DateTime Kind={dt.Kind}; DateTimeOffset Offset={dto.Offset}");
        Console.WriteLine($"  DateOnly={date}; TimeOnly={time}; TimeSpan={span}; TZ={tz.Id}");
    }

    private static void DemoDecisionTable()
    {
        Console.WriteLine("-- choose type by intent --");
        // birthday → DateOnly (no accidental timezone shift)
        DateOnly birthday = new(1990, 5, 20);
        // event timestamp → DateTimeOffset
        DateTimeOffset occurred = DateTimeOffset.UtcNow;
        // business hours → TimeOnly
        TimeOnly open = new(9, 0);
        // timeout → TimeSpan
        TimeSpan timeout = TimeSpan.FromSeconds(30);
        Debug.Assert(birthday.Month == 5 && open.Hour == 9 && timeout.TotalSeconds == 30);
        Console.WriteLine($"  birthday={birthday}; event={occurred:O}; open={open}; timeout={timeout}");
        Console.WriteLine("  Prefer DateTimeOffset/DateOnly/TimeOnly over bare DateTime when possible");
    }
}
