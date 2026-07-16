// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第1部分-反射.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section01_Reflection
// Item     : ReflectionPerformance
// Topic id : stage13/section01/reflection_performance
//
// Lesson: reflection cost = discovery + Invoke; ladder: cache → CreateDelegate → generators.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section01;

internal static class ReflectionPerformance
{
    private static readonly MethodInfo CachedSubstring =
        typeof(string).GetMethod(nameof(string.Substring), [typeof(int)])!;

    private static readonly Func<string, int, string> SubstringDelegate =
        (Func<string, int, string>)CachedSubstring.CreateDelegate(typeof(Func<string, int, string>));

    [LearnTopic("stage13/section01/reflection_performance")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReflectionPerformance ===");
        DemoWhySlow();
        DemoOptimizationLadder();
        DemoUnsafeAccessor();
        return 0;
    }

    private static void DemoWhySlow()
    {
        Console.WriteLine("-- why reflection is slow --");
        Console.WriteLine("  1) discovery: GetMethod walks metadata + string match every time");
        Console.WriteLine("  2) Invoke: arg validation + boxing to object[] + indirect call");
        Console.WriteLine("  .NET 7+ sped up Invoke, still far slower than direct/delegate.");
    }

    private static void DemoOptimizationLadder()
    {
        Console.WriteLine("-- optimization ladder (micro timing, not BDN) --");
        const int n = 50_000;
        string s = "hello world";

        // warm-up
        _ = s.Substring(6);
        _ = CachedSubstring.Invoke(s, [6]);
        _ = SubstringDelegate(s, 6);

        long direct = Time(() =>
        {
            for (int i = 0; i < n; i++)
                _ = s.Substring(6);
        });

        long cachedInvoke = Time(() =>
        {
            for (int i = 0; i < n; i++)
                _ = CachedSubstring.Invoke(s, [6]);
        });

        long del = Time(() =>
        {
            for (int i = 0; i < n; i++)
                _ = SubstringDelegate(s, 6);
        });

        Debug.Assert(Equals(CachedSubstring.Invoke(s, [6]), "world"));
        Debug.Assert(SubstringDelegate(s, 6) == "world");
        Console.WriteLine($"  {n} calls: direct≈{direct}ms, cached Invoke≈{cachedInvoke}ms, CreateDelegate≈{del}ms");
        Console.WriteLine("  Ladder: direct reflect → cache MethodInfo → CreateDelegate → expr tree → source gen → UnsafeAccessor");
    }

    private static void DemoUnsafeAccessor()
    {
        Console.WriteLine("-- [UnsafeAccessor] (.NET 8): native-speed private access --");
        var p = new Widget(7);
        ref int secret = ref SecretOf(p);
        Debug.Assert(secret == 7);
        secret = 11;
        Debug.Assert(SecretOf(p) == 11);
        Console.WriteLine($"  private _value via UnsafeAccessor: {SecretOf(p)} (AOT-friendly, no Invoke)");
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_value")]
    private static extern ref int SecretOf(Widget w);

    private static long Time(Action a)
    {
        Stopwatch sw = Stopwatch.StartNew();
        a();
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    private sealed class Widget(int value)
    {
        private int _value = value;
    }
}
