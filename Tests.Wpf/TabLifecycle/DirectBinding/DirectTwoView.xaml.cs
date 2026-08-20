namespace Tests.Wpf.TabLifecycle.DirectBinding;

// VM-first child without DataTemplate: the TabItem Content binding resolves this view
// from DI via ViewModelToViewConverter (ConverterParameter=AttachDataContext), which
// also attaches the bound vm as DataContext.
// 无 DataTemplate 的 VM-first 子项：TabItem Content 绑定经转换器从 DI 解析本视图，
// 转换器（ConverterParameter=AttachDataContext）同时把绑定的 VM 挂为 DataContext。
public partial class DirectTwoView : LifecycleTabViewBase
{
    public DirectTwoView()
    {
        InitializeComponent();
    }
}
