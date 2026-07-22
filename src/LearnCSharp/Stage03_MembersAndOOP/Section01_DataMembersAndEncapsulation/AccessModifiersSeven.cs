// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第1部分-数据成员与封装.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section01_DataMembersAndEncapsulation
// Item     : AccessModifiersSeven
// Topic id : stage03/section01/access_modifiers_seven
//
// 步骤 3：七种访问修饰符与同程序集可达性矩阵。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section01;

internal static class AccessModifiersSeven
{
    [LearnTopic("stage03/section01/access_modifiers_seven")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AccessModifiersSeven ===");
        DemoDefaultsAndPublicPrivate();
        DemoProtectedFamily();
        DemoInternalCombinations();
        DemoFileScopedType();
        DemoStructNoProtected();
        return 0;
    }

    private static void DemoDefaultsAndPublicPrivate()
    {
        Console.WriteLine("-- 默认：成员 private / 顶级类型 internal --");
        VisibilityBox box = new VisibilityBox();
        Debug.Assert(box.PublicValue == 1);
        // box._privateValue; // ❌
        Console.WriteLine($"  PublicValue={box.PublicValue}");
    }

    private static void DemoProtectedFamily()
    {
        Console.WriteLine("-- protected：本类型 + 派生类 --");
        DerivedVisibility child = new DerivedVisibility();
        Debug.Assert(child.ReadProtected() == 42);
        Console.WriteLine($"  Derived reads protected={child.ReadProtected()}");
    }

    private static void DemoInternalCombinations()
    {
        Console.WriteLine("-- internal / protected internal / private protected --");
        SameAssemblyPeer same = new SameAssemblyPeer();
        Debug.Assert(same.ReadInternal() == 7);
        Debug.Assert(same.ReadProtectedInternal() == 8);
        // private protected：仅同程序集派生类
        DerivedVisibility derived = new DerivedVisibility();
        Debug.Assert(derived.ReadPrivateProtected() == 9);
        Console.WriteLine("  same-assembly peer: internal+protected internal OK");
        Console.WriteLine("  same-assembly derived: private protected OK");
    }

    private static void DemoFileScopedType()
    {
        Console.WriteLine("-- file 修饰符：仅本源文件可见 --");
        FileOnlyHelper helper = new FileOnlyHelper(99);
        Debug.Assert(helper.Value == 99);
        Console.WriteLine($"  FileOnlyHelper.Value={helper.Value}");
    }

    private static void DemoStructNoProtected()
    {
        Console.WriteLine("-- struct 成员：仅 public/internal/private --");
        PointLike s = new PointLike(3, 4);
        Debug.Assert(s.X == 3 && s.Y == 4);
        Console.WriteLine($"  PointLike=({s.X},{s.Y})");
        // struct 不能有 protected 成员（不能被继承）
    }

    private sealed class VisibilityBox
    {
        private readonly int _privateValue = 0;
        public int PublicValue { get; } = 1;
        public int PeekPrivate() => _privateValue;
    }

    private class BaseVisibility
    {
        protected readonly int ProtectedValue = 42;
        internal readonly int InternalValue = 7;
        protected internal readonly int ProtectedInternalValue = 8;
        private protected readonly int PrivateProtectedValue = 9;
    }

    private sealed class DerivedVisibility : BaseVisibility
    {
        public int ReadProtected() => ProtectedValue;
        public int ReadPrivateProtected() => PrivateProtectedValue;
    }

    private sealed class SameAssemblyPeer
    {
        public int ReadInternal()
        {
            BaseVisibility b = new BaseVisibility();
            return b.InternalValue;
        }

        public int ReadProtectedInternal()
        {
            BaseVisibility b = new BaseVisibility();
            return b.ProtectedInternalValue;
        }
        // b.PrivateProtectedValue; // ❌ 非派生
        // b.ProtectedValue; // ❌ 非派生
    }

    private readonly struct PointLike
    {
        public int X { get; }
        public int Y { get; }
        public PointLike(int x, int y) => (X, Y) = (x, y);
    }
}

// file 类型：仅本文件可见（C# 11）
file sealed class FileOnlyHelper
{
    public int Value { get; }
    public FileOnlyHelper(int value) => Value = value;
}
