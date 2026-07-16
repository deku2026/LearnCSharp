// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第2部分-迭代器.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section02_Iterators
// Item     : CompilerGeneratedStateMachine
// Topic id : stage05/section02/compiler_generated_state_machine
//
// 步骤 3：编译器状态机概念——局部→字段、state 调度、MoveNext。

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section02;

internal static class CompilerGeneratedStateMachine
{
    [LearnTopic("stage05/section02/compiler_generated_state_machine")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CompilerGeneratedStateMachine ===");
        DemoManualStateMachine();
        DemoEnumeratorShape();
        DemoMultipleEnumerators();
        DemoStateConcept();
        return 0;
    }

    private static void DemoManualStateMachine()
    {
        Console.WriteLine("-- 手写状态机 ≈ 编译器对 yield 做的事 --");
        using SquaresEnumerator e = new(4);
        List<int> got = [];
        while (e.MoveNext())
            got.Add(e.Current);
        Debug.Assert(got.SequenceEqual([0, 1, 4, 9]));
        Console.WriteLine($"  SquaresEnumerator(4) → [{string.Join(", ", got)}]");
    }

    private static void DemoEnumeratorShape()
    {
        Console.WriteLine("-- yield 生成的对象实现 IEnumerator + IDisposable --");
        IEnumerable<int> seq = Demo(3);
        using IEnumerator<int> e = seq.GetEnumerator();
        Debug.Assert(e.MoveNext() && e.Current == 0);
        Debug.Assert(e.MoveNext() && e.Current == 1);
        Debug.Assert(e.MoveNext() && e.Current == 4);
        Debug.Assert(!e.MoveNext());
        Type t = e.GetType();
        Debug.Assert(typeof(IDisposable).IsAssignableFrom(t));
        Console.WriteLine($"  runtime type={t.Name} implements IDisposable");
    }

    private static IEnumerable<int> Demo(int n)
    {
        for (int i = 0; i < n; i++)
            yield return i * i;
    }

    private static void DemoMultipleEnumerators()
    {
        Console.WriteLine("-- 每次 GetEnumerator 新状态机实例 --");
        IEnumerable<int> seq = Demo(2);
        List<int> a = seq.ToList();
        List<int> b = seq.ToList();
        Debug.Assert(a.SequenceEqual(b) && a.SequenceEqual([0, 1]));
        Console.WriteLine("  two enumerations independent, same values");
    }

    private static void DemoStateConcept()
    {
        Console.WriteLine("-- 概念：局部变量→字段，state 记录暂停点 --");
        Console.WriteLine("  MoveNext: switch(state) → run to next yield → save current+state");
        Console.WriteLine("  async/await 用极相似状态机（阶段 7）");
        FieldInfo[] fields = typeof(SquaresEnumerator).GetFields(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Debug.Assert(fields.Any(f => f.Name is "state" or "_state" || f.Name.Contains("state", StringComparison.OrdinalIgnoreCase)));
        Console.WriteLine($"  manual machine fields: {string.Join(", ", fields.Select(f => f.Name))}");
    }

    /// <summary>手写简化版：for i in 0..n-1 yield i*i。</summary>
    private sealed class SquaresEnumerator(int n) : IEnumerator<int>
    {
        private int _state; // 0=start, 1=after yield, -1=done
        private int _i;
        private readonly int _n = n;
        private int _current;

        public int Current => _current;
        object System.Collections.IEnumerator.Current => Current;

        public bool MoveNext()
        {
            switch (_state)
            {
                case 0:
                    _i = 0;
                    goto case 1;
                case 1:
                    if (_i >= _n)
                    {
                        _state = -1;
                        return false;
                    }
                    _current = _i * _i;
                    _i++;
                    _state = 1;
                    return true;
                default:
                    return false;
            }
        }

        public void Reset() => throw new NotSupportedException();
        public void Dispose() => _state = -1;
    }
}
