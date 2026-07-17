// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第1部分-SDK项目与MSBuild构建.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section01_SdkProjectAndMSBuild
// Item     : SdkStyleProjectStructure
// Topic id : stage10/section01/sdk_style_project_structure
//
// SDK-style .csproj：读本仓库真实 csproj + global.json。

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section01;

internal static class SdkStyleProjectStructure
{
    [LearnTopic("stage10/section01/sdk_style_project_structure")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SdkStyleProjectStructure ===");
        DemoReadRealCsproj();
        DemoSdkAttributeMeaning();
        DemoGlobVsExplicitCompile();
        DemoGlobalJson();
        DemoRuntimeAssemblyEvidence();
        return 0;
    }

    private static void DemoReadRealCsproj()
    {
        Console.WriteLine("-- real LearnCSharp.csproj --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string path = Path.Combine(root, "src", "LearnCSharp", "LearnCSharp.csproj");
        string text = File.ReadAllText(path);
        Debug.Assert(text.Contains("Microsoft.NET.Sdk", StringComparison.Ordinal));
        Debug.Assert(text.Contains("OutputType", StringComparison.Ordinal));
        Debug.Assert(text.Contains("Exe", StringComparison.Ordinal));
        Console.WriteLine($"  path={path}");
        Console.WriteLine("  Sdk=Microsoft.NET.Sdk; OutputType=Exe; PackageReference via CPM");
        Console.WriteLine("  要点: 无 <Compile Include>；SDK 默认 glob **/*.cs");
    }

    private static void DemoSdkAttributeMeaning()
    {
        Console.WriteLine("-- Sdk= attribute injects MSBuild logic --");
        (string Sdk, string Role)[] sdks =
        [
            ("Microsoft.NET.Sdk", "console / classlib 默认"),
            ("Microsoft.NET.Sdk.Web", "ASP.NET Core"),
            ("Microsoft.NET.Sdk.Worker", "后台 Worker"),
            ("Microsoft.NET.Sdk.Razor", "Razor 类库"),
        ];
        foreach ((string? sdk, string? role) in sdks)
            Console.WriteLine($"  {sdk} → {role}");
        Debug.Assert(sdks[0].Sdk.EndsWith(".Sdk", StringComparison.Ordinal));
    }

    private static void DemoGlobVsExplicitCompile()
    {
        Console.WriteLine("-- SDK-style vs legacy explicit Compile --");
        Console.WriteLine("  legacy: 每个 .cs 都要 <Compile Include=\"File.cs\" />");
        Console.WriteLine("  SDK: 自动 glob；可用 <Compile Remove=...> 排除");
        string[] painPoints = ["explicit Compile list", "HintPath hell", "packages.config dual source"];
        Debug.Assert(painPoints.Length == 3);
    }

    private static void DemoGlobalJson()
    {
        Console.WriteLine("-- real global.json pins SDK --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string text = File.ReadAllText(Path.Combine(root, "global.json"));
        Debug.Assert(text.Contains("sdk", StringComparison.OrdinalIgnoreCase));
        Debug.Assert(text.Contains("10.0", StringComparison.Ordinal));
        Console.WriteLine($"  global.json: {text.ReplaceLineEndings(" ").Trim()}");
    }

    private static void DemoRuntimeAssemblyEvidence()
    {
        Console.WriteLine("-- 本进程程序集证据（SDK 产物） --");
        Assembly asm = typeof(SdkStyleProjectStructure).Assembly;
        Console.WriteLine($"  AssemblyName={asm.GetName().Name}");
        Console.WriteLine($"  Location={(string.IsNullOrEmpty(asm.Location) ? "(empty/single-file)" : Path.GetFileName(asm.Location))}");
        Console.WriteLine($"  Framework={System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
        Debug.Assert(asm.GetName().Name is "LearnCSharp");
    }

    private static string? FindRepoRoot()
    {
        foreach (string start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            DirectoryInfo? dir = new(start);
            while (dir is not null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "global.json"))
                    && File.Exists(Path.Combine(dir.FullName, "Directory.Build.props")))
                    return dir.FullName;
                dir = dir.Parent;
            }
        }

        return null;
    }
}
