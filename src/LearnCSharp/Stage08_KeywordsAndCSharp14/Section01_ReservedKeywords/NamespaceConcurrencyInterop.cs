// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : NamespaceConcurrencyInterop (十二、命名空间、并发、互操作)
// Topic id : stage08/section01/namespace_concurrency_interop
//
// namespace / using / lock。

using System.Diagnostics;
using LearnCSharp.Topics;
using AliasList = System.Collections.Generic.List<int>;

namespace LearnCSharp.Stage08.Section01;

internal static class NamespaceConcurrencyInterop
{
    [LearnTopic("stage08/section01/namespace_concurrency_interop")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== NamespaceConcurrencyInterop ===");
        DemoNamespaceAndUsing();
        DemoUsingDispose();
        DemoLock();
        return 0;
    }

    private static void DemoNamespaceAndUsing()
    {
        Console.WriteLine("-- namespace / using 指令 / 别名 --");
        // using System.Diagnostics 已在文件顶；using 别名 AliasList
        AliasList list = [1, 2, 3];
        Debug.Assert(list.Count == 3);
        // global:: 限定
        global::System.Text.StringBuilder sb = new();
        sb.Append("ns");
        Debug.Assert(sb.ToString() == "ns");
        Console.WriteLine($"  AliasList count={list.Count}, global::StringBuilder={sb}");
    }

    private static void DemoUsingDispose()
    {
        Console.WriteLine("-- using 语句 / 声明（确定性释放） --");
        int disposed = 0;
        using (ProbeResource r = new ProbeResource(() => disposed++))
        {
            Debug.Assert(!r.IsDisposed);
            r.Touch();
        }
        Debug.Assert(disposed == 1);

        using ProbeResource r2 = new ProbeResource(() => disposed++);
        r2.Touch();
        // 方法结束时释放
        Console.WriteLine($"  disposed count after block={disposed} (r2 still open until return)");
    }

    private static void DemoLock()
    {
        Console.WriteLine("-- lock 互斥 --");
        object gate = new();
        int counter = 0;
        Thread[] threads = new Thread[4];
        for (int t = 0; t < threads.Length; t++)
        {
            threads[t] = new Thread(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    lock (gate)
                    {
                        counter++;
                    }
                }
            });
            threads[t].Start();
        }
        foreach (Thread th in threads) th.Join();
        Debug.Assert(counter == 4000);
        Console.WriteLine($"  lock counter={counter}");
    }

    private sealed class ProbeResource(Action onDispose) : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Touch() => Debug.Assert(!IsDisposed);
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            onDispose();
        }
    }
}
