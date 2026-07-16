// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第2部分-CLR对象模型与方法表.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section02_CLRObjectModelAndMethodTable
// Item     : MethodTableAndEEClass
// Topic id : stage11/section02/method_table_and_eeclass
//
// Lesson: MethodTable is the runtime type descriptor; EEClass holds less-hot data.

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section02;

internal static class MethodTableAndEEClass
{
    [LearnTopic("stage11/section02/method_table_and_eeclass")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MethodTableAndEEClass ===");
        DemoTypeAsMethodTableProxy();
        DemoSharedGenericMethodTables();
        DemoEeClassHotColdSplit();
        return 0;
    }

    private static void DemoTypeAsMethodTableProxy()
    {
        Console.WriteLine("-- System.Type reflects MethodTable identity --");
        var dog = new Dog("Rex");
        Type t1 = dog.GetType();
        Type t2 = typeof(Dog);
        Debug.Assert(ReferenceEquals(t1, t2));
        Console.WriteLine($"  dog.GetType() == typeof(Dog): {ReferenceEquals(t1, t2)}");
        Console.WriteLine($"  TypeHandle: 0x{t1.TypeHandle.Value:X}");
        Console.WriteLine("  Every object header points at its MethodTable (type identity for cast/dispatch).");
        Debug.Assert(t1.TypeHandle.Value != IntPtr.Zero);
    }

    private static void DemoSharedGenericMethodTables()
    {
        Console.WriteLine("-- generics: reference types often share code / related MTs --");
        Type listStr = typeof(List<string>);
        Type listObj = typeof(List<object>);
        Type listInt = typeof(List<int>);
        Console.WriteLine($"  List<string> IsGenericType={listStr.IsGenericType}, def={listStr.GetGenericTypeDefinition().Name}");
        Console.WriteLine($"  List<int> is value-type T → distinct native layout / code paths");
        Debug.Assert(listStr.GetGenericTypeDefinition() == listObj.GetGenericTypeDefinition());
        Debug.Assert(listStr.GetGenericTypeDefinition() == listInt.GetGenericTypeDefinition());
        Debug.Assert(listStr != listInt);
        // Method tables differ per closed type
        Console.WriteLine($"  Closed types distinct: stringMT≠intMT → {listStr != listInt}");
    }

    private static void DemoEeClassHotColdSplit()
    {
        Console.WriteLine("-- MethodTable (hot) vs EEClass (cold) --");
        Console.WriteLine("  MethodTable: size, GC desc, vtable slots, interface map — used every cast/call.");
        Console.WriteLine("  EEClass: field layout details, less frequently touched metadata.");
        Type t = typeof(Dog);
        FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        MethodInfo[] methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        Console.WriteLine($"  Dog fields={fields.Length}, declared public methods={methods.Length}");
        Debug.Assert(methods.Any(m => m.Name == nameof(Dog.Speak)));
        Console.WriteLine("  Reflection walks metadata; execution uses MethodTable slots.");
    }

    private sealed class Dog(string name)
    {
        private readonly string _name = name;
        public string Speak() => $"woof:{_name}";
    }
}
