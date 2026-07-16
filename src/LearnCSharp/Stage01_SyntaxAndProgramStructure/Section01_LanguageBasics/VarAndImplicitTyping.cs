// LearnCSharp example (filled)
// Doc      : CSharp-阶段1-语法基础与程序结构-详解.md
// Stage    : Stage01_SyntaxAndProgramStructure
// Section  : Section01_LanguageBasics
// Item     : VarAndImplicitTyping
// Topic id : stage01/section01/var_and_implicit_typing
//
// 步骤 7：显式声明 vs var；硬规则；≠ dynamic；IL 与显式相同。
// 本文件刻意大量使用 var（主题本身），关闭 IDE0008。

#pragma warning disable IDE0008 // Use explicit type instead of var

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage01.Section01;

internal static class VarAndImplicitTyping
{
    [LearnTopic("stage01/section01/var_and_implicit_typing")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== VarAndImplicitTyping ===");
        DemoExplicitDeclaration();
        DemoVarInference();
        DemoVarHardRules();
        DemoWhenToUseVar();
        DemoNotDynamic();
        return 0;
    }

    private static void DemoExplicitDeclaration()
    {
        Console.WriteLine("-- 显式声明 --");
        int count = 10;
        string name;
        name = "Ada";
        Console.WriteLine($"  int count={count}, string name={name}");
        Debug.Assert(count == 10);
        Debug.Assert(name == "Ada");
    }

    private static void DemoVarInference()
    {
        Console.WriteLine("-- var 推断 --");
        var i = 10;                    // int
        var s = "hello";               // string
        var list = new List<int>();    // List<int>
        var anon = new { X = 1, Y = 2 }; // 匿名类型（必须 var）

        Console.WriteLine($"  var i=10 → {i.GetType().Name}");
        Console.WriteLine($"  var s=\"hello\" → {s.GetType().Name}");
        Console.WriteLine($"  var list=new List<int>() → {list.GetType().Name}");
        Console.WriteLine($"  匿名类型 → {anon.GetType().Name}, X={anon.X}");

        Debug.Assert(i.GetType() == typeof(int));
        Debug.Assert(s.GetType() == typeof(string));
        Debug.Assert(list.GetType() == typeof(List<int>));
        Debug.Assert(anon.X == 1 && anon.Y == 2);

        // 与显式写法静态类型完全相同
        int i2 = 10;
        Debug.Assert(i.GetType() == i2.GetType());
    }

    private static void DemoVarHardRules()
    {
        Console.WriteLine("-- var 硬规则 --");
        Console.WriteLine("  ✓ 必须同语句初始化: var x = 1;");
        Console.WriteLine("  ✗ 不能 var x;          → CS0818");
        Console.WriteLine("  ✗ 不能 var y = null;   → 无法推断");
        Console.WriteLine("  ✗ 不能 var f = Console.WriteLine; → 方法组");
        Console.WriteLine("  ✗ 一条语句多个 var");
        Console.WriteLine("  ✗ 字段/属性/参数/返回类型不能用 var");
        Console.WriteLine("  ✗ 不能在自身初始化里引用自己");

        // 合法对照
        var ok = 1;
        // var bad;              // CS0818
        // var n = null;         // 需目标类型，如 string? n = null;
        // private var field;    // 字段禁止

        string? explicitNull = null;
        Debug.Assert(ok == 1);
        Debug.Assert(explicitNull is null);
        Console.WriteLine($"  对照: string? explicitNull=null 合法; var ok={ok}");
    }

    private static void DemoWhenToUseVar()
    {
        Console.WriteLine("-- 何时用 / 不用 --");
        // 右边显而易见 → 用 var 减重复
        var map = new Dictionary<string, List<int>>();
        map["a"] = [1, 2];
        Console.WriteLine($"  冗长泛型用 var: Dictionary keys={map.Count}");

        // 匿名类型 / 查询投影 → 必须 var
        var projection = from n in new[] { 1, 2, 3 }
                         select new { N = n, Sq = n * n };
        Debug.Assert(projection.Count() == 3);
        Console.WriteLine($"  LINQ 匿名投影 count={projection.Count()}");

        // 右边看不出类型时，显式更清晰
        int port = int.Parse("8080");
        Debug.Assert(port == 8080);
        Console.WriteLine($"  显式 int port = int.Parse(...) → {port}");
    }

    private static void DemoNotDynamic()
    {
        Console.WriteLine("-- var ≠ dynamic（编译期类型）--");
        var i = 10;
        // i = "x"; // 编译错误：i 已是 int
        Console.WriteLine($"  var i=10 后类型锁定为 {i.GetType().Name}");
        Console.WriteLine("  🔶 C++ auto 可用处更广；C# var 仅局部变量");
        Console.WriteLine("  SharpLab: var i=10 与 int i=10 的 IL 完全一致");

        Debug.Assert(i.GetType() == typeof(int));
        // dynamic 才是运行期绑定（阶段后期）
        dynamic d = 10;
        d = "now string";
        Debug.Assert(((object)d).GetType() == typeof(string));
        Console.WriteLine($"  dynamic 可改绑: 最终 type={((object)d).GetType().Name}");
    }
}
