using System.Collections.Specialized;
using System.Windows;

namespace Tests.Wpf.TabLifecycle;

// Page host: injects the page viewmodel plus the three view-first child views
// (resolved as transient by the container) and assigns them to the tab items.
// The viewmodel-first child vms are injected into TabLifecycleViewModel instead.
// 页面宿主：注入页面 VM 与三个 view-first 子 View（容器 transient 解析）并挂到 TabItem；
// VM-first 子 VM 注入到 TabLifecycleViewModel。
public partial class TabLifecycleView : LifecycleTabViewBase
{
    public TabLifecycleView(TabLifecycleViewModel viewModel,
        ViewFirst.TabOneView tabOne,
        ViewFirst.TabTwoView tabTwo,
        ViewFirst.TabThreeView tabThree)
    {
        InitializeComponent();

        DataContext = viewModel;

        ViewFirstTabOne.Content = tabOne;
        ViewFirstTabTwo.Content = tabTwo;
        ViewFirstTabThree.Content = tabThree;

        // Subscribe while loaded and unsubscribe while unloaded, so a transient page
        // never leaks its handler into the static logger's CollectionChanged.
        // Loaded 时订阅、Unloaded 时取消订阅，避免 transient 页面向静态日志器泄漏事件处理器。
        Loaded += (_, _) => TabLifecycleLogger.Entries.CollectionChanged += LogEntries_CollectionChanged;
        Unloaded += (_, _) => TabLifecycleLogger.Entries.CollectionChanged -= LogEntries_CollectionChanged;
    }

    // Keep the log list scrolled to the newest entry.
    // 日志列表自动滚动到最新条目。
    private void LogEntries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && LogList.Items.Count > 0)
        {
            LogList.ScrollIntoView(LogList.Items[^1]);
        }
    }

    private void CheckAlive_Click(object sender, RoutedEventArgs e)
    {
        TabLifecycleLogger.CheckAlive();
    }

    private void ClearLog_Click(object sender, RoutedEventArgs e)
    {
        TabLifecycleLogger.Clear();
    }
}
