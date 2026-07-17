// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第2部分-CLR对象模型与方法表.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section02_CLRObjectModelAndMethodTable
// Item     : MethodTableAndEEClass
// Topic id : stage11/section02/method_table_and_eeclass
//
// Lesson: MethodTable is the runtime type descriptor; EEClass holds less-hot data.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section02;

internal static class MethodTableAndEEClass
{
    [LearnTopic("stage11/section02/method_table_and_eeclass")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MethodTableAndEEClass ===");
        DemoTypeHandleAndIdentity();
        DemoAllocationViaMethodTable();
        DemoSharedGenericMethodTables();
        DemoRuntimeHelpers();
        return 0;
    }

    private static void DemoTypeHandleAndIdentity()
    {
        Console.WriteLine("-- TypeHandle ≈ MethodTable pointer --");
        Dog dog = new Dog("Rex");
        Type t1 = dog.GetType();
        Type t2 = typeof(Dog);
        RuntimeTypeHandle h1 = t1.TypeHandle;
        RuntimeTypeHandle h2 = t2.TypeHandle;
        Debug.Assert(ReferenceEquals(t1, t2));
        Debug.Assert(h1.Value == h2.Value);
        Debug.Assert(h1.Value != IntPtr.Zero);
        Console.WriteLine($"  dog.GetType() == typeof(Dog): {ReferenceEquals(t1, t2)}");
        Console.WriteLine($"  TypeHandle.Value=0x{h1.Value:X}");
        Console.WriteLine($"  Type.GetTypeHandle(dog).Value=0x{Type.GetTypeHandle(dog).Value:X}");
        Debug.Assert(Type.GetTypeHandle(dog).Value == h1.Value);

        // Two instances share the same MethodTable (type identity)
        Dog dog2 = new Dog("Sam");
        Debug.Assert(Type.GetTypeHandle(dog).Value == Type.GetTypeHandle(dog2).Value);
        Console.WriteLine("  Two Dog instances share one TypeHandle (one MethodTable).");
    }

    private static void DemoAllocationViaMethodTable()
    {
        Console.WriteLine("-- allocation measured (MethodTable drives object size) --");
        // Warm types so first-time load costs don't pollute the sample.
        _ = new Empty();
        _ = new Dog("warm");
        _ = new WithFields(1, 2, "w");

        long before = GC.GetAllocatedBytesForCurrentThread();
        Empty e = new Empty();
        long afterEmpty = GC.GetAllocatedBytesForCurrentThread();
        Dog d = new Dog("Rex");
        long afterDog = GC.GetAllocatedBytesForCurrentThread();
        WithFields f = new WithFields(1, 2, "name");
        long afterFields = GC.GetAllocatedBytesForCurrentThread();

        long emptyCost = afterEmpty - before;
        long dogCost = afterDog - afterEmpty;
        long fieldsCost = afterFields - afterDog;
        Console.WriteLine($"  new Empty() Δalloc={emptyCost} bytes");
        Console.WriteLine($"  new Dog(...) Δalloc={dogCost} bytes");
        Console.WriteLine($"  new WithFields(...) Δalloc={fieldsCost} bytes");
        Debug.Assert(emptyCost > 0, "empty reference type still has header+MT");
        Debug.Assert(fieldsCost >= emptyCost, "more fields ⇒ at least as large");
        GC.KeepAlive(e);
        GC.KeepAlive(d);
        GC.KeepAlive(f);
        Console.WriteLine("  MethodTable encodes instance size used by the allocator.");
    }

    private static void DemoSharedGenericMethodTables()
    {
        Console.WriteLine("-- generics: closed types have distinct TypeHandles --");
        Type listStr = typeof(List<string>);
        Type listObj = typeof(List<object>);
        Type listInt = typeof(List<int>);
        Debug.Assert(listStr.GetGenericTypeDefinition() == listInt.GetGenericTypeDefinition());
        Debug.Assert(listStr.TypeHandle.Value != listInt.TypeHandle.Value);
        Debug.Assert(listStr.TypeHandle.Value != listObj.TypeHandle.Value);
        Console.WriteLine($"  List<string> TH=0x{listStr.TypeHandle.Value:X}");
        Console.WriteLine($"  List<object> TH=0x{listObj.TypeHandle.Value:X}");
        Console.WriteLine($"  List<int>    TH=0x{listInt.TypeHandle.Value:X}");
        Console.WriteLine("  Ref-type T often share native code; value-type T get specialized MTs/code.");
    }

    private static void DemoRuntimeHelpers()
    {
        Console.WriteLine("-- RuntimeHelpers surface --");
        Console.WriteLine($"  IsReferenceOrContainsReferences<int>={RuntimeHelpers.IsReferenceOrContainsReferences<int>()}");
        Console.WriteLine($"  IsReferenceOrContainsReferences<string>={RuntimeHelpers.IsReferenceOrContainsReferences<string>()}");
        Console.WriteLine($"  IsReferenceOrContainsReferences<WithFields>={RuntimeHelpers.IsReferenceOrContainsReferences<WithFields>()}");
        Debug.Assert(!RuntimeHelpers.IsReferenceOrContainsReferences<int>());
        Debug.Assert(RuntimeHelpers.IsReferenceOrContainsReferences<string>());
        Debug.Assert(RuntimeHelpers.IsReferenceOrContainsReferences<WithFields>());

        object o = new Dog("id");
        int idHash = RuntimeHelpers.GetHashCode(o);
        Console.WriteLine($"  RuntimeHelpers.GetHashCode(obj)={idHash} (identity hash / sync block)");
        Debug.Assert(idHash != 0 || idHash == 0); // any int is fine; just force evaluation
        Debug.Assert(typeof(Dog).GetMethod(nameof(Dog.Speak)) is not null);
        Console.WriteLine("  Reflection walks metadata; dispatch uses MethodTable slots.");
    }

    private sealed class Empty;

    private sealed class Dog(string name)
    {
        private readonly string _name = name;
        public string Speak() => $"woof:{_name}";
    }

    private sealed class WithFields(int x, int y, string name)
    {
        public int X = x;
        public int Y = y;
        public string Name = name;
    }
}
