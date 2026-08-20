namespace Tests.Wpf.TabLifecycle.MarkupResolved;

// View-first child resolved from DI in XAML via UserControlBindingExtension;
// the viewmodel is still injected in the view ctor and assigned to DataContext.
// 经 UserControlBindingExtension 在 XAML 中从 DI 解析的 view-first 子项；
// VM 仍由 View 构造函数注入并设置 DataContext。
public partial class ResolvedOneView : LifecycleTabViewBase
{
    public ResolvedOneView(ResolvedOneViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
