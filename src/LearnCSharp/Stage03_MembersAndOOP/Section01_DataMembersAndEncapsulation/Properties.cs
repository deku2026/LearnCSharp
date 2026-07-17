// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第1部分-数据成员与封装.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section01_DataMembersAndEncapsulation
// Item     : Properties
// Topic id : stage03/section01/properties
//
// 步骤 2：自动属性、init/required、field(C#14)、计算属性、访问器访问级别。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section01;

internal static class Properties
{
    [LearnTopic("stage03/section01/properties")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Properties ===");
        DemoAutoAndInitRequired();
        DemoFieldKeywordValidation();
        DemoComputedAndExpressionBodied();
        DemoAccessorAccessLevels();
        return 0;
    }

    private static void DemoAutoAndInitRequired()
    {
        Console.WriteLine("-- 自动属性 / init / required --");
        Rectangle r = new Rectangle { Width = 3, Height = 4, Name = "R1" };
        Debug.Assert(r.Width == 3 && r.Height == 4);
        Debug.Assert(r.Name == "R1");
        Debug.Assert(r.Id == 0); // 只读自动属性默认 0，构造后不可写
        Console.WriteLine($"  {r.Name}: {r.Width}x{r.Height}");
        // r.Width = 9; // ❌ init 属性构造后不可改
    }

    private static void DemoFieldKeywordValidation()
    {
        Console.WriteLine("-- C#14 field 关键字：自动后台字段 + 自定义 set --");
        ValidatedPerson p = new ValidatedPerson { FirstName = "Jane", Age = 30 };
        Debug.Assert(p.FirstName == "Jane");
        Debug.Assert(p.Age == 30);
        try
        {
            p.Age = -1;
            Debug.Assert(false, "should throw");
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("  Age=-1 rejected via field set");
        }
        try
        {
            p.FirstName = "  ";
            Debug.Assert(false, "should throw");
        }
        catch (ArgumentException)
        {
            Console.WriteLine("  blank FirstName rejected");
        }
    }

    private static void DemoComputedAndExpressionBodied()
    {
        Console.WriteLine("-- 计算属性 / 表达式主体 --");
        Rectangle r = new Rectangle { Width = 3, Height = 4, Name = "box" };
        Debug.Assert(Math.Abs(r.Area - 12) < 1e-9);
        Debug.Assert(r.Label == "box(3x4)");
        Console.WriteLine($"  Area={r.Area}, Label={r.Label}");
    }

    private static void DemoAccessorAccessLevels()
    {
        Console.WriteLine("-- 访问器不同访问级别 --");
        TokenBox t = new TokenBox("secret");
        Debug.Assert(t.Token == "secret");
        t.Rotate("next");
        Debug.Assert(t.Token == "next");
        Console.WriteLine($"  Token(get public, set private)={t.Token}");
    }

    private sealed class Rectangle
    {
        public double Width { get; init; }
        public double Height { get; init; }
        public required string Name { get; init; }
        public int Id { get; } // 只读自动属性
        public double Area => Width * Height; // 计算属性，无后台字段
        public string Label => $"{Name}({Width}x{Height})";
    }

    private sealed class ValidatedPerson
    {
        // C#14 field：混搭自动 get + 自定义 set
        public string FirstName
        {
            get;
            set => field = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("不能为空", nameof(value))
                : value;
        } = "Jane";

        public int Age
        {
            get;
            set => field = value < 0
                ? throw new ArgumentOutOfRangeException(nameof(value))
                : value;
        }
    }

    private sealed class TokenBox
    {
        public string Token { get; private set; }
        public TokenBox(string token) => Token = token;
        public void Rotate(string next) => Token = next;
    }
}
