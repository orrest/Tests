using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ModelNotification;

/// <summary>
/// 组合根 ViewModel。暴露顶层 <see cref="StepViewModel"/>，View 通过
/// <c>{Binding Step.Name}</c>、<c>{Binding Step.Action.Option.Value}</c> 等路径访问三层嵌套数据。
/// </summary>
public partial class ModelNotificationViewModel : ObservableObject
{
    [ObservableProperty]
    public partial StepViewModel Step { get; set; }

    public ModelNotificationViewModel(StepViewModel step)
    {
        Step = step;
    }
}
