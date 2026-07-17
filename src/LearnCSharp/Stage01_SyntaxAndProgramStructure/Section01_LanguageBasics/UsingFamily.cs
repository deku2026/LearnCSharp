// LearnCSharp example (filled)
// Doc      : CSharp-阶段1-语法基础与程序结构-详解.md
// Stage    : Stage01_SyntaxAndProgramStructure
// Section  : Section01_LanguageBasics
// Item     : UsingFamily
// Topic id : stage01/section01/using_family
//
// 步骤 5：using 五种——普通 / global / implicit / static / 别名。

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;
using static System.Math;
using Gen = System.Collections.Generic;
using IntList = System.Collections.Generic.List<int>;
using PointAlias = (int X, int Y);

namespace LearnCSharp.Stage01.Section01;

internal static class UsingFamily
{
    [LearnTopic("stage01/section01/using_family")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== UsingFamily ===");
        DemoOrdinaryUsing();
        DemoUsingStatic();
        DemoUsingAlias();
        DemoImplicitAndGlobal();
        DemoNotInclude();
        return 0;
    }

    private static void DemoOrdinaryUsing()
    {
        Console.WriteLine("-- ① 普通 using --");
        // using System.Text; → 可用 StringBuilder 短名
        StringBuilder sb = new("hello");
        sb.Append(" world");
        Console.WriteLine($"  StringBuilder → {sb}");
        Debug.Assert(sb.ToString() == "hello world");

        // using 只导入该命名空间本身，不导入嵌套命名空间
        Console.WriteLine("  using System; 不会导入 System.Collections.Generic");
        Console.WriteLine("  作用域: 所在文件（非 global 时）");
    }

    private static void DemoUsingStatic()
    {
        Console.WriteLine("-- ④ using static --");
        // using static System.Math; → 直接 Sqrt / PI，无需 Math.
        double root = Sqrt(16);
        double circle = PI * 2;
        Console.WriteLine($"  Sqrt(16) = {root}");
        Console.WriteLine($"  PI * 2   = {circle}");
        Debug.Assert(root == 4.0);
        Debug.Assert(Abs(circle - (Math.PI * 2)) < 1e-12);

        Console.WriteLine("  适合数学/常量密集；滥用会模糊成员来源");
    }

    private static void DemoUsingAlias()
    {
        Console.WriteLine("-- ⑤ using 别名 --");
        // 命名空间别名 / 类型别名 / C# 12 任意类型别名
        Gen.List<string> names = ["Ada", "Grace"];
        IntList nums = [1, 2, 3];
        PointAlias p = (3, 4);

        Console.WriteLine($"  Gen.List<string> count={names.Count}");
        Console.WriteLine($"  IntList = List<int>, sum={nums.Sum()}");
        Console.WriteLine($"  PointAlias (int X,int Y) = ({p.X},{p.Y})");

        Debug.Assert(names.Count == 2);
        Debug.Assert(nums.Sum() == 6);
        Debug.Assert(p is (3, 4));

        Console.WriteLine("  限制: 别名不能含 ref/in/out；不能 using static X = ...");
    }

    private static void DemoImplicitAndGlobal()
    {
        Console.WriteLine("-- ②③ global using / implicit usings --");
        Console.WriteLine("  global using: 对整个项目所有文件生效，写一次");
        Console.WriteLine("  规则: 所有 global using 在非 global 之前；不能写在 namespace 内");
        Console.WriteLine("  ImplicitUsings=enable → SDK 按项目类型生成 global using");
        Console.WriteLine("  控制台默认 7 个: System, Collections.Generic, IO, Linq,");
        Console.WriteLine("                   Net.Http, Threading, Threading.Tasks");
        Console.WriteLine("  查看: obj/Debug/net10.0/*.GlobalUsings.g.cs");
        Console.WriteLine("  增删: <Using Include=\"...\"/> / <Using Remove=\"...\"/>");

        // 本项目开了 ImplicitUsings：List/LINQ 无需显式 using 也能用
        List<int> xs = [10, 20, 30];
        int total = xs.Sum();
        Debug.Assert(total == 60);
        Console.WriteLine($"  List+Sum 可用 → implicit usings 生效, sum={total}");
    }

    private static void DemoNotInclude()
    {
        Console.WriteLine("-- 🔶 using ≠ #include --");
        Console.WriteLine("  #include = 文本替换，改变编译单元物理内容");
        Console.WriteLine("  using   = 仅名字解析便利，不引入代码");
        Console.WriteLine("  真正的“链接”是程序集引用 (PackageReference / ProjectReference)");
        Debug.Assert(typeof(UsingFamily).Assembly.GetName().Name is not null);
    }
}
