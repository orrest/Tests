using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.ModelNotification.Direct.Models;
using Tests.Wpf.ModelNotification.Helpers;

namespace Tests.Wpf.ModelNotification.Direct;

/// <summary>
/// Middle-layer ViewModel. Exposes the underlying <see cref="ActionModel"/> directly
/// and holds an <see cref="Options"/> collection of <see cref="OptionViewModel"/>.
/// No proxy properties — the View binds through <c>Model.Name/Enabled/DelayMs</c>.
/// 中层 ViewModel，直接暴露 <see cref="ActionModel"/>，并持有 <see cref="OptionViewModel"/> 集合。
/// 无代理属性——View 通过 <c>Model.Name/Enabled/DelayMs</c> 绑定。
/// <para>
/// <b>订阅链路</b>：
/// <list type="bullet">
/// <item><see cref="Subscribe{TSource}"/> 监听自身 Model.Name/Enabled 变化 → OnPropertyChanged(nameof(Summary))</item>
/// <item><see cref="SubscribeCollection{TElement, TNested}(ObservableCollection{TElement}, Func{TElement, TNested}, IEnumerable{string}, Action)"/>
/// 监听 Options 中每个 OptionVm 的 <c>Model</c>（OptionModel）的 Name/Value 变化 + 集合增删 → OnPropertyChanged(nameof(Summary))</item>
/// </list>
/// </para>
/// </summary>
public partial class ActionViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ActionModel Model { get; private set; }

    /// <summary>Holds the下层 VM 集合. View binds ItemsControl to this.</summary>
    public ObservableCollection<OptionViewModel> Options { get; }

    private PropertyChangeTracker.ModelSubscription? _actionSub;
    private PropertyChangeTracker.ModelSubscription? _optionsColSub;

    public ActionViewModel()
    {
        Model = ActionModel.CreateRandom();
        Options = [new OptionViewModel(Model.Option)];
        BindAction();
        BindOptions();
    }

    /// <summary>Construct with an existing Model (used by parent when creating VM collection).</summary>
    internal ActionViewModel(ActionModel model)
    {
        Model = model;
        Options = [new OptionViewModel(model.Option)];
        BindAction();
        BindOptions();
    }

    // ===== 派生属性 =====

    /// <summary>聚合 Action.Model.Name/Enabled + 所有 OptionVm.Model.Name。</summary>
    public string Summary =>
        $"{Model.Name}({(Model.Enabled ? "On" : "Off")}, {Options.Count} opts: {string.Join(", ", Options.Select(o => o.Model.Name))})";

    // ===== 订阅管理 =====

    /// <summary>监听自身 Model.Name/Enabled 变化 → 反射触发 Summary 通知。</summary>
    private void BindAction()
    {
        _actionSub?.Dispose();
        _actionSub = PropertyChangeTracker.Subscribe(
            Model,
            [nameof(ActionModel.Name), nameof(ActionModel.Enabled)],
            this,
            [nameof(Summary)]);
    }

    /// <summary>
    /// 监听 Options 集合中每个 OptionVm 的 <c>Model</c>（OptionModel）的 Name/Value 变化 + 集合增删 → 反射触发 Summary 通知。
    /// selector <c>ovm => ovm.Model</c> 提取出 OptionModel 实例直接订阅其属性变化，无需代理属性中转。
    /// </summary>
    private void BindOptions()
    {
        _optionsColSub?.Dispose();
        _optionsColSub = PropertyChangeTracker.SubscribeCollection(
            Options,
            (OptionViewModel ovm) => ovm.Model,
            [nameof(OptionModel.Name), nameof(OptionModel.Value)],
            this,
            [nameof(Summary)]);
    }

    // ===== 命令 =====

    /// <summary>Add a new random OptionVm; SubscribeCollection auto-subscribes via CollectionChanged.</summary>
    [RelayCommand]
    private void AddOption()
    {
        Options.Add(new OptionViewModel());
    }

    /// <summary>Remove the first OptionVm; SubscribeCollection auto-unsubscribes via CollectionChanged.</summary>
    [RelayCommand]
    private void RemoveOption()
    {
        if (Options.Count > 0)
        {
            Options.RemoveAt(0);
        }
    }
}
