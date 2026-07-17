// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第1部分-集合体系.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section01_CollectionEcosystem
// Item     : CollectionInterfaces
// Topic id : stage05/section01/collection_interfaces
//
// 步骤 1：集合接口层次 + 接受最窄接口 + 泛型 vs 非泛型。

using System.Collections;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section01;

internal static class CollectionInterfaces
{
    [LearnTopic("stage05/section01/collection_interfaces")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CollectionInterfaces ===");
        DemoInterfaceHierarchy();
        DemoAcceptNarrowest();
        DemoGenericVsNonGeneric();
        DemoReadOnlyInterfaces();
        return 0;
    }

    private static void DemoInterfaceHierarchy()
    {
        Console.WriteLine("-- IEnumerable → ICollection → IList / IDictionary / ISet --");
        IEnumerable<int> en = new List<int> { 1, 2, 3 };
        ICollection<int> col = new List<int> { 1, 2 };
        IList<int> list = new List<int> { 10, 20 };
        IDictionary<string, int> dict = new Dictionary<string, int> { ["a"] = 1 };
        ISet<int> set = new HashSet<int> { 1, 2 };

        Debug.Assert(en.GetEnumerator() is not null);
        Debug.Assert(col.Count == 2);
        Debug.Assert(list[0] == 10);
        Debug.Assert(dict["a"] == 1);
        Debug.Assert(set.Contains(2));
        Console.WriteLine("  五类接口均可由具体集合实现");
    }

    private static void DemoAcceptNarrowest()
    {
        Console.WriteLine("-- ⭐ 参数用最窄接口：只遍历 → IEnumerable --");
        int[] arr = [1, 2, 3];
        List<int> list = [1, 2, 3];
        HashSet<int> set = [1, 2, 3];
        Debug.Assert(SumAll(arr) == 6);
        Debug.Assert(SumAll(list) == 6);
        Debug.Assert(SumAll(set) == 6);
        Console.WriteLine($"  SumAll(array/list/set) = {SumAll(arr)}（均可）");
    }

    private static int SumAll(IEnumerable<int> items)
    {
        int s = 0;
        foreach (int x in items)
            s += x;
        return s;
    }

    private static void DemoGenericVsNonGeneric()
    {
        Console.WriteLine("-- 泛型零装箱 vs ArrayList 装箱 --");
        List<int> good = [1, 2, 3];
        ArrayList bad = [1, 2, 3];
        Debug.Assert(good[0] == 1);
        Debug.Assert((int)bad[0]! == 1);
        // bad.Add("oops"); // 编译通过，运行期 cast 才炸
        Console.WriteLine("  List<int> 类型安全；ArrayList 存 object（遗留）");
    }

    private static void DemoReadOnlyInterfaces()
    {
        Console.WriteLine("-- 只读接口：IReadOnlyList / IReadOnlyCollection --");
        List<int> mut = [1, 2, 3];
        IReadOnlyList<int> ro = mut;
        Debug.Assert(ro.Count == 3 && ro[1] == 2);
        mut.Add(4);
        Debug.Assert(ro.Count == 4);
        Console.WriteLine($"  IReadOnlyList 视图 Count={ro.Count}（底层可变时跟着变）");
    }
}
