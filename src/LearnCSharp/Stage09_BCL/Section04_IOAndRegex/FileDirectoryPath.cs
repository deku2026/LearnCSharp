// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第4部分-IO与正则.md
// Stage    : Stage09_BCL
// Section  : Section04_IOAndRegex
// Item     : FileDirectoryPath
// Topic id : stage09/section04/file_directory_path
//
// 步骤 1：File / Directory / Path 跨平台路径

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section04;

internal static class FileDirectoryPath
{
    [LearnTopic("stage09/section04/file_directory_path")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== FileDirectoryPath ===");
        DemoFileAndDirectory();
        DemoPathApis();
        return 0;
    }

    private static void DemoFileAndDirectory()
    {
        Console.WriteLine("-- File + Directory on temp path --");
        string root = Path.Join(Path.GetTempPath(), $"learn-io-{Guid.NewGuid():N}");
        try
        {
            Directory.CreateDirectory(root);
            string file = Path.Join(root, "note.txt");
            File.WriteAllText(file, "hello");
            Debug.Assert(File.Exists(file));
            string text = File.ReadAllText(file);
            Debug.Assert(text == "hello");
            File.AppendAllText(file, " world");
            Debug.Assert(File.ReadAllText(file) == "hello world");
            string[] files = Directory.GetFiles(root);
            Debug.Assert(files.Length == 1);
            Console.WriteLine($"  wrote {file}; content length={File.ReadAllText(file).Length}");
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    private static void DemoPathApis()
    {
        Console.WriteLine("-- Path: join/combine without hardcoding separators --");
        string combined = Path.Combine("a", "b", "c.txt");
        string full = Path.GetFullPath(combined);
        string? dir = Path.GetDirectoryName(full);
        string name = Path.GetFileName(full);
        string ext = Path.GetExtension(full);
        char sep = Path.DirectorySeparatorChar;
        Debug.Assert(name == "c.txt" && ext == ".txt");
        Debug.Assert(dir is not null);
        string changed = Path.ChangeExtension("report.log", ".txt");
        Debug.Assert(changed.EndsWith(".txt", StringComparison.Ordinal));
        Console.WriteLine($"  Combine → {combined}; sep='{sep}'; file={name}");
        Console.WriteLine($"  full={full}");
    }
}
