// LearnCSharp example (filled)
// Doc      : CSharp-阶段1-语法基础与程序结构-详解.md
// Stage    : Stage01_SyntaxAndProgramStructure
// Section  : Section01_LanguageBasics
// Item     : CommentsAndXmlDocs
// Topic id : stage01/section01/comments_and_xml_docs
//
// 步骤 10：// /* */ /// /** */ 与常用 XML 文档标签；GenerateDocumentationFile / CS1591。

using System.Diagnostics;
using System.Reflection;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage01.Section01;

internal static class CommentsAndXmlDocs
{
    [LearnTopic("stage01/section01/comments_and_xml_docs")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CommentsAndXmlDocs ===");
        DemoCommentKinds();
        DemoXmlDocUsage();
        DemoRecommendedTags();
        DemoGenerateDocFile();
        return 0;
    }

    private static void DemoCommentKinds()
    {
        Console.WriteLine("-- 四种注释 --");
        // 单行注释（到行尾）

        /* 块注释,
           可跨多行 */

        // XML 文档形态（须贴在类型/成员上，见 XmlDemoMath）：
        //   /// <summary>...</summary>
        //   /** 块形式 XML 文档注释 */

        Console.WriteLine("  // 单行  |  /* */ 块  |  /// XML 单行  |  /** */ XML 块");
        Console.WriteLine("  🔶 // 与 /* */ 同 C++；/// 是语言一等公民文档，进 IntelliSense");
        Debug.Assert("//".Length == 2 && "/*".Length == 2);
    }

    private static void DemoXmlDocUsage()
    {
        Console.WriteLine("-- XML 文档实际调用 --");
        int sum = XmlDemoMath.Add(2, 3);
        int diff = XmlDemoMath.Subtract(10, 4);
        Console.WriteLine($"  Add(2,3)={sum}, Subtract(10,4)={diff}");
        Debug.Assert(sum == 5);
        Debug.Assert(diff == 6);

        // 反射读取方法上的文档需要 GenerateDocumentationFile；此处验证方法存在
        MethodInfo? add = typeof(XmlDemoMath).GetMethod(nameof(XmlDemoMath.Add));
        Debug.Assert(add is not null);
        Console.WriteLine($"  方法 {add!.Name} 有 XML 注释（IDE 悬停可见 summary）");
    }

    private static void DemoRecommendedTags()
    {
        Console.WriteLine("-- 常用标签 --");
        string[] tags =
        [
            "<summary> 一句话职责（最重要）",
            "<param name=\"\"> 参数",
            "<returns> 返回值",
            "<value> 属性值",
            "<remarks> 补充说明",
            "<example>/<code> 示例",
            "<exception cref=\"\"> 可能抛出的异常",
            "<see cref=\"\"> / <seealso> 交叉引用",
            "<paramref name=\"\"> 引用参数名",
            "<typeparam> 泛型参数",
            "<see langword=\"true\"/> 引用关键字",
        ];

        foreach (string t in tags)
            Console.WriteLine($"  · {t}");

        Debug.Assert(tags.Length >= 8);
        Debug.Assert(tags[0].Contains("summary", StringComparison.Ordinal));
    }

    private static void DemoGenerateDocFile()
    {
        Console.WriteLine("-- 生成文档文件 --");
        Console.WriteLine("  csproj: <GenerateDocumentationFile>true</GenerateDocumentationFile>");
        Console.WriteLine("  编译额外产出 .xml；公共成员缺文档 → 警告 CS1591");
        Console.WriteLine("  风格: 注释解释“为什么”，别复述代码“做了什么”");

        // 用 StringBuilder 拼一段示意 XML 文档片段（非真实文件）
        StringBuilder sample = new();
        sample.AppendLine("/// <summary>");
        sample.AppendLine("/// 把两个整数相加并返回结果。");
        sample.AppendLine("/// </summary>");
        sample.AppendLine("/// <param name=\"left\">左操作数。</param>");
        sample.AppendLine("/// <param name=\"right\">右操作数。</param>");
        sample.AppendLine("/// <returns>两数之和。</returns>");
        Console.WriteLine("  示例骨架:");
        foreach (string line in sample.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries))
            Console.WriteLine($"    {line.TrimEnd()}");

        Debug.Assert(sample.ToString().Contains("<summary>", StringComparison.Ordinal));
        Debug.Assert(XmlDemoMath.Add(1, 2) == 3);
    }
}

/// <summary>
/// 教学用：展示 XML 文档注释标签的迷你工具类。
/// </summary>
/// <remarks>
/// 文档注释必须紧贴在所描述的类型/成员之前。
/// 参见 <see cref="Add"/> 与 <see cref="Subtract"/>。
/// </remarks>
file static class XmlDemoMath
{
    /// <summary>
    /// 把两个整数相加并返回结果。
    /// </summary>
    /// <param name="left">左操作数。</param>
    /// <param name="right">右操作数。</param>
    /// <returns>两数之和。</returns>
    /// <exception cref="OverflowException">结果溢出 int 时抛出（checked）。</exception>
    /// <remarks>更长的说明放 remarks。参见 <see cref="Subtract"/>。</remarks>
    /// <example>
    /// <code>
    /// int s = XmlDemoMath.Add(1, 2); // 3
    /// </code>
    /// </example>
    public static int Add(int left, int right) => checked(left + right);

    /// <summary>
    /// 计算 <paramref name="left"/> 减 <paramref name="right"/>。
    /// </summary>
    /// <param name="left">被减数。</param>
    /// <param name="right">减数。</param>
    /// <returns>差。</returns>
    public static int Subtract(int left, int right) => left - right;
}
