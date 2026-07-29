using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.ModelNotification.Models;

namespace Tests.Wpf.ModelNotification;

/// <summary>
/// Middle-layer ViewModel of the three-layer nested scenario. Exposes <see cref="ActionModel"/> and the
/// nested <see cref="OptionModel"/> directly; View binds deep paths (<c>ActionModel.Name</c>, <c>OptionModel.Value</c>).
/// 三层嵌套场景的中层 ViewModel，直接暴露 <see cref="ActionModel"/> 与内层 <see cref="OptionModel"/>，
/// View 绑定深链路 <c>ActionModel.Name</c>、<c>OptionModel.Value</c> 等。
/// <para>
/// 无代理属性、无派生属性、无跨层订阅——本 VM 仅负责持有 Model 并提供替换命令，与 <see cref="OptionViewModel"/> 结构对称。
/// 跨层聚合派生统一由顶层 <see cref="StepViewModel.Summary"/> 承担。
/// </para>
/// </summary>
public partial class ActionViewModel : ObservableObject
{
    /// <summary>
    /// Wrapped Model, exposed as ObservableProperty so replacement auto-notifies View + parent VM.
    /// 包装的 Model，作为 ObservableProperty 暴露：替换时自动通知 View 和上层 VM。
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OptionModel))]
    public partial ActionModel Model { get; set; }

    /// <summary>
    /// The nested OptionModel, kept in sync with <see cref="ActionModel.Option"/>. Exposed directly for View binding.
    /// 嵌套的 OptionModel，与 <see cref="ActionModel.Option"/> 保持同步；直接暴露供 View 深链路绑定。
    /// </summary>
    [ObservableProperty]
    public partial OptionModel OptionModel { get; set; }

    public ActionViewModel()
    {
        Model = ActionModel.CreateRandom();
        OptionModel = Model.Option;
    }

    /// <summary>Construct with an existing Model (used when parent replaces nested instances).</summary>
    internal ActionViewModel(ActionModel model)
    {
        Model = model;
        OptionModel = model.Option;
    }

    /// <summary>
    /// Replace the nested OptionModel with a random one; sync ActionModel.Option.
    /// 替换 OptionModel：同步 ActionModel.Option 引用，ObservableProperty 自动通知 View。
    /// </summary>
    [RelayCommand]
    private void ReplaceOption()
    {
        Model.Option = OptionModel.CreateRandom();
        OptionModel = Model.Option;
    }
}
