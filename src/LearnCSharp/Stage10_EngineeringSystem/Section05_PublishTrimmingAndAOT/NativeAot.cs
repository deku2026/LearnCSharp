// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第5部分-发布裁剪与AOT.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section05_PublishTrimmingAndAOT
// Item     : NativeAot
// Topic id : stage10/section05/native_aot
//
// Native AOT：无 JIT 原生可执行文件；限制清单与适用场景。

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section05;

internal static class NativeAot
{
    [LearnTopic("stage10/section05/native_aot")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== NativeAot ===");
        DemoWhatIsNativeAot();
        DemoEnableAndOutput();
        DemoConstraintChecklist();
        DemoProsCons();
        DemoDetectRuntimeFlavor();
        return 0;
    }

    private static void DemoWhatIsNativeAot()
    {
        Console.WriteLine("-- what is Native AOT --");
        Console.WriteLine("  publish 时编译为单一（或少量）原生可执行文件");
        Console.WriteLine("  无 JIT、无 IL 运行时解释；GC 仍在（原生运行时）");
        Console.WriteLine("  启动快、部署简单、体积通常小于完整 SCD");
        Debug.Assert(true);
    }

    private static void DemoEnableAndOutput()
    {
        Console.WriteLine("-- enable --");
        Console.WriteLine("  <PublishAot>true</PublishAot>");
        Console.WriteLine("  需要 RID；常用自包含原生输出");
        Console.WriteLine("  $ dotnet publish -c Release -r win-x64 -p:PublishAot=true");
        Console.WriteLine("  产物: 原生 MyApp.exe / MyApp（非托管依赖需注意）");
        string prop = "PublishAot";
        Debug.Assert(prop.Contains("Aot", StringComparison.Ordinal));
    }

    private static void DemoConstraintChecklist()
    {
        Console.WriteLine("-- constraint checklist (maps to Unity IL2CPP pain) --");
        string[] limits =
        [
            "动态程序集加载 / emit 基本不可用",
            "无约束反射易被裁掉 → 需注解/源生成",
            "部分运行时功能不可用或受限",
            "调试/诊断与 CoreCLR 有差异",
            "编译更慢、平台交叉编译有限制",
            "不适合强插件化、脚本宿主场景",
        ];
        foreach (string l in limits)
            Console.WriteLine($"  ☐ {l}");
        Debug.Assert(limits.Length >= 5);
    }

    private static void DemoProsCons()
    {
        Console.WriteLine("-- pros / cons --");
        Console.WriteLine("  + 冷启动、磁盘体积、无运行时安装");
        Console.WriteLine("  + 攻击面更小（无 JIT 编译服务）");
        Console.WriteLine("  - 功能子集；库生态需 trim/AOT 友好");
        Console.WriteLine("  - 迭代：每次改代码需重新 publish 测原生产物");
        (string Side, int Score)[] demo = [("startup", 9), ("flexibility", 4)];
        Debug.Assert(demo[0].Score > demo[1].Score);
    }

    private static void DemoDetectRuntimeFlavor()
    {
        Console.WriteLine("-- runtime flavor hints (this process) --");
        Console.WriteLine($"  FrameworkDescription={RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"  RuntimeIdentifier={RuntimeInformation.RuntimeIdentifier}");
        // NativeAOT 程序通常 Assembly.Location 为空且描述含 NativeAOT（随版本变化）
        string fw = RuntimeInformation.FrameworkDescription;
        bool looksAot = fw.Contains("Native", StringComparison.OrdinalIgnoreCase)
                        || fw.Contains("AOT", StringComparison.OrdinalIgnoreCase);
        Console.WriteLine($"  looks like Native AOT description? {looksAot}");
        Console.WriteLine("  学习仓一般是 CoreCLR 开发构建；AOT 需单独 publish 验证");
        Debug.Assert(fw.Length > 0);
    }
}
