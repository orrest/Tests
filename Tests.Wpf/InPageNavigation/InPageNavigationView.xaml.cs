using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tests.Wpf.InPageNavigation;

public partial class InPageNavigationView : UserControl
{
    public InPageNavigationView(InPageNavigationViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    // Click handler shared by all catalog buttons.
    // The Tag carries the target element's x:Name; we resolve it and scroll the ScrollViewer
    // so that the target's top aligns with the top of the viewport.
    // 目录按钮的共用点击处理：从 Tag 读取目标元素名，查找后滚动 ScrollViewer，使目标顶部与视口顶部对齐。
    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.Tag is not string targetName) return;

        // Resolve the target element by name within this view.
        // 在当前视图内按名称解析目标元素。
        if (FindName(targetName) is not FrameworkElement target) return;

        // BringIntoView uses "minimal scroll": if the target is already partially visible,
        // it does NOT scroll further, leaving the header in the middle of the viewport.
        // To force top-alignment, compute the target's offset relative to the ScrollViewer
        // and call ScrollToVerticalOffset directly.
        // BringIntoView 采用"最小滚动"策略：目标已部分可见时不再滚动，导致 header 停在视口中部。
        // 为强制顶对齐，直接计算目标相对 ScrollViewer 的偏移并调用 ScrollToVerticalOffset。
        ScrollToTopAligned(ContentScroll, target);
    }

    // Scroll the ScrollViewer so that target's top edge aligns with the viewport's top.
    // 将 ScrollViewer 滚动到目标顶部与视口顶部对齐的位置。
    private static void ScrollToTopAligned(ScrollViewer scrollViewer, FrameworkElement target)
    {
        // TransformToVisual gives the relative position between two visuals in the visual tree.
        // The resulting vector's Y is the target's offset from the ScrollViewer's current viewport origin.
        // TransformToVisual 返回可视树中两个元素之间的相对位置，其 Y 分量即目标相对 ScrollViewer 当前视口原点的偏移。
        var transform = target.TransformToVisual(scrollViewer);
        var position = transform.Transform(new Point(0, 0));

        // targetTopInContent = how far the target sits from the content's very top.
        // Current viewport position (VerticalOffset) + relative Y = absolute offset of target in content.
        // targetTopInContent = 目标相对内容顶部的绝对距离。
        // 当前视口位置 (VerticalOffset) + 相对 Y = 目标在内容中的绝对偏移。
        var targetTopInContent = scrollViewer.VerticalOffset + position.Y;

        // Clamp into the valid range [0, ScrollableHeight] to avoid overshooting at the last items.
        // 限制在 [0, ScrollableHeight] 范围内，避免最后几项超出可滚动范围。
        var clamped = Math.Clamp(targetTopInContent, 0, scrollViewer.ScrollableHeight);

        scrollViewer.ScrollToVerticalOffset(clamped);
    }
}
