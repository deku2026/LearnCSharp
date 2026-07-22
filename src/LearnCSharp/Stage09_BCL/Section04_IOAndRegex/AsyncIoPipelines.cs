// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第4部分-IO与正则.md
// Stage    : Stage09_BCL
// Section  : Section04_IOAndRegex
// Item     : AsyncIoPipelines
// Topic id : stage09/section04/async_io_pipelines
//
// 步骤 3：异步 IO；Pipelines 概念用 FileStream 缓冲读写演示（无额外包）

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section04;

internal static class AsyncIoPipelines
{
    [LearnTopic("stage09/section04/async_io_pipelines")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AsyncIoPipelines ===");
        DemoAsyncFileIo().GetAwaiter().GetResult();
        DemoBufferedReadWrite().GetAwaiter().GetResult();
        DemoPipelinesConcept();
        return 0;
    }

    private static async Task DemoAsyncFileIo()
    {
        Console.WriteLine("-- File.WriteAllTextAsync / ReadAllTextAsync --");
        string path = Path.Join(Path.GetTempPath(), $"learn-aio-{Guid.NewGuid():N}.txt");
        try
        {
            await File.WriteAllTextAsync(path, "async-hello", Encoding.UTF8);
            string text = await File.ReadAllTextAsync(path, Encoding.UTF8);
            Debug.Assert(text == "async-hello");
            Console.WriteLine($"  async content={text}");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    private static async Task DemoBufferedReadWrite()
    {
        Console.WriteLine("-- FileStream async Read/Write with buffer --");
        string path = Path.Join(Path.GetTempPath(), $"learn-fs-{Guid.NewGuid():N}.bin");
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(new string('x', 4096));
            await using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            {
                await fs.WriteAsync(data);
            }

            await using FileStream rs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            byte[] buf = new byte[data.Length];
            int total = 0;
            while (total < buf.Length)
            {
                int n = await rs.ReadAsync(buf.AsMemory(total, buf.Length - total));
                if (n == 0) break;
                total += n;
            }
            Debug.Assert(total == data.Length);
            Console.WriteLine($"  async FileStream transferred {total} bytes");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    private static void DemoPipelinesConcept()
    {
        Console.WriteLine("-- System.IO.Pipelines concept (high-throughput parsers) --");
        Console.WriteLine("  PipeReader/PipeWriter: backpressure + buffer pooling for network/protocol IO");
        Console.WriteLine("  Prefer FileStream async APIs for simple files; Pipelines for Kestrel-style streams");
        // educational stand-in: producer/consumer via Channel-like manual buffer
        using MemoryStream buffer = new MemoryStream();
        byte[] chunk = Encoding.UTF8.GetBytes("pipeline-chunk");
        buffer.Write(chunk);
        Debug.Assert(buffer.Length == chunk.Length);
        Console.WriteLine($"  mini buffer length={buffer.Length} (stand-in for PipeWriter)");
    }
}
