// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第1部分-地基.md
// Stage    : Stage02_TypeSystem
// Section  : Section01_Foundations
// Item     : EqualityFullSet
// Topic id : stage02/section01/equality_full_set
//
// 步骤 5：引用相等 vs 值相等；IEquatable + Equals + GetHashCode + ==/!=；EqualityComparer。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section01;

internal static class EqualityFullSet
{
    [LearnTopic("stage02/section01/equality_full_set")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== EqualityFullSet ===");
        DemoReferenceVsValue();
        DemoStringException();
        DemoFullValueEquality();
        DemoHashSetDedup();
        DemoCustomComparer();
        DemoBrokenHashCodePitfall();
        return 0;
    }

    private static void DemoReferenceVsValue()
    {
        Console.WriteLine("-- 引用相等 vs 值相等 --");
        object a = new object(), b = a, c = new object();
        Debug.Assert(ReferenceEquals(a, b));
        Debug.Assert(!ReferenceEquals(a, c));

        PointV v1 = new(1, 2), v2 = new(1, 2);
        Debug.Assert(v1.Equals(v2)); // 值类型默认值相等
        Console.WriteLine($"  ReferenceEquals(a,b)={ReferenceEquals(a, b)}; struct.Equals={v1.Equals(v2)}");
    }

    private static void DemoStringException()
    {
        Console.WriteLine("-- string 特例：== 比值 --");
        string s1 = "hi";
        string s2 = "h" + "i";
        Debug.Assert(s1 == s2);
        Debug.Assert(s1.Equals(s2));
        Console.WriteLine($"  s1==s2={s1 == s2}（引用类型但值相等）");
    }

    private static void DemoFullValueEquality()
    {
        Console.WriteLine("-- 成套值相等实现 --");
        var p1 = new Point(1, 2);
        var p2 = new Point(1, 2);
        var p3 = new Point(9, 9);
        Debug.Assert(p1.Equals(p2));
        Debug.Assert(p1 == p2);
        Debug.Assert(p1 != p3);
        Debug.Assert(p1.GetHashCode() == p2.GetHashCode());
        Debug.Assert(!ReferenceEquals(p1, p2));
        Console.WriteLine($"  Point(1,2)==Point(1,2): {p1 == p2}, hash same={p1.GetHashCode() == p2.GetHashCode()}");
    }

    private static void DemoHashSetDedup()
    {
        Console.WriteLine("-- HashSet 用值相等去重 --");
        var set = new HashSet<Point> { new(1, 2), new(1, 2), new(3, 4) };
        Debug.Assert(set.Count == 2);
        Console.WriteLine($"  HashSet count={set.Count}（两个 (1,2) 合成一个）");
    }

    private static void DemoCustomComparer()
    {
        Console.WriteLine("-- IEqualityComparer 外部规则 --");
        var set = new HashSet<Box>(new ByVolume())
        {
            new(2, 3, 4),
            new(1, 4, 6), // 同体积 24
            new(1, 1, 1)
        };
        Debug.Assert(set.Count == 2);
        Console.WriteLine($"  ByVolume set count={set.Count}");
    }

    private static void DemoBrokenHashCodePitfall()
    {
        Console.WriteLine("-- ⚠ 只重写 Equals 不重写 GetHashCode 会坏 Dictionary --");
        // BrokenPoint 故意只改 Equals：演示“相等但哈希不同”的风险说明
        var broken = new BrokenPoint(1, 2);
        var other = new BrokenPoint(1, 2);
        Debug.Assert(broken.Equals(other));
        // GetHashCode 仍是默认引用哈希 → 通常不同
        bool sameHash = broken.GetHashCode() == other.GetHashCode();
        Console.WriteLine($"  Broken Equals={broken.Equals(other)}, hash equal?={sameHash}（常为 false）");

        // Dictionary 用 GetHashCode 选桶，再用 Equals 确认：
        // 键入 new BrokenPoint(1,2) 后用另一个相等实例查找 → 常找不到
        var map = new Dictionary<BrokenPoint, string> { [broken] = "found" };
        bool lookupOk = map.TryGetValue(other, out string? value);
        Debug.Assert(map.ContainsKey(broken)); // 同一引用通常能命中
        // 值相等但哈希不同 → 查找失败（若碰巧哈希碰撞相同则可能命中，故用“坏实现”教学）
        if (!sameHash)
        {
            Debug.Assert(!lookupOk);
            Debug.Assert(value is null);
            Console.WriteLine("  Dictionary 查找另一相等实例失败（坏 GetHashCode）");
        }
        else
        {
            Console.WriteLine("  （罕见：默认哈希碰巧相同，查找可能仍成功——仍属未定义可靠行为）");
        }
        Console.WriteLine("  铁律：Equals 与 GetHashCode 必须成对；相等 → 哈希相同");
    }

    private readonly struct PointV(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
    }

    private sealed class Point : IEquatable<Point>
    {
        public int X { get; }
        public int Y { get; }
        public Point(int x, int y) => (X, Y) = (x, y);

        public bool Equals(Point? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj) => Equals(obj as Point);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public static bool operator ==(Point? a, Point? b) => a is null ? b is null : a.Equals(b);
        public static bool operator !=(Point? a, Point? b) => !(a == b);
    }

    private sealed class Box(int w, int h, int d)
    {
        public int Volume { get; } = w * h * d;
    }

    private sealed class ByVolume : IEqualityComparer<Box>
    {
        public bool Equals(Box? x, Box? y) => x?.Volume == y?.Volume;
        public int GetHashCode(Box obj) => obj.Volume.GetHashCode();
    }

    private sealed class BrokenPoint(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
        public override bool Equals(object? obj) =>
            obj is BrokenPoint o && X == o.X && Y == o.Y;
        // 故意不重写 GetHashCode
    }
}
