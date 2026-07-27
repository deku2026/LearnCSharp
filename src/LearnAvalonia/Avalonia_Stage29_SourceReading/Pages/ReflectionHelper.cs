// Doc:    第11部分-阶段29-必读子系统源码精读.md §0.5
// Title:  ReflectionHelper —— 反射探查公共工具
// Stage:  Stage29
// Accept: 提供统一的类型/成员枚举文本化方法
// Notes:  一次性缓存结果，避免在 Render/Layout 路径里反复反射；仅用于启动期探查。
using System.Reflection;
using System.Text;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public sealed class ReflectionEntry
{
    public string Key { get; }
    public string Value { get; }
    public ReflectionEntry(string key, string value)
    {
        Key = key;
        Value = value;
    }
}

public static class ReflectionHelper
{
    public static string DescribeType(Type t)
    {
        StringBuilder sb = new();
        sb.Append("Type: ").Append(t.FullName).Append('\n');
        sb.Append("  Base: ").Append(t.BaseType?.FullName ?? "(none)").Append('\n');
        sb.Append("  Assembly: ").Append(t.Assembly.GetName().Name).Append('\n');
        Type[] ifaces = t.GetInterfaces();
        if (ifaces.Length > 0)
        {
            sb.Append("  Interfaces:\n");
            foreach (Type i in ifaces)
            {
                sb.Append("    - ").Append(i.Name).Append('\n');
            }
        }

        return sb.ToString();
    }

    public static string DescribeMethods(Type t, int max = 12)
    {
        StringBuilder sb = new();
        MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        int shown = 0;
        foreach (MethodInfo m in methods)
        {
            if (shown >= max)
            {
                sb.Append("  ... (").Append(methods.Length - shown).Append(" more)\n");
                break;
            }

            sb.Append("  ").Append(m.ReturnType.Name).Append(' ').Append(m.Name).Append('(');
            ParameterInfo[] parms = m.GetParameters();
            for (int i = 0; i < parms.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(parms[i].ParameterType.Name).Append(' ').Append(parms[i].Name);
            }

            sb.Append(")\n");
            shown++;
        }

        return sb.ToString();
    }

    public static string DescribeProperties(Type t, int max = 12)
    {
        StringBuilder sb = new();
        PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        int shown = 0;
        foreach (PropertyInfo p in props)
        {
            if (shown >= max)
            {
                sb.Append("  ... (").Append(props.Length - shown).Append(" more)\n");
                break;
            }

            sb.Append("  ").Append(p.PropertyType.Name).Append(' ').Append(p.Name).Append(" { ");
            if (p.CanRead)
            {
                sb.Append("get; ");
            }

            if (p.CanWrite)
            {
                sb.Append("set; ");
            }

            sb.Append("}\n");
            shown++;
        }

        return sb.ToString();
    }

    public static string DescribeFields(Type t, int max = 12)
    {
        StringBuilder sb = new();
        FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        int shown = 0;
        foreach (FieldInfo f in fields)
        {
            if (shown >= max)
            {
                sb.Append("  ... (").Append(fields.Length - shown).Append(" more)\n");
                break;
            }

            sb.Append("  ").Append(f.FieldType.Name).Append(' ').Append(f.Name).Append('\n');
            shown++;
        }

        return sb.ToString();
    }

    public static string DescribeHierarchy(Type? t)
    {
        StringBuilder sb = new();
        Type? cur = t;
        int depth = 0;
        while (cur is not null)
        {
            for (int i = 0; i < depth; i++)
            {
                sb.Append("  ");
            }

            sb.Append("- ").Append(cur.FullName ?? cur.Name).Append('\n');
            cur = cur.BaseType;
            depth++;
        }

        return sb.ToString();
    }
}
