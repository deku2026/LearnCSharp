// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : AccessModifiersBasic (三、访问修饰符基础 — 4 个)
// Topic id : stage08/section01/access_modifiers_basic
//
// public / private / protected / internal（+ 组合 protected internal / private protected）。

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class AccessModifiersBasic
{
    [LearnTopic("stage08/section01/access_modifiers_basic")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AccessModifiersBasic ===");
        DemoFourBasics();
        DemoCombinationsViaReflection();
        DemoDefaults();
        return 0;
    }

    private static void DemoFourBasics()
    {
        Console.WriteLine("-- public / private / protected / internal --");
        BaseBox baseObj = new BaseBox(10);
        Debug.Assert(baseObj.PublicValue == 10);
        Debug.Assert(baseObj.InternalValue == 10);
        // private / protected 从外部不可见
        DerivedBox derived = new DerivedBox(20);
        Debug.Assert(derived.ReadProtected() == 20);
        Console.WriteLine($"  Public={baseObj.PublicValue}, Internal={baseObj.InternalValue}, Protected via derived={derived.ReadProtected()}");
    }

    private static void DemoCombinationsViaReflection()
    {
        Console.WriteLine("-- protected internal / private protected 元数据 --");
        Type t = typeof(BaseBox);
        MethodInfo? pi = t.GetMethod(nameof(BaseBox.ProtectedInternalMethod),
            BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? pp = t.GetMethod("PrivateProtectedMethod",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Debug.Assert(pi is not null && pi.IsFamilyOrAssembly);
        Debug.Assert(pp is not null && pp.IsFamilyAndAssembly);
        Console.WriteLine($"  ProtectedInternal IsFamilyOrAssembly={pi.IsFamilyOrAssembly}");
        Console.WriteLine($"  PrivateProtected IsFamilyAndAssembly={pp.IsFamilyAndAssembly}");
    }

    private static void DemoDefaults()
    {
        Console.WriteLine("-- 默认访问级别 --");
        // 顶级类型默认 internal；类成员默认 private
        Type top = typeof(TopDefault);
        Debug.Assert(top.IsNotPublic); // internal 顶级类型
        FieldInfo? f = typeof(BaseBox).GetField("_secret", BindingFlags.Instance | BindingFlags.NonPublic);
        Debug.Assert(f is not null && f.IsPrivate);
        Console.WriteLine($"  TopDefault IsNotPublic={top.IsNotPublic}, _secret IsPrivate={f.IsPrivate}");
    }

    private class BaseBox
    {
        private int _secret;
        public int PublicValue { get; }
        internal int InternalValue { get; }
        protected int ProtectedValue { get; }

        public BaseBox(int v)
        {
            _secret = v;
            PublicValue = v;
            InternalValue = v;
            ProtectedValue = v;
        }

        public int ReadSecret() => _secret;
        protected internal void ProtectedInternalMethod() { }
        private protected void PrivateProtectedMethod() { }
    }

    private sealed class DerivedBox : BaseBox
    {
        public DerivedBox(int v) : base(v) { }
        public int ReadProtected() => ProtectedValue;
    }
}

internal sealed class TopDefault
{
}
