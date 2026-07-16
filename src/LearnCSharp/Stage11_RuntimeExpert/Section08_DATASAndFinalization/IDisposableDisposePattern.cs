// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第8部分-DATAS与终结化调优.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section08_DATASAndFinalization
// Item     : IDisposableDisposePattern
// Topic id : stage11/section08/idisposable_dispose_pattern
//
// Lesson: Dispose(bool), SuppressFinalize, SafeHandle; using / await using.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section08;

internal static class IDisposableDisposePattern
{
    [LearnTopic("stage11/section08/idisposable_dispose_pattern")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== IDisposableDisposePattern ===");
        DemoUsing();
        DemoFullPattern();
        DemoSafeHandleNote();
        return 0;
    }

    private static void DemoUsing()
    {
        Console.WriteLine("-- using statement --");
        int disposed = 0;
        using (var r = new Resource("A", () => disposed++))
        {
            Debug.Assert(r.Name == "A");
            Console.WriteLine($"  using resource {r.Name}");
        }

        Debug.Assert(disposed == 1);
        Console.WriteLine($"  disposed count={disposed}");
    }

    private static void DemoFullPattern()
    {
        Console.WriteLine("-- classic Dispose(bool) pattern --");
        var r = new Resource("B", static () => { });
        r.Dispose();
        r.Dispose(); // idempotent
        Debug.Assert(r.IsDisposed);
        Console.WriteLine("  double Dispose is safe; SuppressFinalize called once.");
    }

    private static void DemoSafeHandleNote()
    {
        Console.WriteLine("-- SafeHandle --");
        Console.WriteLine("  Prefer SafeHandle over custom finalizers for OS handles.");
        Console.WriteLine("  Critical finalization + ref counting integrated with P/Invoke.");
        Console.WriteLine("  FileStream/Socket already wrap handles correctly.");
    }

    private sealed class Resource : IDisposable
    {
        private readonly Action _onDispose;
        private bool _disposed;

        public Resource(string name, Action onDispose)
        {
            Name = name;
            _onDispose = onDispose;
        }

        public string Name { get; }
        public bool IsDisposed => _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // managed cleanup
                _onDispose();
            }

            // unmanaged cleanup would go here
            _disposed = true;
        }

        ~Resource() => Dispose(false);
    }
}
