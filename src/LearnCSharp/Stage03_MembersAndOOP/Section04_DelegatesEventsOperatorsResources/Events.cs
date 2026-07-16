// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第4部分-委托事件运算符资源管理.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section04_DelegatesEventsOperatorsResources
// Item     : Events
// Topic id : stage03/section04/events
//
// 步骤 3：event 封装、EventHandler 模式、自定义 add/remove、退订陷阱。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section04;

internal static class Events
{
    [LearnTopic("stage03/section04/events")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Events ===");
        DemoFieldStyleEvent();
        DemoStandardPattern();
        DemoCustomAccessors();
        DemoUnsubscribeRequiresSameInstance();
        return 0;
    }

    private static void DemoFieldStyleEvent()
    {
        Console.WriteLine("-- 字段式事件：外部只能 += / -= --");
        var b = new Button();
        int clicks = 0;
        b.Clicked += (_, _) => clicks++;
        b.SimulateClick();
        b.SimulateClick();
        Debug.Assert(clicks == 2);
        // b.Clicked = null; // ❌
        // b.Clicked.Invoke(...); // ❌
        Console.WriteLine($"  clicks={clicks}");
    }

    private static void DemoStandardPattern()
    {
        Console.WriteLine("-- EventHandler<T> + OnXxx --");
        var t = new Thermostat();
        double seen = 0;
        t.TemperatureChanged += (_, e) => seen = e.NewTemperature;
        t.Set(36.5);
        Debug.Assert(Math.Abs(seen - 36.5) < 1e-9);
        Console.WriteLine($"  TemperatureChanged => {seen}");
    }

    private static void DemoCustomAccessors()
    {
        Console.WriteLine("-- 自定义 add/remove --");
        var src = new CustomEventSource();
        int n = 0;
        EventHandler<EventArgs> h = (_, _) => n++;
        src.Changed += h;
        src.Raise();
        Debug.Assert(n == 1);
        src.Changed -= h;
        src.Raise();
        Debug.Assert(n == 1);
        Console.WriteLine($"  after unsub n={n}");
    }

    private static void DemoUnsubscribeRequiresSameInstance()
    {
        Console.WriteLine("-- 退订需同一委托实例(lambda 陷阱) --");
        Button leaky = new();
        int n = 0;
        // 错误：两次 lambda 是不同实例
        leaky.Clicked += (_, _) => n++;
        leaky.Clicked -= (_, _) => n++; // 退订失败
        leaky.SimulateClick();
        Debug.Assert(n == 1);

        // 正确：保存引用
        Button clean = new();
        n = 0;
        EventHandler h = (_, _) => n++;
        clean.Clicked += h;
        clean.Clicked -= h;
        clean.SimulateClick();
        Debug.Assert(n == 0);
        Console.WriteLine("  save handler ref to unsubscribe successfully");
    }

    private sealed class Button
    {
        public event EventHandler? Clicked;
        public void SimulateClick() => Clicked?.Invoke(this, EventArgs.Empty);
    }

    private sealed class TemperatureChangedEventArgs : EventArgs
    {
        public double NewTemperature { get; init; }
    }

    private class Thermostat
    {
        public event EventHandler<TemperatureChangedEventArgs>? TemperatureChanged;

        protected virtual void OnTemperatureChanged(double t) =>
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs { NewTemperature = t });

        public void Set(double t) => OnTemperatureChanged(t);
    }

    private sealed class CustomEventSource
    {
        private EventHandler<EventArgs>? _changed;
        public event EventHandler<EventArgs> Changed
        {
            add => _changed += value;
            remove => _changed -= value;
        }

        public void Raise() => _changed?.Invoke(this, EventArgs.Empty);
    }
}
