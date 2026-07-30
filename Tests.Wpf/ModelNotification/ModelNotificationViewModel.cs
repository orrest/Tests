using CommunityToolkit.Mvvm.ComponentModel;
using Tests.Wpf.ModelNotification.Delegate;
using Tests.Wpf.ModelNotification.Direct;
using DelegateStepViewModel = Tests.Wpf.ModelNotification.Delegate.StepViewModel;
using DirectStepViewModel = Tests.Wpf.ModelNotification.Direct.StepViewModel;

namespace Tests.Wpf.ModelNotification;

/// <summary>
/// 组合根 ViewModel。同时持有两个同名 <c>StepViewModel</c>（Direct / Delegate，分属不同命名空间），
/// 通过 using 别名区分。View 的 TabControl 分别绑定 <see cref="DirectStep"/> 与 <see cref="DelegateStep"/>，
/// 对比两种分层模式。
/// 组合根 ViewModel。同时持有两个同名 <c>StepViewModel</c>（Direct / Delegate，分属不同命名空间），
/// 通过 using 别名区分。View 的 TabControl 分别绑定 <see cref="DirectStep"/> 与 <see cref="DelegateStep"/>，
/// 对比两种分层模式。
/// </summary>
public partial class ModelNotificationViewModel : ObservableObject
{
    /// <summary>
    /// Direct 模式顶层 ViewModel：VM 直接暴露 Model，View 经 <c>Model.Xxx</c> 绑定，通知来自 Model。
    /// </summary>
    [ObservableProperty]
    public partial DirectStepViewModel DirectStep { get; set; }

    /// <summary>
    /// Delegate 模式顶层 ViewModel：Model 为 POCO，VM 通过代理属性暴露，View 经 VM 代理属性绑定，通知来自 VM。
    /// </summary>
    [ObservableProperty]
    public partial DelegateStepViewModel DelegateStep { get; set; }

    public ModelNotificationViewModel(DirectStepViewModel directStep, DelegateStepViewModel delegateStep)
    {
        DirectStep = directStep;
        DelegateStep = delegateStep;
    }
}
