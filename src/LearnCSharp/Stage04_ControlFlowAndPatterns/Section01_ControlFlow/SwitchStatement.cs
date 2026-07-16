// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第1部分-控制流.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section01_ControlFlow
// Item     : SwitchStatement
// Topic id : stage04/section01/switch_statement
//
// 步骤 4：switch 语句、无穿透、空标签堆叠、goto case、字符串/关系模式预热。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section01;

internal static class SwitchStatement
{
    [LearnTopic("stage04/section01/switch_statement")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SwitchStatement ===");
        DemoBasicAndStacking();
        DemoNoFallThrough();
        DemoGotoCase();
        DemoStringSwitch();
        DemoRelationalCase();
        return 0;
    }

    private static void DemoBasicAndStacking()
    {
        Console.WriteLine("-- 基本 switch + 空标签堆叠 --");
        static string DayKind(int day) => day switch
        {
            // 语句形态演示堆叠：
            _ => DescribeDay(day),
        };

        static string DescribeDay(int day)
        {
            switch (day)
            {
                case 1:
                    return "周一";
                case 6:
                case 7:
                    return "周末";
                default:
                    return "工作日";
            }
        }

        Debug.Assert(DescribeDay(1) == "周一");
        Debug.Assert(DescribeDay(6) == "周末");
        Debug.Assert(DescribeDay(7) == "周末");
        Debug.Assert(DescribeDay(3) == "工作日");
        Console.WriteLine($"  1→{DescribeDay(1)}, 6→{DescribeDay(6)}, 3→{DescribeDay(3)}");
        _ = DayKind(1);
    }

    private static void DemoNoFallThrough()
    {
        Console.WriteLine("-- 无穿透：每段须 break/return/goto/throw --");
        // case 1: DoA(); case 2: ...  // ❌ CS0163 控制不能穿透
        int x = 1;
        string result;
        switch (x)
        {
            case 1:
                result = "one";
                break; // 必须显式终止
            case 2:
                result = "two";
                break;
            default:
                result = "other";
                break;
        }
        Debug.Assert(result == "one");
        Console.WriteLine("  漏 break → CS0163；C++ 默认穿透是经典 bug 源");
    }

    private static void DemoGotoCase()
    {
        Console.WriteLine("-- goto case：显式穿透 --");
        static string RunState(string state)
        {
            var log = new List<string>();
            switch (state)
            {
                case "start":
                    log.Add("init");
                    goto case "run";
                case "run":
                    log.Add("execute");
                    break;
                default:
                    log.Add("unknown");
                    break;
            }
            return string.Join("+", log);
        }

        Debug.Assert(RunState("start") == "init+execute");
        Debug.Assert(RunState("run") == "execute");
        Console.WriteLine($"  start → {RunState("start")}（goto case 显式意图）");
    }

    private static void DemoStringSwitch()
    {
        Console.WriteLine("-- 字符串 switch（C++ 做不到） --");
        static int Code(string cmd)
        {
            switch (cmd)
            {
                case "help":
                    return 0;
                case "quit":
                case "exit":
                    return 1;
                default:
                    return -1;
            }
        }

        Debug.Assert(Code("help") == 0);
        Debug.Assert(Code("exit") == 1);
        Debug.Assert(Code("nope") == -1);
        Console.WriteLine($"  help→0, exit→1, nope→-1");
    }

    private static void DemoRelationalCase()
    {
        Console.WriteLine("-- case 可用关系模式（预热模式匹配） --");
        static string Band(double measurement)
        {
            switch (measurement)
            {
                case < 0.0:
                    return "过低";
                case > 100.0:
                    return "过高";
                case double.NaN:
                    return "无效";
                default:
                    return "正常";
            }
        }

        Debug.Assert(Band(-1) == "过低");
        Debug.Assert(Band(50) == "正常");
        Debug.Assert(Band(101) == "过高");
        Debug.Assert(Band(double.NaN) == "无效");
        Console.WriteLine($"  -1→{Band(-1)}, 50→{Band(50)}, NaN→{Band(double.NaN)}");
    }
}
