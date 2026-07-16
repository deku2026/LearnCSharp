// LearnCSharp example (filled)
// Doc      : CSharp-阶段1-语法基础与程序结构-详解.md
// Stage    : Stage01_SyntaxAndProgramStructure
// Section  : Section01_LanguageBasics
// Item     : EntryPointMainVsTopLevel
// Topic id : stage01/section01/entry_point_main_vs_top_level
//
// 步骤 3：顶级语句 vs 显式 Main；args 语义；退出码；合法 Main 签名。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage01.Section01;

internal static class EntryPointMainVsTopLevel
{
    [LearnTopic("stage01/section01/entry_point_main_vs_top_level")]
    internal static int Run(string[] args)
    {
        // 本主题演示 args 语义：用真实传入的 args，但不阻塞读入
        Console.WriteLine("=== EntryPointMainVsTopLevel ===");
        DemoArgsSemantics(args);
        DemoExitCodes(args);
        DemoMainSignaturesCatalog();
        DemoTopLevelRules();
        DemoProgramNameVsArgs0();
        return 0;
    }

    private static void DemoArgsSemantics(string[] args)
    {
        Console.WriteLine("-- args 语义（⚠ ≠ C++ argv）--");
        // C#: args[0] = 第一个用户参数；C++: argv[0] = 程序路径
        // 运行时 args 永不 null；空参时 Length==0
        ArgumentNullException.ThrowIfNull(args);
        Console.WriteLine($"  args.Length = {args.Length}");

        if (args.Length == 0)
        {
            Console.WriteLine("  无参数: 用法示例 → program <名字>");
            Console.WriteLine("  传参: dotnet run -- 张三   （-- 后才是程序参数）");
        }
        else
        {
            Console.WriteLine($"  第一个参数 args[0] = '{args[0]}'");
            for (int i = 0; i < args.Length; i++)
                Console.WriteLine($"    [{i}] = {args[i]}");
        }
    }

    private static void DemoExitCodes(string[] args)
    {
        Console.WriteLine("-- 退出码 --");
        // 顶级语句 / Main 可 return int；0 成功，非 0 失败（脚本/CI 可读）
        int simulatedExit = args.Length == 0 ? 1 : 0;
        Console.WriteLine($"  模拟: 无参 return 1，有参 return 0 → 当前模拟码={simulatedExit}");
        Debug.Assert(simulatedExit is 0 or 1);

        // 本 Run 仍固定 return 0，避免打断 LearnCSharp 宿主
        Console.WriteLine("  本宿主要求 Run 返回 0；真实 CLI 程序才用非 0 表示失败");
    }

    private static void DemoMainSignaturesCatalog()
    {
        Console.WriteLine("-- 合法 Main 签名（8 种）--");
        string[] signatures =
        [
            "static void Main()",
            "static void Main(string[] args)",
            "static int Main()",
            "static int Main(string[] args)",
            "static async Task Main()",
            "static async Task Main(string[] args)",
            "static async Task<int> Main()",
            "static async Task<int> Main(string[] args)",
        ];

        foreach (string s in signatures)
            Console.WriteLine($"  {s}");

        Console.WriteLine("  规则: 必须 static；类名任意(约定 Program)；访问修饰符对入口无影响");
        Debug.Assert(signatures.Length == 8);
        Debug.Assert(signatures.All(s => s.Contains("Main", StringComparison.Ordinal)));
    }

    private static void DemoTopLevelRules()
    {
        Console.WriteLine("-- 顶级语句硬规则 --");
        string[] rules =
        [
            "整个程序只能有一个文件使用顶级语句",
            "按出现顺序执行",
            "必须位于任何 namespace/类型声明之前",
            "可访问 args / await / return 退出码 / 定义本地函数",
            "与显式 Main 编译产物等价（SharpLab 可验证）",
        ];

        foreach (string r in rules)
            Console.WriteLine($"  · {r}");

        // 模拟“有参问候”逻辑（顶级语句版与显式 Main 版同语义）
        int code = SimulateGreeting(["Ada"]);
        Debug.Assert(code == 0);
        code = SimulateGreeting([]);
        Debug.Assert(code == 1);
        Console.WriteLine($"  SimulateGreeting([])={SimulateGreeting([])}, ([\"Ada\"])={SimulateGreeting(["Ada"])}");
    }

    private static void DemoProgramNameVsArgs0()
    {
        Console.WriteLine("-- 程序名 vs args[0] --");
        string? processPath = Environment.ProcessPath;
        string[] cmd = Environment.GetCommandLineArgs();
        Console.WriteLine($"  Environment.ProcessPath = {processPath}");
        Console.WriteLine($"  GetCommandLineArgs()[0]  = {cmd[0]}  ← 含程序路径");
        Console.WriteLine("  Main 的 string[] args 已剥离程序名，从用户参数开始");
        Debug.Assert(cmd.Length >= 1);
        Debug.Assert(!string.IsNullOrEmpty(cmd[0]));
    }

    /// <summary>模拟顶级语句/Main 的问候退出码逻辑。</summary>
    private static int SimulateGreeting(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("  [sim] 用法: program <名字>");
            return 1;
        }

        Console.WriteLine($"  [sim] 你好,{args[0]}");
        return 0;
    }

    // 演示“显式 Main 挂在任意类上”——此处仅为教学嵌套类型，不是真实入口
    private static class ExplicitMainShape
    {
        // 合法形状示意（不会被运行时当作入口，宿主已有唯一入口）
        internal static int MainLike(string[] args) => SimulateGreeting(args);
    }
}
