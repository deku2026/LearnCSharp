// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第1部分-反射.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section01_Reflection
// Item     : ReflectionCoreApi
// Topic id : stage13/section01/reflection_core_api
//
// Lesson: typeof / GetType / GetMethod.Invoke / Activator / BindingFlags.NonPublic.

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section01;

internal static class ReflectionCoreApi
{
    [LearnTopic("stage13/section01/reflection_core_api")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReflectionCoreApi ===");
        DemoGetType();
        DemoInvokeAndProperty();
        DemoActivatorAndPrivate();
        return 0;
    }

    private static void DemoGetType()
    {
        Console.WriteLine("-- three ways to get Type --");
        Type t1 = typeof(string);
        Type t2 = "hello".GetType();
        Type? t3 = Type.GetType("System.DateTime");
        Debug.Assert(ReferenceEquals(t1, t2));
        Debug.Assert(t3 == typeof(DateTime));
        Console.WriteLine($"  typeof(string) == \"hello\".GetType()? {ReferenceEquals(t1, t2)}");
        Console.WriteLine($"  Type.GetType(\"System.DateTime\") => {t3?.FullName}");
        Console.WriteLine("  Type.GetType(string) is the plugin/config entry (may return null).");
    }

    private static void DemoInvokeAndProperty()
    {
        Console.WriteLine("-- MethodInfo.Invoke + PropertyInfo --");
        MethodInfo? sub = typeof(string).GetMethod("Substring", [typeof(int)]);
        Debug.Assert(sub is not null);
        object? result = sub.Invoke("hello world", [6]);
        Debug.Assert(Equals(result, "world"));
        Console.WriteLine($"  Invoke Substring(6) on \"hello world\" => {result}");

        var person = new Person();
        PropertyInfo name = typeof(Person).GetProperty(nameof(Person.Name))!;
        name.SetValue(person, "Alice");
        string n = (string)name.GetValue(person)!;
        Debug.Assert(n == "Alice");
        Console.WriteLine($"  Property SetValue/GetValue => Name={n}");
    }

    private static void DemoActivatorAndPrivate()
    {
        Console.WriteLine("-- Activator + BindingFlags.NonPublic --");
        object p1 = Activator.CreateInstance(typeof(Person))!;
        object p2 = Activator.CreateInstance(typeof(Person), ["Bob", 30])!;
        Debug.Assert(p1 is Person && p2 is Person { Name: "Bob", Age: 30 });
        Console.WriteLine($"  Activator no-arg: {((Person)p1).Name}");
        Console.WriteLine($"  Activator (Bob,30): {((Person)p2).Name}, age={((Person)p2).Age}");

        FieldInfo? secret = typeof(Person).GetField("_secret",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Debug.Assert(secret is not null);
        object? v = secret.GetValue(p2);
        Debug.Assert(Equals(v, 42));
        secret.SetValue(p2, 99);
        Debug.Assert(Equals(secret.GetValue(p2), 99));
        Console.WriteLine($"  NonPublic field _secret was 42, set to {secret.GetValue(p2)}");
        Console.WriteLine("  Reflection can bypass private — useful for tests/serializers; don't abuse.");
    }

    private sealed class Person
    {
        private int _secret = 42;
        public string Name { get; set; } = "anon";
        public int Age { get; set; }

        public Person() { }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}
