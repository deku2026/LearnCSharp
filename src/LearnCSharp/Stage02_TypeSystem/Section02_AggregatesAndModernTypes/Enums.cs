// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第2部分-聚合与现代类型.md
// Stage    : Stage02_TypeSystem
// Section  : Section02_AggregatesAndModernTypes
// Item     : Enums
// Topic id : stage02/section02/enums
//
// 步骤 1：enum 底层整型、[Flags]、非穷尽、Enum API。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section02;

internal static class Enums
{
    [LearnTopic("stage02/section02/enums")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Enums ===");
        DemoBasics();
        DemoUnderlyingAndCast();
        DemoFlags();
        DemoNotClosed();
        DemoEnumApi();
        return 0;
    }

    private static void DemoBasics()
    {
        Console.WriteLine("-- 基础命名常量 --");
        Season s = Season.Summer;
        Debug.Assert(s.ToString() == "Summer");
        Debug.Assert((int)s == 1);
        Debug.Assert((Season)3 == Season.Winter);
        Console.WriteLine($"  {s} = {(int)s}");
    }

    private static void DemoUnderlyingAndCast()
    {
        Console.WriteLine("-- 底层类型与互转 --");
        LogLevel level = LogLevel.Warning;
        Debug.Assert(Enum.GetUnderlyingType(typeof(LogLevel)) == typeof(byte));
        byte raw = (byte)level;
        Debug.Assert(raw == 2);
        LogLevel back = (LogLevel)raw;
        Debug.Assert(back == LogLevel.Warning);
        Console.WriteLine($"  LogLevel underlying=byte, Warning={(byte)LogLevel.Warning}");
    }

    private static void DemoFlags()
    {
        Console.WriteLine("-- [Flags] 位标志 --");
        FileAccess p = FileAccess.Read | FileAccess.Write;
        Debug.Assert(p.HasFlag(FileAccess.Write));
        Debug.Assert((p & FileAccess.Write) == FileAccess.Write);
        Debug.Assert(!p.HasFlag(FileAccess.Execute));
        Console.WriteLine($"  Read|Write ToString={p}");
        Debug.Assert(p.ToString().Contains("Read", StringComparison.Ordinal));
    }

    private static void DemoNotClosed()
    {
        Console.WriteLine("-- ⚠ 枚举不封闭：(Season)99 合法 --");
        Season weird = (Season)99;
        Debug.Assert(!Enum.IsDefined(weird));
        Console.WriteLine($"  (Season)99={weird}, IsDefined={Enum.IsDefined(weird)}");
        Console.WriteLine("  switch 处理枚举要留 _ 兜底或 IsDefined 校验");
    }

    private static void DemoEnumApi()
    {
        Console.WriteLine("-- System.Enum API --");
        string[] names = Enum.GetNames<Season>();
        Debug.Assert(names.Length == 4);
        Debug.Assert(Enum.Parse<Season>("Autumn") == Season.Autumn);
        Debug.Assert(!Enum.TryParse("Nope", out Season _));
        Console.WriteLine($"  GetNames count={names.Length}");
    }

    private enum Season { Spring, Summer, Autumn, Winter }

    private enum LogLevel : byte { Trace = 0, Info = 1, Warning = 2, Error = 3 }

    [Flags]
    private enum FileAccess
    {
        None = 0,
        Read = 1 << 0,
        Write = 1 << 1,
        Execute = 1 << 2,
        ReadWrite = Read | Write
    }
}
