using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Interop;

// Alias avoids ambiguity with System.Windows.Condition / the Automation static class.
// 别名用于避开 System.Windows.Condition 与 Automation 静态类的歧义。
using AutomationCondition = System.Windows.Automation.Condition;

namespace Tests.Wpf.AutomationPeer;

// Repro of the archived MSDN post "WPF memory leak with Automation Peer":
// add TabItems, remove them all, force GC - the TabItems stay alive,
// rooted through the UI Automation infrastructure (see README.md).
// 复现 MSDN 存档帖"WPF memory leak with Automation Peer"：添加 TabItem、全部移除、强制 GC，
// TabItem 仍经由 UI Automation 基础设施被根引用而无法回收（详见 README.md）。
public partial class MainWindow : Window
{
    private const int BATCH_SIZE = 10;

    // Weak refs only report survivors; they never root the TabItems themselves.
    // 弱引用只用于统计存活数，本身不会根住 TabItem。
    private readonly List<WeakReference> _trackedItems = [];

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnAddClick(object sender, RoutedEventArgs e)
    {
        for (var i = 0; i < BATCH_SIZE; i++)
        {
            // Unique headers ("wwww-0", "wwww-1", ...) make instances filterable in dotMemory.
            // Header 加编号（wwww-0、wwww-1…）便于在 dotMemory 中按实例筛选。
            var item = new TabItem { Header = $"wwww-{_trackedItems.Count}" };
            item.Content = new Grid();
            TabRegion.Items.Add(item);
            _trackedItems.Add(new WeakReference(item));
        }

        UpdateStatus();
    }

    private void OnRemoveClick(object sender, RoutedEventArgs e)
    {
        // Same sequence as the original post.
        // 与原帖一致的移除序列。
        TabRegion.SelectedItem = null;
        TabRegion.SelectedValue = null;

        if (TabRegion.Items.Count > 0)
        {
            TabRegion.Items.Clear();
        }

        UpdateStatus();
    }

    private void OnSimulateUiaQueryClick(object sender, RoutedEventArgs e)
    {
        // FromHandle + FindAll mimics a UIA client such as Inspect.exe; it forces WPF
        // to materialize the automation peer tree, which is created lazily on .NET 10.
        // FromHandle + FindAll 等价于 Inspect.exe 等 UIA 客户端查询，
        // 强制 WPF 实例化自动化 Peer 树（.NET 10 上 Peer 为懒创建）。
        var root = AutomationElement.FromHandle(new WindowInteropHelper(this).Handle);
        root.FindAll(TreeScope.Descendants, AutomationCondition.TrueCondition);

        UpdateStatus();
    }

    private void OnForceGcClick(object sender, RoutedEventArgs e)
    {
        // The original post uses WaitForFullGCComplete(), which is unreliable under
        // workstation GC; collect-finalize-collect is the deterministic sequence.
        // 原帖的 WaitForFullGCComplete() 在工作站 GC 下不可靠；
        // Collect-Finalize-Collect 三连才是确定性写法。
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        UpdateStatus();
    }

    private void UpdateStatus()
    {
        var alive = _trackedItems.Count(static reference => reference.IsAlive);
        StatusText.Text = $"Tracked TabItems: {_trackedItems.Count}, alive after last GC: {alive}";
    }
}
