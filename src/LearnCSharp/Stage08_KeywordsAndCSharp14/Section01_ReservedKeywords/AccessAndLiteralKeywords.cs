// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : AccessAndLiteralKeywords (十一、访问与字面量关键字 — 5 个)
// Topic id : stage08/section01/access_and_literal_keywords
//
// this / base / true / false / null。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class AccessAndLiteralKeywords
{
    [LearnTopic("stage08/section01/access_and_literal_keywords")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AccessAndLiteralKeywords ===");
        DemoThisAndIndexer();
        DemoBase();
        DemoTrueFalseNull();
        DemoAtEscapeKeyword();
        return 0;
    }

    private static void DemoThisAndIndexer()
    {
        Console.WriteLine("-- this / 索引器 --");
        NumberBag bag = new NumberBag(1, 2, 3);
        Debug.Assert(bag[0] == 1);
        bag[1] = 20;
        Debug.Assert(bag[1] == 20);
        Debug.Assert(bag.SelfSum() == 24);
        Console.WriteLine($"  bag[1]={bag[1]}, SelfSum={bag.SelfSum()}");
    }

    private static void DemoBase()
    {
        Console.WriteLine("-- base --");
        Child d = new Child("kid");
        Debug.Assert(d.Describe() == "Parent:kid|Child:kid");
        Console.WriteLine($"  Describe={d.Describe()}");
    }

    private static void DemoTrueFalseNull()
    {
        Console.WriteLine("-- true / false / null --");
        bool t = true, f = false;
        string? s = null;
        Debug.Assert(t && !f);
        Debug.Assert(s is null);
        s ??= "default";
        Debug.Assert(s == "default");
        string? maybe = null;
        Debug.Assert(maybe?.Length is null);
        Console.WriteLine($"  true={t}, false={f}, null-coalesce={s}");
    }

    private static void DemoAtEscapeKeyword()
    {
        Console.WriteLine("-- @ 转义关键字作标识符 --");
        int @class = 1;
        int @return = 2;
        Debug.Assert(@class + @return == 3);
        Console.WriteLine($"  @class+@return={@class + @return}");
    }

    private sealed class NumberBag
    {
        private readonly int[] _items;
        public NumberBag(params int[] items) => _items = items;
        public int this[int i]
        {
            get => _items[i];
            set => _items[i] = value;
        }
        public int SelfSum()
        {
            int s = 0;
            foreach (int x in this._items) s += x;
            return s;
        }
    }

    private class Parent
    {
        protected string Name { get; }
        public Parent(string name) => Name = name;
        public virtual string Describe() => $"Parent:{Name}";
    }

    private sealed class Child : Parent
    {
        public Child(string name) : base(name) { }
        public override string Describe() => $"{base.Describe()}|Child:{Name}";
    }
}
