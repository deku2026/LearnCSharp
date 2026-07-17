// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第5部分-发布裁剪与AOT.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section05_PublishTrimmingAndAOT
// Item     : UnityIl2CppAndDotnet10Publish
// Topic id : stage10/section05/unity_il2cpp_and_dotnet10_publish
//
// Unity IL2CPP ↔ .NET AOT/裁剪同构；.NET 10 发布相关新点。

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section05;

internal static class UnityIl2CppAndDotnet10Publish
{
    [LearnTopic("stage10/section05/unity_il2cpp_and_dotnet10_publish")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== UnityIl2CppAndDotnet10Publish ===");
        DemoSameProblemDomain();
        DemoFeatureMap();
        DemoDotnet10Highlights();
        DemoFileBasedAotDefault();
        DemoPracticalAdvice();
        return 0;
    }

    private static void DemoSameProblemDomain()
    {
        Console.WriteLine("-- same problem domain --");
        Console.WriteLine("  Unity IL2CPP: C# → C++ → 原生，剥离未用，限制反射");
        Console.WriteLine("  .NET Native AOT / trim: IL 链接 + 原生编译，限制同类");
        Console.WriteLine("  学会 AOT 安全编码 ≈ 在 Unity 少踩 IL2CPP 坑");
        string[] shared = ["static reachability", "reflection risk", "AOT-friendly libs", "link.xml / annotations"];
        foreach (string s in shared)
            Console.WriteLine($"  shared theme: {s}");
        Debug.Assert(shared.Length == 4);
    }

    private static void DemoFeatureMap()
    {
        Console.WriteLine("-- feature map (conceptual) --");
        (string Dotnet, string UnityIsh)[] map =
        [
            ("PublishTrimmed / ILLink", "Managed stripping / link.xml"),
            ("PublishAot", "IL2CPP backend"),
            ("DynamicallyAccessedMembers", "preserve / [Preserve] 思路"),
            ("RequiresUnreferencedCode", "“此 API 在裁剪下危险”文档化"),
            ("source generators", "避免运行时 emit/反射"),
            ("RID-specific publish", "按玩家平台出包"),
        ];
        foreach ((string? dotnet, string? unity) in map)
            Console.WriteLine($"  {dotnet,-32} ≈ {unity}");
        Debug.Assert(map.Length >= 5);
    }

    private static void DemoDotnet10Highlights()
    {
        Console.WriteLine("-- .NET 10 publish highlights (study map) --");
        string[] points =
        [
            "file-based apps 可走默认 Native AOT 发布路径（可用属性关掉）",
            "平台特定运行时包 / any RID 资产模型继续演进",
            "容器场景：更易出小镜像（配合 trim/AOT）",
            "工具链与分析器对 trim 警告更严、更早",
        ];
        foreach (string p in points)
            Console.WriteLine($"  • {p}");
        Debug.Assert(points.Length == 4);
        Console.WriteLine($"  current: {RuntimeInformation.FrameworkDescription}");
    }

    private static void DemoFileBasedAotDefault()
    {
        Console.WriteLine("-- file-based app AOT knobs --");
        Console.WriteLine("  单文件 app.cs 发布可能默认 AOT（以 SDK 文档为准）");
        Console.WriteLine("  关闭: #:property PublishAot=false");
        Console.WriteLine("  或项目: <PublishAot>false</PublishAot>");
        string[] directives =
        [
            "#:property PublishAot=false",
            "#:property PublishTrimmed=true",
            "#:property RuntimeIdentifier=win-x64",
        ];
        foreach (string d in directives)
            Console.WriteLine($"  {d}");
        Debug.Assert(directives[0].Contains("PublishAot", StringComparison.Ordinal));
    }

    private static void DemoPracticalAdvice()
    {
        Console.WriteLine("-- practical advice --");
        string[] tips =
        [
            "先 FDD 跑通，再 SCD，再 trim，最后 AOT（增量加压）",
            "每个阶段跑相同集成测试于 publish 输出",
            "游戏/Unity 开发者：把 link 警告当一等公民",
            "库作者：IsAotCompatible + 注解是礼貌",
            "不要在未验证的 trim 产物上直接生产放量",
        ];
        foreach (string t in tips)
            Console.WriteLine($"  → {t}");
        Debug.Assert(tips.Length == 5);
    }
}
