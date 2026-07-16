// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第4部分-测试.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section04_Testing
// Item     : UnitVsIntegrationTestability
// Topic id : stage10/section04/unit_vs_integration_testability
//
// 单元 vs 集成、可测试设计、测试替身（对接 DI）。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section04;

internal static class UnitVsIntegrationTestability
{
    [LearnTopic("stage10/section04/unit_vs_integration_testability")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== UnitVsIntegrationTestability ===");
        DemoUnitVsIntegration();
        DemoHardToTestVsSeams();
        DemoTestDoubles();
        DemoPyramid();
        return 0;
    }

    private static void DemoUnitVsIntegration()
    {
        Console.WriteLine("-- unit vs integration --");
        (string Kind, string Scope, string Speed)[] rows =
        [
            ("Unit", "单一单元 + 替身依赖", "毫秒级"),
            ("Integration", "真实协作（DB/HTTP/文件）", "较慢"),
            ("E2E", "完整用户路径", "最慢/最脆"),
        ];
        foreach (var (kind, scope, speed) in rows)
            Console.WriteLine($"  {kind,-12} {scope,-28} {speed}");
        Debug.Assert(rows[0].Speed.Contains("毫秒"));
    }

    private static void DemoHardToTestVsSeams()
    {
        Console.WriteLine("-- hard-to-test vs seams --");
        // 难测：硬编码时钟/静态 IO
        Console.WriteLine("  难: new HttpClient() 满天飞、静态 DateTime.Now、直接读配置文件");
        Console.WriteLine("  易: 依赖接口 + 构造注入 → 测试可替换");

        var hard = new HardWiredGreeter();
        Debug.Assert(hard.Greet().StartsWith("hi@", StringComparison.Ordinal));

        var easy = new Greeter(new FixedClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)));
        string msg = easy.Greet("Ada");
        Debug.Assert(msg == "hi Ada @ 2026-01-01");
        Console.WriteLine($"  seam result: {msg}");
    }

    private static void DemoTestDoubles()
    {
        Console.WriteLine("-- test doubles --");
        (string Kind, string Role)[] doubles =
        [
            ("Fake", "可用的简化实现（内存仓库）"),
            ("Stub", "返回固定数据"),
            ("Mock", "验证交互是否发生（行为）"),
            ("Spy", "记录调用再断言"),
            ("Dummy", "填参数但不使用"),
        ];
        foreach (var (kind, role) in doubles)
            Console.WriteLine($"  {kind,-6} {role}");

        var fakeMail = new FakeMailer();
        var svc = new OrderService(fakeMail);
        svc.Place("a@b.c");
        Debug.Assert(fakeMail.LastTo == "a@b.c");
        Console.WriteLine($"  FakeMailer captured: {fakeMail.LastTo}");
        Console.WriteLine("  真实项目常用 Moq/NSubstitute；能手写 Fake 更清晰时不必强上 Mock");
    }

    private static void DemoPyramid()
    {
        Console.WriteLine("-- test pyramid --");
        Console.WriteLine("  多单元、适量集成、少 E2E");
        Console.WriteLine("  集成测契约与配置；单元测分支与规则");
        int[] counts = [50, 10, 2]; // unit, integration, e2e (demo ratio)
        Debug.Assert(counts[0] > counts[1] && counts[1] > counts[2]);
        Console.WriteLine($"  demo ratio unit:int:e2e = {counts[0]}:{counts[1]}:{counts[2]}");
    }

    private interface IClock
    {
        DateTimeOffset UtcNow { get; }
    }

    private sealed class FixedClock(DateTimeOffset now) : IClock
    {
        public DateTimeOffset UtcNow { get; } = now;
    }

    private sealed class Greeter(IClock clock)
    {
        public string Greet(string name) => $"hi {name} @ {clock.UtcNow:yyyy-MM-dd}";
    }

    private sealed class HardWiredGreeter
    {
        public string Greet() => $"hi@{DateTimeOffset.UtcNow:HH}";
    }

    private interface IMailer
    {
        void Send(string to);
    }

    private sealed class FakeMailer : IMailer
    {
        public string? LastTo { get; private set; }
        public void Send(string to) => LastTo = to;
    }

    private sealed class OrderService(IMailer mailer)
    {
        public void Place(string to) => mailer.Send(to);
    }
}
