// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第2部分-CLR对象模型与方法表.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section02_CLRObjectModelAndMethodTable
// Item     : VirtualMethodDispatch
// Topic id : stage11/section02/virtual_method_dispatch
//
// Lesson: virtual call → load MethodTable → vtable slot; override replaces slot.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section02;

internal static class VirtualMethodDispatch
{
    [LearnTopic("stage11/section02/virtual_method_dispatch")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== VirtualMethodDispatch ===");
        DemoVtableDispatch();
        DemoSealedAndFinal();
        DemoNonVirtualCall();
        return 0;
    }

    private static void DemoVtableDispatch()
    {
        Console.WriteLine("-- virtual dispatch via MethodTable slot --");
        Animal[] zoo = [new Animal(), new Dog(), new Cat()];
        string[] expected = ["animal", "dog", "cat"];
        for (int i = 0; i < zoo.Length; i++)
        {
            string sound = zoo[i].Speak(); // callvirt
            Console.WriteLine($"  zoo[{i}].Speak() → {sound} (runtime type={zoo[i].GetType().Name})");
            Debug.Assert(sound == expected[i]);
        }

        Console.WriteLine("  IL: callvirt Animal::Speak — slot resolved from object's MT.");
    }

    private static void DemoSealedAndFinal()
    {
        Console.WriteLine("-- sealed override can enable devirtualization later --");
        Animal d = new Dog();
        Debug.Assert(d.Speak() == "dog");
        Console.WriteLine($"  Dog is sealed class → Speak slot fixed for Dog MT");
        Console.WriteLine("  JIT may devirtualize when concrete type is proven.");
    }

    private static void DemoNonVirtualCall()
    {
        Console.WriteLine("-- non-virtual: call vs callvirt --");
        Animal a = new Animal();
        string direct = a.Id(); // non-virtual instance method
        Debug.Assert(direct == "Animal");
        // base.Speak from Dog would use call (non-virtual) to Animal::Speak
        Console.WriteLine($"  non-virtual Id()={direct}");
        Console.WriteLine("  callvirt also null-checks receiver even for non-virtual methods in C#.");
    }

    private class Animal
    {
        public virtual string Speak() => "animal";
        public string Id() => "Animal";
    }

    private sealed class Dog : Animal
    {
        public override string Speak() => "dog";
    }

    private sealed class Cat : Animal
    {
        public override string Speak() => "cat";
    }
}
