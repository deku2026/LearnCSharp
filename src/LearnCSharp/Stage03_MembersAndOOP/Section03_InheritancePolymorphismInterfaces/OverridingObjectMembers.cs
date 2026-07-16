// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第3部分-继承多态接口.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section03_InheritancePolymorphismInterfaces
// Item     : OverridingObjectMembers
// Topic id : stage03/section03/overriding_object_members
//
// 步骤 5：ToString / Equals / GetHashCode 成对重写；record 自动生成。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section03;

internal static class OverridingObjectMembers
{
    [LearnTopic("stage03/section03/overriding_object_members")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== OverridingObjectMembers ===");
        DemoToStringEqualsHashCode();
        DemoDictionaryKey();
        DemoRecordAutoOverrides();
        DemoGetTypeNonVirtual();
        return 0;
    }

    private static void DemoToStringEqualsHashCode()
    {
        Console.WriteLine("-- 手动重写三件套 --");
        var a = new Money(10m, "USD");
        var b = new Money(10m, "USD");
        var c = new Money(20m, "USD");
        Debug.Assert(a.ToString() == "10 USD");
        Debug.Assert(a.Equals(b));
        Debug.Assert(!a.Equals(c));
        Debug.Assert(a.GetHashCode() == b.GetHashCode());
        Console.WriteLine($"  a={a}, a.Equals(b)={a.Equals(b)}");
    }

    private static void DemoDictionaryKey()
    {
        Console.WriteLine("-- 作 Dictionary key --");
        var map = new Dictionary<Money, string>
        {
            [new Money(5m, "EUR")] = "five",
        };
        Debug.Assert(map[new Money(5m, "EUR")] == "five");
        Console.WriteLine($"  map[5 EUR]={map[new Money(5m, "EUR")]}");
    }

    private static void DemoRecordAutoOverrides()
    {
        Console.WriteLine("-- record 自动 ToString/Equals/GetHashCode --");
        var p1 = new PointRec(1, 2);
        var p2 = new PointRec(1, 2);
        Debug.Assert(p1 == p2);
        Debug.Assert(p1.ToString().Contains("1"));
        Debug.Assert(p1.GetHashCode() == p2.GetHashCode());
        Console.WriteLine($"  record {p1} == {p2}: {p1 == p2}");
    }

    private static void DemoGetTypeNonVirtual()
    {
        Console.WriteLine("-- GetType 非虚 --");
        object o = new Money(1m, "JPY");
        Debug.Assert(o.GetType() == typeof(Money));
        Console.WriteLine($"  GetType={o.GetType().Name}");
    }

    private sealed class Money
    {
        public decimal Amount { get; }
        public string Currency { get; }
        public Money(decimal amount, string currency) => (Amount, Currency) = (amount, currency);

        public override string ToString() => $"{Amount} {Currency}";

        public override bool Equals(object? obj) =>
            obj is Money m && Amount == m.Amount && Currency == m.Currency;

        public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    }

    private sealed record PointRec(int X, int Y);
}
