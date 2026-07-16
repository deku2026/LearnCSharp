// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第4部分-测试.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section04_Testing
// Item     : WritingTestsAaaTheory
// Topic id : stage10/section04/writing_tests_aaa_theory
//
// AAA、Theory/InlineData 思想、setup/teardown（无 xunit 包）。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section04;

internal static class WritingTestsAaaTheory
{
    [LearnTopic("stage10/section04/writing_tests_aaa_theory")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== WritingTestsAaaTheory ===");
        DemoAaaPattern();
        DemoTheoryStyle();
        DemoSetupTeardown();
        DemoNaming();
        DemoAssertionStyles();
        return 0;
    }

    private static void DemoAaaPattern()
    {
        Console.WriteLine("-- AAA: Arrange / Act / Assert --");
        // Arrange
        var cart = new Cart();
        cart.Add("book", 10m);
        cart.Add("pen", 2m);
        // Act
        decimal total = cart.Total();
        // Assert
        Debug.Assert(total == 12m);
        Console.WriteLine($"  Total()={total} (Arrange items → Act Total → Assert 12)");
        Console.WriteLine("  一个测试一个行为；避免多重无关 Assert 掩盖失败原因");
    }

    private static void DemoTheoryStyle()
    {
        Console.WriteLine("-- Theory + data rows (xUnit style, simulated) --");
        (int A, int B, int Expected)[] rows =
        [
            (1, 1, 2),
            (2, 3, 5),
            (-1, 5, 4),
            (0, 0, 0),
        ];
        foreach (var (a, b, expected) in rows)
        {
            int actual = a + b;
            Debug.Assert(actual == expected);
            Console.WriteLine($"  [InlineData] {a}+{b} => {actual}");
        }
        Console.WriteLine("  xUnit: [Theory] + [InlineData]/[MemberData]；NUnit: [TestCase]");
        Console.WriteLine("  同一逻辑多组输入，避免复制粘贴 Fact");
    }

    private static void DemoSetupTeardown()
    {
        Console.WriteLine("-- setup / teardown --");
        var fixture = new TempFolderFixture();
        try
        {
            fixture.Setup();
            Debug.Assert(Directory.Exists(fixture.Path));
            File.WriteAllText(Path.Combine(fixture.Path, "a.txt"), "hi");
            Debug.Assert(File.Exists(Path.Combine(fixture.Path, "a.txt")));
            Console.WriteLine($"  setup created: {fixture.Path}");
        }
        finally
        {
            fixture.Teardown();
            Debug.Assert(!Directory.Exists(fixture.Path));
            Console.WriteLine("  teardown removed temp folder");
        }
        Console.WriteLine("  xUnit: ctor/Dispose 或 IClassFixture；NUnit: [SetUp]/[TearDown]");
    }

    private static void DemoNaming()
    {
        Console.WriteLine("-- naming patterns --");
        string[] names =
        [
            "Add_EmptyCart_ReturnsZero",
            "Parse_InvalidToken_ThrowsFormatException",
            "Method_Scenario_Expected",
        ];
        foreach (string n in names)
            Console.WriteLine($"  {n}");
        Debug.Assert(names[^1].Contains('_'));
    }

    private static void DemoAssertionStyles()
    {
        Console.WriteLine("-- assertion styles --");
        int value = 42;
        // classic
        Debug.Assert(value == 42);
        // constraint-like message
        if (value is < 0 or > 100)
            throw new InvalidOperationException("out of range");
        Debug.Assert(value is >= 0 and <= 100);
        Console.WriteLine("  优先断言有意义差异；消息写清期望 vs 实际");
        Console.WriteLine("  FluentAssertions 等库增强可读性（可选包）");
    }

    private sealed class Cart
    {
        private readonly List<decimal> _prices = [];
        public void Add(string _, decimal price) => _prices.Add(price);
        public decimal Total() => _prices.Sum();
    }

    private sealed class TempFolderFixture
    {
        public string Path { get; private set; } = "";
        public void Setup()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "learn-csharp-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }
        public void Teardown()
        {
            if (!string.IsNullOrEmpty(Path) && Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }
    }
}
