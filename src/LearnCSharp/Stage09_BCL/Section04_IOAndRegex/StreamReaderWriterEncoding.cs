// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第4部分-IO与正则.md
// Stage    : Stage09_BCL
// Section  : Section04_IOAndRegex
// Item     : StreamReaderWriterEncoding
// Topic id : stage09/section04/stream_reader_writer_encoding
//
// 步骤 2：Stream 体系 + StreamReader/Writer + 编码

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section04;

internal static class StreamReaderWriterEncoding
{
    [LearnTopic("stage09/section04/stream_reader_writer_encoding")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== StreamReaderWriterEncoding ===");
        DemoMemoryStream();
        DemoReaderWriterUtf8();
        DemoEncodingRoundTrip();
        return 0;
    }

    private static void DemoMemoryStream()
    {
        Console.WriteLine("-- MemoryStream as in-memory Stream --");
        using MemoryStream ms = new MemoryStream();
        byte[] payload = Encoding.UTF8.GetBytes("stream-data");
        ms.Write(payload, 0, payload.Length);
        ms.Position = 0;
        byte[] buffer = new byte[payload.Length];
        int read = ms.Read(buffer, 0, buffer.Length);
        Debug.Assert(read == payload.Length);
        Debug.Assert(Encoding.UTF8.GetString(buffer) == "stream-data");
        Console.WriteLine($"  read {read} bytes from MemoryStream");
    }

    private static void DemoReaderWriterUtf8()
    {
        Console.WriteLine("-- StreamWriter / StreamReader UTF-8 temp file --");
        string path = Path.Join(Path.GetTempPath(), $"learn-rw-{Guid.NewGuid():N}.txt");
        try
        {
            using (StreamWriter writer = new StreamWriter(path, append: false, Encoding.UTF8))
            {
                writer.WriteLine("line1");
                writer.WriteLine("中文");
            }

            using StreamReader reader = new StreamReader(path, Encoding.UTF8);
            string all = reader.ReadToEnd();
            Debug.Assert(all.Contains("line1", StringComparison.Ordinal));
            Debug.Assert(all.Contains("中文", StringComparison.Ordinal));
            Console.WriteLine($"  file content:\n{all.TrimEnd()}");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    private static void DemoEncodingRoundTrip()
    {
        Console.WriteLine("-- Encoding.UTF8 GetBytes / GetString --");
        string s = "café 😀";
        byte[] utf8 = Encoding.UTF8.GetBytes(s);
        string back = Encoding.UTF8.GetString(utf8);
        Debug.Assert(back == s);
        // UTF-16 LE is how string stores in memory; file IO usually UTF-8
        byte[] utf16 = Encoding.Unicode.GetBytes(s);
        Debug.Assert(utf16.Length >= utf8.Length || utf16.Length < utf8.Length);
        Console.WriteLine($"  UTF-8 bytes={utf8.Length}; UTF-16LE bytes={utf16.Length}");
    }
}
