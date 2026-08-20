namespace Tests.Wpf.TabLifecycle.ViewModelFirst;

// ViewModel-first child: parameterless ctor; the viewmodel arrives via inherited
// DataContext from the TabItem Content binding, so it is exactly the instance the
// parent viewmodel injected — not another one resolved by this view.
// VM-first 子项：无参构造；VM 经 TabItem Content 绑定继承而来，即父 VM 注入的同一实例。
public partial class ContentOneView : LifecycleTabViewBase
{
    public ContentOneView()
    {
        InitializeComponent();
    }
}
