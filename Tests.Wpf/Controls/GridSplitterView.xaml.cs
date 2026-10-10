using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Tests.Wpf.Controls;

public partial class GridSplitterView : UserControl
{
    // Minimum height of an expanded panel's content area.
    // 展开面板内容区的最小高度。
    private const double MIN_PANEL_CONTENT_HEIGHT = 120;

    public GridSplitterView(GridSplitterViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void Hyperlink_RequestNavigate(
        object sender,
        System.Windows.Navigation.RequestNavigateEventArgs e
    )
    {
        Process.Start(
            new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true }
        );
    }

    private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (sender is Thumb { Tag: Grid content })
        {
            // Top edge handle: pointer up (negative delta) grows the panel, down shrinks it.
            // 顶边拖柄:指针上移(增量为负)面板变高,下移变矮。
            // The upper bound is what the layout can actually give: window height minus the
            // main row's MinHeight and everything else in the bottom stack, so the handle
            // never drags past where the layout can follow.
            // 上限取布局真正能给的高度:窗口高减去主内容行的 MinHeight 和底部堆叠中其余部分,拖柄不会拖到布局跟不上的位置。
            double maxHeight = Math.Max(
                MIN_PANEL_CONTENT_HEIGHT,
                RootGrid.ActualHeight
                    - RootGrid.RowDefinitions[0].MinHeight
                    - (BottomPanels.ActualHeight - content.ActualHeight)
            );
            content.Height = Math.Clamp(
                content.Height - e.VerticalChange,
                MIN_PANEL_CONTENT_HEIGHT,
                maxHeight
            );
        }
    }
}
