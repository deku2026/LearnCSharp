// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第2部分-函数成员与构造.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section02_FunctionMembersAndConstruction
// Item     : Indexers
// Topic id : stage03/section02/indexers
//
// 步骤 5：this[] 索引器、重载 key、多参数、Index/Range。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section02;

internal static class Indexers
{
    [LearnTopic("stage03/section02/indexers")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Indexers ===");
        DemoIntAndStringIndexers();
        DemoMultiParamIndexer();
        DemoIndexAndRange();
        return 0;
    }

    private static void DemoIntAndStringIndexers()
    {
        Console.WriteLine("-- this[int] / this[string] --");
        WordList list = new WordList();
        list[0] = "hello";
        list[1] = "world";
        Debug.Assert(list[0] == "hello");
        Debug.Assert(list["world"] == "world");
        Debug.Assert(list["missing"] == "");
        Console.WriteLine($"  list[0]={list[0]}, list[\"world\"]={list["world"]}");
    }

    private static void DemoMultiParamIndexer()
    {
        Console.WriteLine("-- 二维 this[row,col] --");
        Grid grid = new Grid(2, 3);
        grid[0, 1] = 42;
        grid[1, 2] = 7;
        Debug.Assert(grid[0, 1] == 42);
        Debug.Assert(grid[1, 2] == 7);
        Console.WriteLine($"  grid[0,1]={grid[0, 1]}, grid[1,2]={grid[1, 2]}");
    }

    private static void DemoIndexAndRange()
    {
        Console.WriteLine("-- Index / Range 参数 --");
        Sliceable slice = new Sliceable([10, 20, 30, 40, 50]);
        Debug.Assert(slice[^1] == 50);
        Debug.Assert(slice[1..4] is [20, 30, 40]);
        Console.WriteLine($"  ^1={slice[^1]}, 1..4=[{string.Join(',', slice[1..4])}]");
    }

    private sealed class WordList
    {
        private readonly string[] _words = new string[100];

        public string this[int index]
        {
            get => _words[index];
            set => _words[index] = value;
        }

        public string this[string key] =>
            _words.FirstOrDefault(w => w == key) ?? "";
    }

    private sealed class Grid
    {
        private readonly int[,] _cells;
        public Grid(int rows, int cols) => _cells = new int[rows, cols];

        public int this[int row, int col]
        {
            get => _cells[row, col];
            set => _cells[row, col] = value;
        }
    }

    private sealed class Sliceable(int[] data)
    {
        public int this[Index index] => data[index];
        public int[] this[Range range] => data[range];
    }
}
