// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第1部分-SDK项目与MSBuild构建.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section01_SdkProjectAndMSBuild
// Item     : KeyBuildProperties
// Topic id : stage10/section01/key_build_properties
//
// 关键构建属性：TFM、OutputType、LangVersion、Nullable、ImplicitUsings 等。

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section01;

internal static class KeyBuildProperties
{
    [LearnTopic("stage10/section01/key_build_properties")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== KeyBuildProperties ===");
        DemoTargetFramework();
        DemoCommonProperties();
        DemoConditionalCompilationSymbols();
        DemoVersionAttributes();
        return 0;
    }

    private static void DemoTargetFramework()
    {
        Console.WriteLine("-- TargetFramework / TargetFrameworks --");
        Console.WriteLine("  TFM = API surface + default lang version + runtime family");
        Console.WriteLine("  单目标: <TargetFramework>net10.0</TargetFramework>");
        Console.WriteLine("  多目标: <TargetFrameworks>net10.0;net8.0;netstandard2.0</TargetFrameworks>");
        Console.WriteLine("  OS 特定: net10.0-windows / net10.0-android / net10.0-ios");
        string[] tfms = ["net10.0", "net8.0", "netstandard2.0", "net10.0-windows"];
        Debug.Assert(tfms.All(t => t.StartsWith("net", StringComparison.Ordinal)));
        Console.WriteLine("  🔶 C++ -std=c++20 只管语言；TFM 同时钉住 BCL API 面");

        TargetFrameworkAttribute? tfa = typeof(KeyBuildProperties).Assembly
            .GetCustomAttribute<TargetFrameworkAttribute>();
        Console.WriteLine($"  本程序集 TFM 显示名: {tfa?.FrameworkDisplayName ?? tfa?.FrameworkName ?? "(n/a)"}");
        Debug.Assert(tfa is not null);
    }

    private static void DemoCommonProperties()
    {
        Console.WriteLine("-- common property cheat sheet --");
        (string Prop, string Meaning)[] rows =
        [
            ("OutputType", "Exe / Library / WinExe"),
            ("LangVersion", "C# 版本，建议钉 14.0 而非 latest"),
            ("Nullable", "enable / disable / warnings / annotations"),
            ("ImplicitUsings", "enable → 自动 global using"),
            ("RootNamespace", "默认命名空间前缀"),
            ("AssemblyName", "输出程序集名"),
            ("TreatWarningsAsErrors", "警告当错误（质量门禁）"),
            ("GenerateDocumentationFile", "输出 XML 文档"),
            ("InvariantGlobalization", "关掉全球化数据，利于 AOT 瘦身"),
            ("Configuration", "Debug / Release（-c 传入）"),
        ];
        foreach ((string? prop, string? meaning) in rows)
            Console.WriteLine($"  {prop,-28} {meaning}");
        Debug.Assert(rows.Length >= 8);
    }

    private static void DemoConditionalCompilationSymbols()
    {
        Console.WriteLine("-- multi-TFM conditional compilation --");
#if NET10_0_OR_GREATER
        const string symbol = "NET10_0_OR_GREATER";
        const bool isNet10OrGreater = true;
#else
        const string symbol = "(not NET10_0_OR_GREATER)";
        const bool isNet10OrGreater = false;
#endif
#if DEBUG
        const string config = "DEBUG";
#else
        const string config = "RELEASE";
#endif
        Console.WriteLine($"  active: {symbol}, Configuration symbol ≈ {config}");
        Debug.Assert(isNet10OrGreater);
        Console.WriteLine("  多目标时用 #if NET8_0 / NET10_0_OR_GREATER 写兼容层");
    }

    private static void DemoVersionAttributes()
    {
        Console.WriteLine("-- Version / AssemblyVersion / FileVersion --");
        Assembly asm = typeof(KeyBuildProperties).Assembly;
        AssemblyName name = asm.GetName();
        Console.WriteLine($"  AssemblyName.Version={name.Version}");
        AssemblyInformationalVersionAttribute? info = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        Console.WriteLine($"  InformationalVersion={info?.InformationalVersion ?? "(default)"}");
        Debug.Assert(name.Version is not null);
        Console.WriteLine("  csproj: Version 影响 NuGet；AssemblyVersion 影响强名称绑定");
    }
}
