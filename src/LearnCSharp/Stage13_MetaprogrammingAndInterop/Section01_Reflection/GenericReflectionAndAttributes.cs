// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第1部分-反射.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section01_Reflection
// Item     : GenericReflectionAndAttributes
// Topic id : stage13/section01/generic_reflection_and_attributes
//
// Lesson: MakeGenericType/Method + attributes as reflectable metadata.

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section01;

internal static class GenericReflectionAndAttributes
{
    [LearnTopic("stage13/section01/generic_reflection_and_attributes")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== GenericReflectionAndAttributes ===");
        DemoMakeGenericType();
        DemoMakeGenericMethod();
        DemoCustomAttributes();
        return 0;
    }

    private static void DemoMakeGenericType()
    {
        Console.WriteLine("-- MakeGenericType (reified generics) --");
        Type open = typeof(List<>);
        Type closed = open.MakeGenericType(typeof(int));
        object list = Activator.CreateInstance(closed)!;
        MethodInfo add = closed.GetMethod("Add")!;
        add.Invoke(list, [10]);
        add.Invoke(list, [20]);
        int count = (int)closed.GetProperty("Count")!.GetValue(list)!;
        Debug.Assert(count == 2 && closed == typeof(List<int>));
        Console.WriteLine($"  List<> → List<int>, Count={count}");
        Console.WriteLine("  Runtime retains type args → can build List<runtimeType> (C++ templates cannot).");
    }

    private static void DemoMakeGenericMethod()
    {
        Console.WriteLine("-- MakeGenericMethod --");
        MethodInfo open = typeof(Helper).GetMethod(nameof(Helper.Process))!;
        MethodInfo closed = open.MakeGenericMethod(typeof(string));
        object? r = closed.Invoke(null, ["hi"]);
        Debug.Assert(Equals(r, "hi:String")); // typeof(string).Name == "String"
        Console.WriteLine($"  Process<string>(\"hi\") => {r}");
    }

    private static void DemoCustomAttributes()
    {
        Console.WriteLine("-- custom attributes drive framework behavior --");
        foreach (PropertyInfo p in typeof(User).GetProperties())
        {
            ColumnAttribute? col = p.GetCustomAttribute<ColumnAttribute>();
            if (col is not null)
                Console.WriteLine($"  {p.Name} → DB column '{col.Name}'");
        }

        ColumnAttribute? nameCol = typeof(User).GetProperty(nameof(User.Name))!
            .GetCustomAttribute<ColumnAttribute>();
        Debug.Assert(nameCol?.Name == "user_name");
        Console.WriteLine("  Attribute = data only; reflection reading it gives meaning (ORM/JSON/tests).");
    }

    private static class Helper
    {
        public static string Process<T>(T item) => $"{item}:{typeof(T).Name}";
    }

    [AttributeUsage(AttributeTargets.Property)]
    private sealed class ColumnAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }

    private sealed class User
    {
        [Column("user_name")]
        public string Name { get; set; } = "";

        [Column("user_age")]
        public int Age { get; set; }
    }
}
