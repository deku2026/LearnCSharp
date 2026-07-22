// LearnCSharp example (filled)
// Doc      : CSharp-阶段1-语法基础与程序结构-详解.md
// Stage    : Stage01_SyntaxAndProgramStructure
// Section  : Section01_LanguageBasics
// Item     : HelloWorldDissection
// Topic id : stage01/section01/hello_world_dissection
//
// 步骤 2：Hello World 逐字解剖——类/静态方法/字面值/分号 + 编译器合成入口。

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage01.Section01;

internal static class HelloWorldDissection
{
    [LearnTopic("stage01/section01/hello_world_dissection")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== HelloWorldDissection ===");
        DemoOneLinerParts();
        DemoImplicitUsings();
        DemoCompilerSynthesis();
        DemoAssemblyEntryPoint();
        DemoIlIntuition();
        return 0;
    }

    private static void DemoOneLinerParts()
    {
        Console.WriteLine("-- 一行 Hello 里有什么 --");
        // System.Console 的静态方法；字符串字面值；语句以 ; 结束
        Console.WriteLine("Hello, World!");

        Type consoleType = typeof(Console);
        Debug.Assert(consoleType == typeof(Console));
        Debug.Assert(consoleType.GetMethod(nameof(Console.WriteLine), [typeof(string)]) is not null);

        string message = "Hello, World!";
        Debug.Assert(message.Length == 13);
        Console.WriteLine($"  Console → {consoleType.FullName}");
        Console.WriteLine("  WriteLine → 静态方法，写 stdout 并追加 Environment.NewLine");
        Console.WriteLine($"  字面值: \"{message}\"，语句定界符是 ';'");
        Console.WriteLine($"  NewLine 长度={Environment.NewLine.Length} (Windows 常为 2: \\r\\n)");
    }

    private static void DemoImplicitUsings()
    {
        Console.WriteLine("-- 为何能直接写 Console --");
        // ImplicitUsings 自动 global using System; 关掉后须写全名或 using
        System.Console.WriteLine("  全限定: System.Console.WriteLine(...) 永远可用");
        Console.WriteLine("  短名: Console 依赖 implicit usings / using System;");

        string shortName = nameof(Console);
        string fullName = typeof(Console).FullName!;
        Debug.Assert(shortName == "Console");
        Debug.Assert(fullName == "System.Console");
    }

    private static void DemoCompilerSynthesis()
    {
        Console.WriteLine("-- 顶级语句时编译器合成什么 --");
        // SharpLab Results:C# 可见类似:
        //   internal class Program {
        //     private static void <Main>$(string[] args) {
        //       Console.WriteLine("Hello, World!");
        //     }
        //   }
        Console.WriteLine("  合成类名约定: Program（可 partial class Program 追加成员）");
        Console.WriteLine("  入口方法名: <Main>$ —— 实现细节，代码不能直接引用");
        Console.WriteLine("  带 string[] args；类放在全局命名空间");
        Console.WriteLine("  🔶 C++: main 必须显式；C# 可把函数体裸写在文件顶层");

        string synthesizedClass = "Program";
        string synthesizedEntry = "<Main>$";
        Debug.Assert(synthesizedClass == "Program");
        Debug.Assert(synthesizedEntry.Contains("Main", StringComparison.Ordinal));
    }

    private static void DemoAssemblyEntryPoint()
    {
        Console.WriteLine("-- 反射：本程序集入口点名 --");
        // 顶级语句 → 合成 <Main>$；显式 static void/int Main → "Main"
        Assembly asm = typeof(HelloWorldDissection).Assembly;
        MethodInfo? entry = asm.EntryPoint;
        Debug.Assert(entry is not null);
        Debug.Assert(entry.IsStatic);
        Debug.Assert(entry.Name is "Main" or "<Main>$" || entry.Name.Contains("Main", StringComparison.Ordinal));
        Console.WriteLine($"  EntryPoint: {entry.DeclaringType?.FullName}.{entry.Name}");
        Console.WriteLine($"  ReturnType={entry.ReturnType.Name}; params={entry.GetParameters().Length}");
        Debug.Assert(entry.ReturnType == typeof(void) || entry.ReturnType == typeof(int) || entry.ReturnType == typeof(Task) || entry.ReturnType == typeof(Task<int>));
    }

    private static void DemoIlIntuition()
    {
        Console.WriteLine("-- IL 直觉 (SharpLab Results:IL) --");
        Console.WriteLine("  ldstr \"Hello, World!\"  → 加载字符串常量");
        Console.WriteLine("  call  System.Console::WriteLine(string)");
        Console.WriteLine("  ret");

        // 本宿主里 Run 本身就是显式入口方法，行为等价于合成后的调用
        Console.WriteLine("  (本 demo 模拟一次 call Console.WriteLine)");
        string payload = "Hello, World!";
        Debug.Assert(payload.Length > 0);
        Debug.Assert(typeof(Console).GetMethod(nameof(Console.WriteLine), [typeof(string)])!.IsStatic);
    }
}
