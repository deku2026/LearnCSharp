// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第3部分-时间与日期.md
// Stage    : Stage09_BCL
// Section  : Section03_TimeAndDate
// Item     : TimeProviderDotNet8
// Topic id : stage09/section03/time_provider_dotnet8
//
// 步骤 6：TimeProvider — 可注入、可测试的时间抽象

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section03;

internal static class TimeProviderDotNet8
{
    private sealed class OrderService(TimeProvider clock)
    {
        public DateTimeOffset CreatedAt => clock.GetUtcNow();

        public bool IsExpired(DateTimeOffset deadline)
            => clock.GetUtcNow() > deadline;
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    [LearnTopic("stage09/section03/time_provider_dotnet8")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TimeProviderDotNet8 ===");
        DemoSystemProvider();
        DemoFakeForTests();
        return 0;
    }

    private static void DemoSystemProvider()
    {
        Console.WriteLine("-- TimeProvider.System is production clock --");
        TimeProvider clock = TimeProvider.System;
        DateTimeOffset utc = clock.GetUtcNow();
        long ts = clock.GetTimestamp();
        Debug.Assert(utc.Offset == TimeSpan.Zero || utc.ToUniversalTime().Offset == TimeSpan.Zero);
        Debug.Assert(ts != 0 || ts == 0);
        Console.WriteLine($"  System GetUtcNow={utc:O}; timestamp={ts}");
    }

    private static void DemoFakeForTests()
    {
        Console.WriteLine("-- inject FixedTimeProvider → deterministic tests --");
        var fixedUtc = new DateTimeOffset(2026, 7, 16, 12, 0, 0, TimeSpan.Zero);
        var clock = new FixedTimeProvider(fixedUtc);
        var svc = new OrderService(clock);
        Debug.Assert(svc.CreatedAt == fixedUtc);
        Debug.Assert(svc.IsExpired(fixedUtc.AddMinutes(-1)));
        Debug.Assert(!svc.IsExpired(fixedUtc.AddHours(1)));
        Console.WriteLine($"  CreatedAt fixed to {svc.CreatedAt:O}; no flaky DateTime.UtcNow");
    }
}
