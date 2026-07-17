// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第2部分-函数成员与构造.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section02_FunctionMembersAndConstruction
// Item     : ExtensionMembersCSharp14
// Topic id : stage03/section02/extension_members_csharp14
//
// 步骤 4：经典 this 扩展方法 + C#14 extension 块(属性/方法)。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section02;

internal static class ExtensionMembersCSharp14
{
    [LearnTopic("stage03/section02/extension_members_csharp14")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ExtensionMembersCSharp14 ===");
        DemoClassicThisExtensions();
        DemoExtensionBlockProperties();
        DemoGenericEnumerableExtensions();
        return 0;
    }

    private static void DemoClassicThisExtensions()
    {
        Console.WriteLine("-- 经典 this 扩展方法 --");
        Debug.Assert(4.IsEven());
        Debug.Assert(!5.IsEven());
        Debug.Assert("hello world".WordCountClassic() == 2);
        Console.WriteLine($"  4.IsEven={4.IsEven()}, \"hello world\".WordCountClassic=2");
    }

    private static void DemoExtensionBlockProperties()
    {
        Console.WriteLine("-- C#14 extension 块：扩展属性/方法 --");
        string title = "Hello World";
        Debug.Assert(title.WordCount == 2);
        Debug.Assert(!title.IsBlank);
        Debug.Assert("   ".IsBlank);
        Debug.Assert(title.Repeat(2) == "Hello WorldHello World");
        Console.WriteLine($"  WordCount={title.WordCount}, IsBlank={title.IsBlank}");
        Console.WriteLine($"  Repeat(2)={title.Repeat(2)}");
    }

    private static void DemoGenericEnumerableExtensions()
    {
        Console.WriteLine("-- 泛型 extension：IEnumerable 扩展属性 --");
        int[] empty = [];
        int[] data = [1, 2, 3];
        Debug.Assert(empty.IsEmpty);
        Debug.Assert(!data.IsEmpty);
        Debug.Assert(data.SecondOrDefault() == 2);
        Console.WriteLine($"  empty.IsEmpty={empty.IsEmpty}, data.SecondOrDefault={data.SecondOrDefault()}");
    }
}

file static class Stage03Section02Extensions
{
    // 经典扩展方法
    public static bool IsEven(this int n) => n % 2 == 0;

    public static int WordCountClassic(this string s) =>
        s.Split([' ', '.', '?'], StringSplitOptions.RemoveEmptyEntries).Length;

    // C#14 extension 块
    extension(string s)
    {
        public int WordCount =>
            s.Split([' ', '.', '?'], StringSplitOptions.RemoveEmptyEntries).Length;

        public bool IsBlank => string.IsNullOrWhiteSpace(s);

        public string Repeat(int n) => string.Concat(Enumerable.Repeat(s, n));
    }

    extension<T>(IEnumerable<T> source)
    {
        public bool IsEmpty => !source.Any();

        public T? SecondOrDefault()
        {
            using IEnumerator<T> e = source.GetEnumerator();
            if (!e.MoveNext()) return default;
            if (!e.MoveNext()) return default;
            return e.Current;
        }
    }
}
