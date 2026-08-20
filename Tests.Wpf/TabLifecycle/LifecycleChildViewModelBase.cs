using System.Collections.Concurrent;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.TabLifecycle;

// Base class for all child viewmodels of the tab lifecycle demo.
// Logs construction with a per-type instance id; Note is an editable state probe whose
// value survives tab switches (TabControl keeps content alive) but not page re-navigation.
// 子 VM 基类：构造时按类型编号并记录日志；Note 用于观察切换 tab 后状态是否保留。
public abstract partial class LifecycleChildViewModelBase : ObservableObject
{
    private static readonly ConcurrentDictionary<Type, int> instanceCounters = new();

    public int InstanceId { get; }

    [ObservableProperty]
    public partial string Note { get; set; } = string.Empty;

    protected LifecycleChildViewModelBase()
    {
        InstanceId = instanceCounters.AddOrUpdate(GetType(), 1, static (_, count) => count + 1);

        TabLifecycleLogger.Log("VM", "ctor", $"{GetType().Name} #{InstanceId}");
        TabLifecycleLogger.Track(this, $"{GetType().Name} #{InstanceId}");
    }
}
