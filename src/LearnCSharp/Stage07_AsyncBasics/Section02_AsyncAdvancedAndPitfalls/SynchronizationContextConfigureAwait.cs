// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第2部分-异步进阶与陷阱.md
// Stage    : Stage07_AsyncBasics
// Section  : Section02_AsyncAdvancedAndPitfalls
// Item     : SynchronizationContextConfigureAwait
// Topic id : stage07/section02/synchronization_context_configureawait
//
// 步骤 1：await 默认捕获 SynchronizationContext；ConfigureAwait(false) 库代码准则

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section02;

internal static class SynchronizationContextConfigureAwait
{
    [LearnTopic("stage07/section02/synchronization_context_configureawait")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SynchronizationContextConfigureAwait ===");
        DemoCurrentContextOnConsole();
        DemoConfigureAwaitFalse().GetAwaiter().GetResult();
        DemoLibraryVsUiGuidance().GetAwaiter().GetResult();
        return 0;
    }

    private static void DemoCurrentContextOnConsole()
    {
        Console.WriteLine("-- console / ASP.NET Core: usually no SynchronizationContext --");
        SynchronizationContext? ctx = SynchronizationContext.Current;
        Console.WriteLine($"  SynchronizationContext.Current is null: {ctx is null}");
        Console.WriteLine("  UI (WPF/WinForms): single-thread context so await resumes on UI thread");
        Console.WriteLine("  classic ASP.NET: request context; ASP.NET Core: none by default");
        Debug.Assert(ctx is null || true);
    }

    private static async Task DemoConfigureAwaitFalse()
    {
        Console.WriteLine("-- ConfigureAwait(false): do not resume on captured context --");
        int before = Environment.CurrentManagedThreadId;
        await Task.Delay(5).ConfigureAwait(false);
        int after = Environment.CurrentManagedThreadId;
        Console.WriteLine($"  thread before={before}, after ConfigureAwait(false) continuation={after}");
        int library = await LibraryMethodAsync();
        Debug.Assert(library == 42);
        Console.WriteLine($"  LibraryMethodAsync → {library}");
    }

    private static async Task DemoLibraryVsUiGuidance()
    {
        Console.WriteLine("-- library vs UI guidance --");
        // Library code: always ConfigureAwait(false) unless context is required.
        string data = await LibraryFetchAsync("payload").ConfigureAwait(false);
        Debug.Assert(data == "PAYLOAD");
        Console.WriteLine($"  library result: {data}");
        Console.WriteLine("  UI code that touches controls after await: keep default ConfigureAwait(true)");
        Console.WriteLine("  ASP.NET Core: ConfigureAwait(false) is a no-op for context, still good for libraries");
    }

    private static async Task<int> LibraryMethodAsync()
    {
        await Task.Delay(5).ConfigureAwait(false);
        return 42;
    }

    private static async Task<string> LibraryFetchAsync(string input)
    {
        await Task.Delay(5).ConfigureAwait(false);
        return input.ToUpperInvariant();
    }
}
