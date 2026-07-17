// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第5部分-发布裁剪与AOT.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section05_PublishTrimmingAndAOT
// Item     : ReadyToRunR2R
// Topic id : stage10/section05/ready_to_run_r2r
//
// ReadyToRun：预生成部分原生代码，加快启动，仍保留 JIT/IL。

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section05;

internal static class ReadyToRunR2R
{
    [LearnTopic("stage10/section05/ready_to_run_r2r")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReadyToRunR2R ===");
        DemoWhatIsR2R();
        DemoVsJitAndAot();
        DemoEnable();
        DemoTradeoffs();
        DemoStartupMentalModel();
        return 0;
    }

    private static void DemoWhatIsR2R()
    {
        Console.WriteLine("-- what is ReadyToRun --");
        Console.WriteLine("  发布时把部分方法编译成平台原生代码嵌进程序集");
        Console.WriteLine("  启动时少做 JIT → 冷启动更好");
        Console.WriteLine("  仍是 CoreCLR：可再 JIT、可反射、有 GC");
        Debug.Assert(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.Contains(".NET"));
    }

    private static void DemoVsJitAndAot()
    {
        Console.WriteLine("-- R2R vs JIT-only vs Native AOT --");
        (string Mode, string CompileWhen, string Runtime)[] rows =
        [
            ("JIT-only", "首次执行方法时", "CoreCLR + 完整 IL"),
            ("R2R", "publish 时预生成 + 运行时可 JIT", "CoreCLR"),
            ("Native AOT", "publish 时全量原生", "无 JIT 的原生宿主"),
        ];
        foreach ((string? mode, string? when, string? rt) in rows)
            Console.WriteLine($"  {mode,-12} | {when,-28} | {rt}");
        Debug.Assert(rows.Length == 3);
    }

    private static void DemoEnable()
    {
        Console.WriteLine("-- enable --");
        Console.WriteLine("  <PublishReadyToRun>true</PublishReadyToRun>");
        Console.WriteLine("  常与 -r <RID> 及 self-contained 组合");
        Console.WriteLine("  $ dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true");
        string prop = "PublishReadyToRun";
        Debug.Assert(prop.Contains("ReadyToRun", StringComparison.Ordinal));
    }

    private static void DemoTradeoffs()
    {
        Console.WriteLine("-- tradeoffs --");
        string[] pros = ["更好冷启动", "仍兼容大部分反射/动态场景", "比 AOT 迁移成本低"];
        string[] cons = ["输出更大", "平台相关（按 RID）", "峰值吞吐仍可能靠 JIT 优化"];
        Console.WriteLine("  pros:");
        foreach (string p in pros)
            Console.WriteLine($"    + {p}");
        Console.WriteLine("  cons:");
        foreach (string c in cons)
            Console.WriteLine($"    - {c}");
        Debug.Assert(pros.Length == cons.Length);
    }

    private static void DemoStartupMentalModel()
    {
        Console.WriteLine("-- startup mental model --");
        // 用一次方法调用表示“已有可执行体”
        static int HotPath(int x) => x * x;
        int y = HotPath(7);
        Debug.Assert(y == 49);
        MethodInfo mi = typeof(ReadyToRunR2R).GetMethod(nameof(Run), BindingFlags.NonPublic | BindingFlags.Static)!;
        Console.WriteLine($"  method exists for JIT/R2R: {mi.Name}");
        Console.WriteLine("  R2R: 部分方法启动即有原生入口；未覆盖方法仍 JIT");
        Console.WriteLine("  测量: 用真实 publish 产物做冷启动基准，不要用 dotnet run");
    }
}
