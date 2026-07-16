// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第2部分-NuGet包管理.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section02_NuGetPackageManagement
// Item     : DotnetPackPublish
// Topic id : stage10/section02/dotnet_pack_publish
//
// 打包元数据、dotnet pack、PrivateAssets / IncludeAssets。

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section02;

internal static class DotnetPackPublish
{
    [LearnTopic("stage10/section02/dotnet_pack_publish")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DotnetPackPublish ===");
        DemoPackVsPublish();
        DemoPackageMetadata();
        DemoNupkgLayout();
        DemoPrivateAssets();
        DemoPushFlow();
        return 0;
    }

    private static void DemoPackVsPublish()
    {
        Console.WriteLine("-- pack vs publish --");
        Console.WriteLine("  dotnet pack   → 生成 .nupkg（给别人引用）");
        Console.WriteLine("  dotnet publish → 部署应用输出（给机器运行）");
        Console.WriteLine("  库主打 pack；可执行应用主打 publish");
        string[] cmds = ["dotnet pack -c Release", "dotnet publish -c Release"];
        Debug.Assert(cmds[0].Contains("pack") && cmds[1].Contains("publish"));
        foreach (string c in cmds)
            Console.WriteLine($"  $ {c}");
    }

    private static void DemoPackageMetadata()
    {
        Console.WriteLine("-- package metadata properties --");
        (string Prop, string Role)[] meta =
        [
            ("PackageId", "NuGet ID（默认=AssemblyName）"),
            ("Version", "包版本"),
            ("Authors", "作者"),
            ("Description", "说明"),
            ("PackageTags", "搜索标签"),
            ("PackageProjectUrl", "项目主页"),
            ("PackageLicenseExpression", "如 MIT"),
            ("RepositoryUrl", "源码仓"),
            ("GeneratePackageOnBuild", "build 时自动 pack"),
            ("IsPackable", "false 禁止被 pack（应用项目）"),
        ];
        foreach (var (prop, role) in meta)
            Console.WriteLine($"  {prop,-28} {role}");
        Debug.Assert(meta.Length >= 8);

        AssemblyName name = typeof(DotnetPackPublish).Assembly.GetName();
        Console.WriteLine($"  本程序集名可作 PackageId 默认: {name.Name}");
    }

    private static void DemoNupkgLayout()
    {
        Console.WriteLine("-- nupkg layout (conceptual) --");
        string[] layout =
        [
            "MyLib.1.2.3.nupkg  (zip)",
            "  MyLib.nuspec",
            "  lib/net10.0/MyLib.dll",
            "  lib/net8.0/MyLib.dll",
            "  analyzers/dotnet/cs/...",
            "  build/MyLib.targets  (optional MSBuild inject)",
            "  README.md / icon.png",
        ];
        foreach (string line in layout)
            Console.WriteLine($"  {line}");
        Debug.Assert(layout.Any(l => l.Contains("lib/")));
    }

    private static void DemoPrivateAssets()
    {
        Console.WriteLine("-- PrivateAssets / IncludeAssets --");
        Console.WriteLine("  默认: 依赖会作为传递依赖流给上层");
        Console.WriteLine("  PrivateAssets=\"all\": 只给本项目用，不流给消费者");
        Console.WriteLine("  典型: 分析器包、私有实现细节、源代码生成器");
        Console.WriteLine("  IncludeAssets: 控制是否带 compile/runtime/build/analyzers…");
        string sample = """
          <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
          </PackageReference>
          """;
        Debug.Assert(sample.Contains("PrivateAssets", StringComparison.Ordinal));
        Console.WriteLine("  (snippet printed conceptually — analyzers stay private)");
    }

    private static void DemoPushFlow()
    {
        Console.WriteLine("-- publish package to feed --");
        string[] flow =
        [
            "dotnet pack -c Release -o ./artifacts",
            "dotnet nuget push ./artifacts/*.nupkg -s <source> -k <api-key>",
        ];
        foreach (string f in flow)
            Console.WriteLine($"  $ {f}");
        Debug.Assert(flow[1].Contains("nuget push", StringComparison.Ordinal));
        Console.WriteLine("  密钥走环境变量/CI secret，勿写入仓库");
    }
}
