using System.Diagnostics;
using System.Reflection;

namespace LearnCSharp.Topics;

/// <summary>
/// Global topic registry. Lazily reflects the executing assembly the first time
/// it is touched, collecting every static method tagged with
/// <see cref="LearnTopicAttribute"/> into a sorted dictionary keyed by topic id.
/// </summary>
public static class TopicRegistry
{
    public delegate int TopicFn(string[] args);

    private static readonly Lazy<SortedDictionary<string, TopicFn>> Topics = new(BuildRegistry);

    private static SortedDictionary<string, TopicFn> BuildRegistry()
    {
        SortedDictionary<string, TopicFn> map = new(StringComparer.Ordinal);
        Assembly asm = typeof(TopicRegistry).Assembly;
        foreach (Type t in asm.GetTypes())
        {
            const BindingFlags Flags =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
            foreach (MethodInfo m in t.GetMethods(Flags))
            {
                LearnTopicAttribute? attr = m.GetCustomAttribute<LearnTopicAttribute>();
                if (attr is null)
                {
                    continue;
                }

                if (m.ReturnType != typeof(int))
                {
                    throw new InvalidOperationException(
                        $"[LearnTopic] method '{t.FullName}.{m.Name}' must return int.");
                }

                ParameterInfo[] ps = m.GetParameters();
                if (ps.Length != 1 || ps[0].ParameterType != typeof(string[]))
                {
                    throw new InvalidOperationException(
                        $"[LearnTopic] method '{t.FullName}.{m.Name}' must take a single string[] argument.");
                }

                TopicFn fn = (TopicFn)Delegate.CreateDelegate(typeof(TopicFn), m);
                if (!map.TryAdd(attr.Id, fn))
                {
                    throw new InvalidOperationException(
                        $"Duplicate [LearnTopic] id '{attr.Id}' on '{t.FullName}.{m.Name}'.");
                }
            }
        }
        return map;
    }

    public static int Count => Topics.Value.Count;

    public static IReadOnlyCollection<string> Ids => (IReadOnlyCollection<string>)Topics.Value.Keys;

    public static int Run(string id, string[] args)
    {
        if (!Topics.Value.TryGetValue(id, out TopicFn? fn))
        {
            Console.Error.WriteLine($"LearnCSharp: unknown topic '{id}'");
            Console.Error.WriteLine("  run with no args to list available topics");
            return 2;
        }
        return fn(args);
    }

    public static int RunAll(string[] args)
    {
        Console.WriteLine($"LearnCSharp [debug]: iterating {Topics.Value.Count} topics");
        int failures = 0;
        foreach (KeyValuePair<string, TopicFn> kv in Topics.Value)
        {
            int rc;
            try
            {
                rc = kv.Value(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"  ! {kv.Key} threw {ex.GetType().Name}: {ex.Message}");
                ++failures;
                continue;
            }
            if (rc != 0)
            {
                Console.Error.WriteLine($"  ! {kv.Key} returned {rc}");
                ++failures;
            }
        }
        if (failures > 0)
        {
            Console.Error.WriteLine($"LearnCSharp [debug]: {failures} topic(s) failed");
        }
        return failures == 0 ? 0 : 1;
    }

    public static void List()
    {
        Console.WriteLine($"LearnCSharp: {Topics.Value.Count} topics registered");
        foreach (string id in Topics.Value.Keys)
        {
            Console.WriteLine($"  {id}");
        }
    }

    [Conditional("UNUSED_DEBUG_HELPER")]
    public static void Touch() { _ = Topics.Value; }
}
