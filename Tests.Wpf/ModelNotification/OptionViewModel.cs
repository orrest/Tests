using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.ModelNotification.Models;

namespace Tests.Wpf.ModelNotification;

/// <summary>
/// Innermost ViewModel of the three-layer nested scenario. Exposes the wrapped <see cref="OptionModel"/>
/// directly so the View can bind deep paths (<c>OptionModel.Name</c>, <c>OptionModel.Value</c>, etc.).
/// 三层嵌套场景的最内层 ViewModel，直接暴露 <see cref="OptionModel"/>，
/// View 绑定深链路 <c>OptionModel.Name</c> / <c>OptionModel.Value</c> 等。
/// <para>
/// 无代理属性、无派生属性、无跨层订阅——本 VM 仅负责持有 Model 并提供替换命令。
/// Model 自身实现 INotifyPropertyChanged，编辑即写回，无需 VM 转发。
/// </para>
/// </summary>
public partial class OptionViewModel : ObservableObject
{
    /// <summary>
    /// Wrapped Model, exposed as ObservableProperty so replacement auto-notifies View and parent VMs.
    /// 包装的 Model，作为 ObservableProperty 暴露：替换时自动通知 View 和上层 VM。
    /// </summary>
    [ObservableProperty]
    public partial OptionModel Model { get; set; }

    public OptionViewModel()
    {
        Model = OptionModel.CreateRandom();
    }

    /// <summary>Construct with an existing Model (used when parent replaces nested instances).</summary>
    internal OptionViewModel(OptionModel model)
    {
        Model = model;
    }

    /// <summary>
    /// Replace the inner Model with a random one. ObservableProperty auto-notifies View + parent VM.
    /// 用随机新 Model 替换内部实例；ObservableProperty 自动通知 View 和上层 VM。
    /// </summary>
    [RelayCommand]
    private void ReplaceOption()
    {
        Model = OptionModel.CreateRandom();
    }
}
