using System.Collections.Concurrent;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Tests.Wpf.TabLifecycle;

// Base class for all child views of the tab lifecycle demo.
// Logs ctor/Loaded/Unloaded/DataContextChanged with a per-type instance id and counts
// Loaded occurrences, so the load timing is visible both in logs and in the UI.
// 子 View 基类：按类型编号记录构造、Loaded、Unloaded、DataContext 变化，并统计 Loaded 次数。
public abstract class LifecycleTabViewBase : UserControl, INotifyPropertyChanged
{
    private static readonly ConcurrentDictionary<Type, int> instanceCounters = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public int ViewId { get; }

    public int LoadedCount { get; private set; }

    protected LifecycleTabViewBase()
    {
        ViewId = instanceCounters.AddOrUpdate(GetType(), 1, static (_, count) => count + 1);

        TabLifecycleLogger.Log("View", "ctor", $"{GetType().Name} #{ViewId}");
        TabLifecycleLogger.Track(this, $"{GetType().Name} #{ViewId}");

        Loaded += (_, _) =>
        {
            LoadedCount++;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadedCount)));
            TabLifecycleLogger.Log("View", "Loaded", $"{GetType().Name} #{ViewId} (loaded {LoadedCount})");
        };

        Unloaded += (_, _) =>
        {
            TabLifecycleLogger.Log("View", "Unloaded", $"{GetType().Name} #{ViewId} (loaded {LoadedCount})");
        };

        DataContextChanged += (_, _) =>
        {
            var vmName = DataContext?.GetType().Name ?? "null";
            TabLifecycleLogger.Log("View", "DataContextChanged", $"{GetType().Name} #{ViewId} -> {vmName}");
        };
    }
}
