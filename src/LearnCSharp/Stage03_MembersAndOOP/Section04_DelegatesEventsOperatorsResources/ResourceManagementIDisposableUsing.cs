// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第4部分-委托事件运算符资源管理.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section04_DelegatesEventsOperatorsResources
// Item     : ResourceManagementIDisposableUsing
// Topic id : stage03/section04/resource_management_idisposable_using
//
// 步骤 5：IDisposable/using、IAsyncDisposable、Dispose 模式、SafeHandle、终结器安全网。
//
// 🔶 C++ RAII 对照：
//   C++ 靠析构函数确定性释放（作用域结束即 ~T()）。
//   C# 无确定性析构：GC 回收时间不确定；确定性释放靠 IDisposable + using（≈ 显式 RAII）。
//   终结器 (~T) 只是非托管资源的安全网，绝非 C++ 析构的等价物。
//   Deconstruct(out ...) 是模式匹配/元组拆解语法，与终结器毫无关系。

using System.Diagnostics;
using System.Runtime.InteropServices;
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
        DemoDisposeOrderLifo();
        DemoDisposeOnException();
        DemoAsyncDisposable();
        DemoFullDisposePattern();
        DemoSafeHandleLike();
        DemoDeconstructIsNotFinalizer();
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

    private static void DemoDisposeOrderLifo()
    {
        Console.WriteLine("-- Dispose 顺序：嵌套 using 逆序（LIFO，像栈） --");
        var order = new List<string>();
        using (var outer = new TrackedResource(() => order.Add("outer")))
        using (var inner = new TrackedResource(() => order.Add("inner")))
        {
            outer.Write("o");
            inner.Write("i");
            Debug.Assert(order.Count == 0);
        }
        // 先 inner 后 outer（与声明顺序相反）
        Debug.Assert(order.Count == 2);
        Debug.Assert(order[0] == "inner" && order[1] == "outer");
        Console.WriteLine($"  dispose order: {string.Join(" → ", order)}");
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
        Console.WriteLine($"  unmanaged released={unmanagedReleases}（Dispose 路径调 SuppressFinalize，避免终结器再跑）");
    }

    private static void DemoSafeHandleLike()
    {
        Console.WriteLine("-- SafeHandle 迷你演示（可靠释放非托管句柄） --");
        // SafeHandle 是 CLR 对“句柄 + 临界终结”的标准封装：
        // Dispose → ReleaseHandle；若忘记 Dispose，critical finalizer 仍会释放。
        int releases = 0;
        using (var h = new DemoSafeHandle(() => releases++))
        {
            Debug.Assert(!h.IsInvalid);
            Debug.Assert(!h.IsClosed);
            Console.WriteLine($"  handle={h.DangerousGetHandle()} IsInvalid={h.IsInvalid}");
        }
        Debug.Assert(releases == 1);
        Debug.Assert(releases == 1); // 仅释放一次
        Console.WriteLine($"  ReleaseHandle 调用次数={releases}（using 结束确定性释放）");
    }

    private static void DemoDeconstructIsNotFinalizer()
    {
        Console.WriteLine("-- Deconstruct ≠ 终结器（~T） --");
        // Deconstruct：编译器用于 (x, y) = p 与位置模式；与资源释放无关
        var p = new Coord(3, 4);
        p.Deconstruct(out int x, out int y);
        Debug.Assert(x == 3 && y == 4);
        (int a, int b) = p;
        Debug.Assert(a == 3 && b == 4);
        // 终结器 ~T：GC 回收前的安全网，时间不确定；Dispose 路径应 GC.SuppressFinalize
        Console.WriteLine("  Deconstruct = 拆解语法；~T = GC 安全网；IDisposable = 确定性释放");
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

    /// <summary>教学用 SafeHandle：用递增假句柄演示 ReleaseHandle + 所有权。</summary>
    private sealed class DemoSafeHandle : SafeHandle
    {
        private static int s_next = 1;
        private readonly Action _onRelease;

        public DemoSafeHandle(Action onRelease)
            : base(invalidHandleValue: IntPtr.Zero, ownsHandle: true)
        {
            _onRelease = onRelease;
            SetHandle(new IntPtr(Interlocked.Increment(ref s_next)));
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            _onRelease();
            return true;
        }
    }

    private readonly struct Coord(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
    }
}
