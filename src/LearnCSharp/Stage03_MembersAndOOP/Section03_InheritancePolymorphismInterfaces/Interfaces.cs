// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第3部分-继承多态接口.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section03_InheritancePolymorphismInterfaces
// Item     : Interfaces
// Topic id : stage03/section03/interfaces
//
// 步骤 6：接口契约、多实现、接口继承、隐式/显式实现。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section03;

internal static class Interfaces
{
    [LearnTopic("stage03/section03/interfaces")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Interfaces ===");
        DemoInterfaceAsContract();
        DemoInterfaceInheritance();
        DemoExplicitImplementation();
        DemoStructImplementsInterface();
        return 0;
    }

    private static void DemoInterfaceAsContract()
    {
        Console.WriteLine("-- 面向接口编程 --");
        ILogger logger = new ConsoleLogger();
        string msg = logger.Log("hello");
        Debug.Assert(msg == "LOG:hello");
        Console.WriteLine($"  {msg}");
    }

    private static void DemoInterfaceInheritance()
    {
        Console.WriteLine("-- 接口多继承(契约组合) --");
        IReadWritable buf = new Buffer();
        buf.Write("data");
        Debug.Assert(buf.Read() == "data");
        IReadable r = buf;
        IWritable w = buf;
        w.Write("x");
        Debug.Assert(r.Read() == "x");
        Console.WriteLine($"  Buffer via IReadWritable: {buf.Read()}");
    }

    private static void DemoExplicitImplementation()
    {
        Console.WriteLine("-- 显式实现：同名消歧 + 隐藏公开 API --");
        var p = new Panel();
        // p.Paint(); // ❌ 不在类公开 API
        Debug.Assert(((IControl)p).Paint() == "IControl.Paint");
        Debug.Assert(((ISurface)p).Paint() == "ISurface.Paint");
        Console.WriteLine("  ((IControl)p).Paint / ((ISurface)p).Paint distinct");
    }

    private static void DemoStructImplementsInterface()
    {
        Console.WriteLine("-- struct 也可实现接口 --");
        IComparable<Score> a = new Score(10);
        IComparable<Score> b = new Score(20);
        Debug.Assert(a.CompareTo((Score)b) < 0);
        Console.WriteLine($"  Score(10) vs Score(20): {a.CompareTo((Score)b)}");
    }

    private interface ILogger
    {
        string Log(string message);
    }

    private sealed class ConsoleLogger : ILogger
    {
        public string Log(string message) => $"LOG:{message}";
    }

    private interface IReadable
    {
        string Read();
    }

    private interface IWritable
    {
        void Write(string value);
    }

    private interface IReadWritable : IReadable, IWritable { }

    private sealed class Buffer : IReadWritable
    {
        private string _data = "";
        public string Read() => _data;
        public void Write(string value) => _data = value;
    }

    private interface IControl
    {
        string Paint();
    }

    private interface ISurface
    {
        string Paint();
    }

    private sealed class Panel : IControl, ISurface
    {
        string IControl.Paint() => "IControl.Paint";
        string ISurface.Paint() => "ISurface.Paint";
    }

    private readonly struct Score(int value) : IComparable<Score>
    {
        public int Value { get; } = value;
        public int CompareTo(Score other) => Value.CompareTo(other.Value);
    }
}
