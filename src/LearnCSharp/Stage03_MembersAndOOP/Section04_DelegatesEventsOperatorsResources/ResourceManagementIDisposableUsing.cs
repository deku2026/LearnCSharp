// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第4部分-委托事件运算符资源管理.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section04_DelegatesEventsOperatorsResources
// Item     : ResourceManagementIDisposableUsing
// Topic id : stage03/section04/resource_management_idisposable_using
//
// 步骤 5：IDisposable/using、IAsyncDisposable、Dispose 模式、终结器安全网。

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section04;

internal static class ResourceManagementIDisposableUsing
{
    [LearnTopic("stage03/section04/resource_management_idisposable_using")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ResourceManagementIDisposableUsing ===");
        DemoUsingStatementAndDeclaration();
        DemoDisposeOnException();
        DemoAsyncDisposable();
        DemoFullDisposePattern();
        return 0;
    }

    private static void DemoUsingStatementAndDeclaration()
    {
        Console.WriteLine("-- using 语句 / using 声明 --");
        int disposed = 0;
        using (var r = new TrackedResource(() => disposed++))
        {
            r.Write("hello");
            Debug.Assert(r.Content == "hello");
        }
        Debug.Assert(disposed == 1);

        using var r2 = new TrackedResource(() => disposed++);
        r2.Write("hi");
        Debug.Assert(r2.Content == "hi");
        // 作用域结束时 Dispose
        Console.WriteLine($"  disposed after block={disposed} (will +1 at scope end)");
    }

    private static void DemoDisposeOnException()
    {
        Console.WriteLine("-- 异常路径仍 Dispose(try/finally) --");
        int disposed = 0;
        try
        {
            using var r = new TrackedResource(() => disposed++);
            throw new InvalidOperationException("boom");
        }
        catch (InvalidOperationException)
        {
            Debug.Assert(disposed == 1);
            Console.WriteLine("  Dispose ran in finally despite throw");
        }
    }

    private static void DemoAsyncDisposable()
    {
        Console.WriteLine("-- IAsyncDisposable + await using --");
        DemoAsyncDisposableCore().GetAwaiter().GetResult();
    }

    private static async Task DemoAsyncDisposableCore()
    {
        int disposed = 0;
        await using (var c = new AsyncConnection(() => disposed++))
        {
            await c.WriteAsync("ping");
            Debug.Assert(c.Last == "ping");
        }
        Debug.Assert(disposed == 1);
        Console.WriteLine("  await using disposed async resource");
    }

    private static void DemoFullDisposePattern()
    {
        Console.WriteLine("-- 完整 Dispose 模式 + SuppressFinalize --");
        int unmanagedReleases = 0;
        using (var n = new NativeLikeResource(() => unmanagedReleases++))
        {
            Debug.Assert(n.IsOpen);
            n.Touch();
        }
        Debug.Assert(unmanagedReleases == 1);
        Console.WriteLine($"  unmanaged released={unmanagedReleases}");
    }

    private sealed class TrackedResource : IDisposable
    {
        private readonly Action _onDispose;
        private readonly StringBuilder _sb = new();
        private bool _disposed;
        public TrackedResource(Action onDispose) => _onDispose = onDispose;
        public string Content => _sb.ToString();
        public void Write(string s)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _sb.Append(s);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _onDispose();
        }
    }

    private sealed class AsyncConnection : IAsyncDisposable
    {
        private readonly Action _onDispose;
        public string Last { get; private set; } = "";
        public AsyncConnection(Action onDispose) => _onDispose = onDispose;
        public Task WriteAsync(string s)
        {
            Last = s;
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            _onDispose();
            return ValueTask.CompletedTask;
        }
    }

    private sealed class NativeLikeResource : IDisposable
    {
        private readonly Action _releaseUnmanaged;
        private bool _disposed;
        public bool IsOpen => !_disposed;
        public NativeLikeResource(Action releaseUnmanaged) => _releaseUnmanaged = releaseUnmanaged;

        public void Touch() => ObjectDisposedException.ThrowIf(_disposed, this);

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
                // 托管资源
            }
            _releaseUnmanaged();
            _disposed = true;
        }

        ~NativeLikeResource() => Dispose(false);
    }
}
