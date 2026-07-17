// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第2部分-上下文关键字与C#14专题.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section02_ContextualKeywordsAndCSharp14
// Item     : CSharp14Features
// Topic id : stage08/section02/csharp14_features
//
// C#14：field / extension / 复合赋值与实例++ / partial 构造与事件 / null 条件赋值 /
// nameof 未绑定泛型 / 简单 lambda 修饰符 / 一等 Span。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section02;

internal static class CSharp14Features
{
    [LearnTopic("stage08/section02/csharp14_features")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CSharp14Features ===");
        DemoFieldKeyword();
        DemoExtensionMembers();
        DemoCompoundAssignmentAndIncrement();
        DemoPartialConstructorAndEvent();
        DemoNullConditionalAssignment();
        DemoNameofUnboundGeneric();
        DemoLambdaParameterModifiers();
        DemoFirstClassSpan();
        return 0;
    }

    private static void DemoFieldKeyword()
    {
        Console.WriteLine("-- field 关键字 --");
        FieldPerson p = new FieldPerson { Name = "Ada" };
        Debug.Assert(p.Name == "Ada");
        try
        {
            p.Name = "  ";
            Debug.Assert(false);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("  blank Name rejected via field set");
        }
        p.Age = 30;
        Debug.Assert(p.Age == 30);
        Console.WriteLine($"  Name={p.Name}, Age={p.Age}");
    }

    private static void DemoExtensionMembers()
    {
        Console.WriteLine("-- extension 块：属性/方法 --");
        string s = "hello world";
        Debug.Assert(s.WordCount == 2);
        Debug.Assert(!s.IsBlank);
        Debug.Assert("   ".IsBlank);
        Debug.Assert(s.Stage08Repeat(2) == "hello worldhello world");
        Console.WriteLine($"  WordCount={s.WordCount}, IsBlank={s.IsBlank}");
    }

    private static void DemoCompoundAssignmentAndIncrement()
    {
        Console.WriteLine("-- 用户定义 += / 实例 ++ --");
        Vec3 v = new Vec3(1, 2, 3);
        v += new Vec3(4, 5, 6);
        Debug.Assert(v == new Vec3(5, 7, 9));
        Counter c = new Counter(10);
        c++;
        Debug.Assert(c.Value == 11);
        Console.WriteLine($"  Vec3 after += {v}, Counter={c.Value}");
    }

    private static void DemoPartialConstructorAndEvent()
    {
        Console.WriteLine("-- partial 构造 / 事件 --");
        PartialHost h = new PartialHost(42);
        Debug.Assert(h.Value == 42);
        int fired = 0;
        h.Tick += (_, n) => fired += n;
        h.Raise(3);
        Debug.Assert(fired == 3);
        Console.WriteLine($"  PartialHost.Value={h.Value}, event fired={fired}");
    }

    private static void DemoNullConditionalAssignment()
    {
        Console.WriteLine("-- null 条件赋值 ?. 作左值 --");
        int calls = 0;
        Order Make()
        {
            calls++;
            return new Order { Id = 1 };
        }

        Customer? c = null;
        c?.Order = Make();
        Debug.Assert(calls == 0); // 右侧未求值
        Debug.Assert(c is null);

        c = new Customer();
        c?.Order = Make();
        Debug.Assert(calls == 1);
        Debug.Assert(c!.Order is { Id: 1 });

        int[]? arr = null;
        arr?[0] = 9; // 跳过
        arr = [0, 0];
        arr?[0] = 9;
        Debug.Assert(arr![0] == 9);
        Console.WriteLine($"  null skip calls={calls - 1}, after assign Id={c.Order!.Id}, arr[0]={arr[0]}");
    }

    private static void DemoNameofUnboundGeneric()
    {
        Console.WriteLine("-- nameof 未绑定泛型 --");
        string n1 = nameof(List<>);
        string n2 = nameof(Dictionary<,>);
        Debug.Assert(n1 == "List");
        Debug.Assert(n2 == "Dictionary");
        Console.WriteLine($"  nameof(List<>)={n1}, nameof(Dictionary<,>)={n2}");
    }

    private static void DemoLambdaParameterModifiers()
    {
        Console.WriteLine("-- 简单 lambda 参数修饰符 --");
        var inc = (ref int x) => x++;
        int n = 5;
        inc(ref n);
        Debug.Assert(n == 6);

        // 隐式类型 + 修饰符（C#14）
        TryParseHandler tryParse = (string s, out int v) => int.TryParse(s, out v);
        bool parsedOk = tryParse("42", out int parsed);
        Debug.Assert(parsedOk && parsed == 42);
        Console.WriteLine($"  ref lambda n={n}, tryParse={parsed}");
    }

    private static void DemoFirstClassSpan()
    {
        Console.WriteLine("-- 一等 Span 隐式转换 --");
        int[] data = [1, 2, 3, 4];
        Span<int> span = data; // 数组 → Span
        ReadOnlySpan<int> ros = data;
        Debug.Assert(span.Length == 4 && ros[0] == 1);
        Span<int> slice = span[1..3];
        Debug.Assert(slice is [2, 3]);
        string text = "abcd";
        ReadOnlySpan<char> chars = text;
        Debug.Assert(chars.Length == 4 && chars[0] == 'a');
        Console.WriteLine($"  Span len={span.Length}, slice=[{slice[0]},{slice[1]}], chars[0]={chars[0]}");
    }

    private delegate bool TryParseHandler(string s, out int value);

    private sealed class FieldPerson
    {
        public string Name
        {
            get;
            set => field = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("empty", nameof(value))
                : value;
        } = "";

        public int Age
        {
            get;
            set => field = value < 0
                ? throw new ArgumentOutOfRangeException(nameof(value))
                : value;
        }
    }

    private record struct Vec3(double X, double Y, double Z)
    {
        public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public void operator +=(Vec3 r)
        {
            X += r.X;
            Y += r.Y;
            Z += r.Z;
        }
    }

    private record struct Counter(int Value)
    {
        public void operator ++() => Value++;
    }

    private sealed partial class PartialHost
    {
        public int Value { get; private set; }
        public partial PartialHost(int value);
        public partial event EventHandler<int>? Tick;
        public void Raise(int n) => OnTick(n);
        private partial void OnTick(int n);
    }

    private sealed partial class PartialHost
    {
        private EventHandler<int>? _tick;

        public partial PartialHost(int value) => Value = value;

        public partial event EventHandler<int>? Tick
        {
            add => _tick += value;
            remove => _tick -= value;
        }

        private partial void OnTick(int n) => _tick?.Invoke(this, n);
    }

    private sealed class Customer
    {
        public Order? Order { get; set; }
    }

    private sealed class Order
    {
        public int Id { get; set; }
    }
}

file static class Stage08Section02CSharp14Extensions
{
    extension(string s)
    {
        public int WordCount =>
            s.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries).Length;

        public bool IsBlank => string.IsNullOrWhiteSpace(s);

        public string Stage08Repeat(int n) => string.Concat(Enumerable.Repeat(s, n));
    }
}
