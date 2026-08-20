namespace Tests.Wpf.TabLifecycle.ViewFirst;

// View-first child: the viewmodel is injected in the view ctor and assigned to DataContext.
// View-first 子项：在 View 构造函数中注入 VM 并设置 DataContext。
public partial class TabTwoView : LifecycleTabViewBase
{
    public TabTwoView(TabTwoViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
