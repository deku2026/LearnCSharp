// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第4部分-测试.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section04_Testing
// Item     : WritingTestsAaaTheory
// Topic id : stage10/section04/writing_tests_aaa_theory
//
// AAA、Theory/InlineData 思想、setup/teardown — 可执行 assert 演示（无 xunit 包）。

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
        Cart cart = new Cart();
        cart.Add("book", 10m);
        cart.Add("pen", 2m);
        // Act
        decimal total = cart.Total();
        // Assert
        Debug.Assert(total == 12m);
        Debug.Assert(cart.Count == 2);
        Console.WriteLine($"  Total()={total}; Count={cart.Count}");
        Console.WriteLine("  一个测试一个行为；避免多重无关 Assert 掩盖失败原因");
    }

    private static void DemoTheoryStyle()
    {
        Console.WriteLine("-- Theory + data rows (xUnit style, simulated) --");
        (string Input, bool Expected)[] rows =
        [
            ("", false),
            ("ada", true),
            ("   ", false),
            ("x", true),
        ];
        int passed = 0;
        foreach ((string? input, bool expected) in rows)
        {
            bool actual = IsNonEmpty(input);
            Debug.Assert(actual == expected);
            passed++;
            Console.WriteLine($"  [InlineData] IsNonEmpty({input!}) => {actual}");
        }

        Debug.Assert(passed == rows.Length);
        Console.WriteLine($"  theory rows passed: {passed}/{rows.Length}");
        Console.WriteLine("  xUnit: [Theory] + [InlineData]/[MemberData]；NUnit: [TestCase]");
    }

    private static void DemoSetupTeardown()
    {
        Console.WriteLine("-- setup / teardown --");
        TempFolderFixture fixture = new TempFolderFixture();
        try
        {
            fixture.Setup();
            Debug.Assert(Directory.Exists(fixture.Path));
            string file = Path.Join(fixture.Path, "a.txt");
            File.WriteAllText(file, "hi");
            Debug.Assert(File.Exists(file));
            Debug.Assert(File.ReadAllText(file) == "hi");
            Console.WriteLine($"  setup created: {fixture.Path}");
        }
        finally
        {
            fixture.Teardown();
            Debug.Assert(!Directory.Exists(fixture.Path));
            Console.WriteLine("  teardown removed temp folder");
        }
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
        Debug.Assert(names.All(n => n.Contains('_', StringComparison.Ordinal)));
    }

    private static void DemoAssertionStyles()
    {
        Console.WriteLine("-- assertion styles --");
        int value = 42;
        Debug.Assert(value == 42);
        Debug.Assert(value is >= 0 and <= 100);
        // exception-style assert for educational parity with unit test frameworks
        try
        {
            _ = int.Parse("not-a-number", System.Globalization.CultureInfo.InvariantCulture);
            Debug.Assert(false, "expected FormatException");
        }
        catch (FormatException)
        {
            Console.WriteLine("  classic: Assert.Throws / try-catch FormatException OK");
        }
    }

    private static bool IsNonEmpty(string? s) => !string.IsNullOrWhiteSpace(s);

    private sealed class Cart
    {
        private readonly List<decimal> _prices = [];
        public int Count => _prices.Count;
        public void Add(string _, decimal price) => _prices.Add(price);
        public decimal Total() => _prices.Sum();
    }

    private sealed class TempFolderFixture
    {
        public string Path { get; private set; } = "";
        public void Setup()
        {
            Path = System.IO.Path.Join(System.IO.Path.GetTempPath(), "learn-csharp-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }
        public void Teardown()
        {
            if (!string.IsNullOrEmpty(Path) && Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }
    }
}
