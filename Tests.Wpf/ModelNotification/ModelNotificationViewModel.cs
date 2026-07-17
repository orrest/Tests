using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ModelNotification;

/// <summary>
/// 组合根 ViewModel。页面里三个分区分别用三种方式演示，
/// 把三个子 ViewModel 通过构造函数注入并暴露为属性，View 里用
/// <c>DataContext="{Binding Direct}"</c> 等切换每个分区的内部 DataContext。
/// </summary>
public partial class ModelNotificationViewModel : ObservableObject
{
    [ObservableProperty]
    public partial DirectUserViewModel Direct { get; set; }

    [ObservableProperty]
    public partial WrappedUserViewModel Wrapped { get; set; }

    [ObservableProperty]
    public partial DelegateUserViewModel Delegate { get; set; }

    public ModelNotificationViewModel(
        DirectUserViewModel direct,
        WrappedUserViewModel wrapped,
        DelegateUserViewModel @delegate)
    {
        Direct = direct;
        Wrapped = wrapped;
        Delegate = @delegate;
    }
}
