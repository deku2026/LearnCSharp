// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第5部分-发布裁剪与AOT.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section05_PublishTrimmingAndAOT
// Item     : AotTrimSafeCoding
// Topic id : stage10/section05/aot_trim_safe_coding
//
// 注解、源生成、库 vs 应用职责：写出 trim/AOT 安全代码。

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section05;

internal static partial class AotTrimSafeCoding
{
    private sealed class Payload
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    [JsonSerializable(typeof(Payload))]
    private partial class PayloadJsonContext : JsonSerializerContext;

    [LearnTopic("stage10/section05/aot_trim_safe_coding")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AotTrimSafeCoding ===");
        DemoAnnotationAttributes();
        DemoJsonSourceGen();
        DemoPreferStaticOverStringReflection();
        DemoLibraryVsApp();
        DemoChecklist();
        return 0;
    }

    private static void DemoAnnotationAttributes()
    {
        Console.WriteLine("-- key attributes --");
        (string Attr, string Role)[] attrs =
        [
            ("RequiresUnreferencedCode", "声明成员在 trim 下不安全，调用链传播警告"),
            ("RequiresDynamicCode", "需要动态代码（AOT 敏感）"),
            ("DynamicallyAccessedMembers", "保证 Type 上哪些成员被保留"),
            ("DynamicDependency", "额外根住某成员/类型"),
            ("UnconditionalSuppressMessage", "确认安全后的定点抑制"),
        ];
        foreach (var (attr, role) in attrs)
            Console.WriteLine($"  {attr,-28} {role}");
        Debug.Assert(attrs.Length == 5);

        string kept = CallWithAnnotation(typeof(Payload));
        Debug.Assert(kept == nameof(Payload.Id));
        Console.WriteLine($"  DynamicallyAccessedMembers demo saw member: {kept}");
    }

    private static void DemoJsonSourceGen()
    {
        Console.WriteLine("-- JSON source generation (trim-safe) --");
        var dto = new Payload { Id = 7, Name = "ada" };
        string json = JsonSerializer.Serialize(dto, PayloadJsonContext.Default.Payload);
        Payload? back = JsonSerializer.Deserialize(json, PayloadJsonContext.Default.Payload);
        Debug.Assert(back is { Id: 7, Name: "ada" });
        Console.WriteLine($"  source-gen json: {json}");
        Console.WriteLine("  避免运行时反射契约：JsonSerializerContext + [JsonSerializable]");
    }

    private static void DemoPreferStaticOverStringReflection()
    {
        Console.WriteLine("-- prefer static references --");
        // 好: typeof(T) / 泛型
        Type t = typeof(Payload);
        Debug.Assert(t.GetProperty(nameof(Payload.Name)) is not null);
        // 坏: Type.GetType("Namespace.Payload") 仅字符串
        Console.WriteLine("  good: typeof(Payload), generic helpers, source gen");
        Console.WriteLine("  bad: 配置文件里的类型全名 + Activator.CreateInstance 无注解");
    }

    private static void DemoLibraryVsApp()
    {
        Console.WriteLine("-- library vs application duties --");
        Console.WriteLine("  库: 标注公共 API 的 trim/AOT 特性；提供源生成或工厂");
        Console.WriteLine("  库: IsAotCompatible / IsTrimmable 属性向消费者声明");
        Console.WriteLine("  应用: 开启 PublishTrimmed/Aot，修警告，测发布产物");
        Console.WriteLine("  应用: 避免引入未标注的反射重度包");
        string[] roles = ["annotate", "generate", "test published output"];
        Debug.Assert(roles.Length == 3);
    }

    private static void DemoChecklist()
    {
        Console.WriteLine("-- safe coding checklist --");
        string[] items =
        [
            "热点序列化改用源生成",
            "公共反射 API 加 DynamicallyAccessedMembers",
            "动态特性 API 标 RequiresUnreferencedCode",
            "CI 对 publish trim/aot 开 warnaserror",
            "集成测试跑真正的 publish 输出",
        ];
        foreach (string i in items)
            Console.WriteLine($"  ☐ {i}");
        Debug.Assert(items.Length == 5);
    }

    private static string CallWithAnnotation(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type)
    {
        // 注解告诉裁剪器：需要保留 public properties
        return type.GetProperties().FirstOrDefault()?.Name ?? "";
    }
}
