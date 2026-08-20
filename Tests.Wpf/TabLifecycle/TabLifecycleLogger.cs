using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Tests.Wpf.TabLifecycle;

// Static logger for the tab lifecycle demo.
// Every entry is timestamped, mirrored to the Debug output and appended to an observable
// collection that the page binds to, so lifecycle events can be observed in-app.
// Tab 生命周期演示的静态日志器：每条日志带时间戳，同时输出到 Debug 窗口与页面内日志列表。
public static class TabLifecycleLogger
{
    private const int MAX_ENTRIES = 500;

    private static readonly List<(WeakReference Reference, string Name)> tracked = [];

    public static ObservableCollection<string> Entries { get; } = [];

    public static void Log(string kind, string eventName, string actor)
    {
        var entry = $"{DateTime.Now:HH:mm:ss.fff} | {kind,-4} | {eventName,-19} | {actor}";

        if (Entries.Count >= MAX_ENTRIES)
        {
            Entries.RemoveAt(0);
        }

        Entries.Add(entry);
        Debug.WriteLine($"[TabLifecycle] {entry}");
    }

    // Track an instance with a weak reference so aliveness can be checked after GC.
    // 以弱引用跟踪实例，便于 GC 后检查存活状态。
    public static void Track(object instance, string name)
    {
        lock (tracked)
        {
            tracked.Add((new WeakReference(instance), name));
        }
    }

    // Force a full GC and report which tracked instances are still alive.
    // 强制完整 GC 并报告仍存活的被跟踪实例。
    public static void CheckAlive()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        lock (tracked)
        {
            var alive = tracked.Where(t => t.Reference.IsAlive)
                .Select(t => t.Name)
                .ToList();

            Log("SYS", "gc-check", $"{alive.Count} alive: {string.Join(", ", alive)}");
        }
    }

    public static void Clear()
    {
        Entries.Clear();
    }
}
