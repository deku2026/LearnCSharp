// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第1部分-控制流.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section01_ControlFlow
// Item     : Conditionals
// Topic id : stage04/section01/conditionals
//
// 步骤 1：if/else、三元 ?:、空合并 ??/??=、空条件 ?./?[]。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section01;

internal static class Conditionals
{
    [LearnTopic("stage04/section01/conditionals")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Conditionals ===");
        DemoIfElse();
        DemoMustBeBool();
        DemoTernary();
        DemoNullCoalescing();
        DemoNullConditional();
        DemoNullConditionalChain();
        return 0;
    }

    private static void DemoIfElse()
    {
        Console.WriteLine("-- if / else if / else --");
        string Grade(int score)
        {
            if (score >= 90)
                return "A";
            else if (score >= 60)
                return "Pass";
            else
                return "Fail";
        }

        Debug.Assert(Grade(95) == "A");
        Debug.Assert(Grade(75) == "Pass");
        Debug.Assert(Grade(40) == "Fail");
        Console.WriteLine($"  95→{Grade(95)}, 75→{Grade(75)}, 40→{Grade(40)}");
    }

    private static void DemoMustBeBool()
    {
        Console.WriteLine("-- 条件必须是 bool（C++ 差异） --");
        int n = 5;
        // if (n) { } // ❌ 编译错误：不能把 int 当 bool
        if (n != 0)
            Console.WriteLine($"  if (n != 0) OK, n={n}");

        string? s = null;
        // if (s) { } // ❌
        if (s is not null)
            Debug.Assert(false);
        else
            Console.WriteLine("  if (s is not null) 判空须显式");

        Debug.Assert(n != 0);
        Debug.Assert(s is null);
    }

    private static void DemoTernary()
    {
        Console.WriteLine("-- 条件运算符 ?: --");
        int a = 10, b = 3;
        int max = a > b ? a : b;
        string label = 75 >= 60 ? "Pass" : "Fail";
        Debug.Assert(max == 10 && label == "Pass");

        int x = 1, y = 2;
        bool preferX = true;
        ref int r = ref (preferX ? ref x : ref y);
        r = 99;
        Debug.Assert(x == 99 && y == 2);
        Console.WriteLine($"  max={max}, label={label}, ref 三元后 x={x}");
    }

    private static void DemoNullCoalescing()
    {
        Console.WriteLine("-- ?? 与 ??= --");
        string? name = null;
        string display = name ?? "(匿名)";
        Debug.Assert(display == "(匿名)");

        name ??= "默认名";
        Debug.Assert(name == "默认名");
        name ??= "不会覆盖";
        Debug.Assert(name == "默认名");

        int? age = null;
        int shown = age ?? -1;
        Debug.Assert(shown == -1);
        Console.WriteLine($"  null ?? \"(匿名)\" → {display}; ??= 惰性赋值 → {name}");
    }

    private static void DemoNullConditional()
    {
        Console.WriteLine("-- 空条件 ?. 与 ?[] --");
        string? name = null;
        int? len = name?.Length;
        char? first = name?[0];
        Debug.Assert(len is null && first is null);

        name = "Ada";
        len = name?.Length;
        first = name?[0];
        Debug.Assert(len == 3 && first == 'A');
        Console.WriteLine($"  null?.Length=null; \"Ada\"?.Length={len}, [0]={first}");
    }

    private static void DemoNullConditionalChain()
    {
        Console.WriteLine("-- ?. 链式短路 --");
        Person? p = null;
        string? city = p?.Address?.City?.ToUpper();
        Debug.Assert(city is null);

        p = new Person("Bob", new Address("Seattle", "US"));
        city = p?.Address?.City?.ToUpper();
        Debug.Assert(city == "SEATTLE");

        p = new Person("NoAddr", null);
        city = p?.Address?.City?.ToUpper();
        Debug.Assert(city is null);
        Console.WriteLine("  a?.B?.C：任一环 null → 整体 null，不抛 NRE");
    }

    private sealed class Person(string name, Address? address)
    {
        public string Name { get; } = name;
        public Address? Address { get; } = address;
    }

    private sealed class Address(string city, string country)
    {
        public string City { get; } = city;
        public string Country { get; } = country;
    }
}
