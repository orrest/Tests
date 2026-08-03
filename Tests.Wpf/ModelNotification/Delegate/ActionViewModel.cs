using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.ModelNotification.Delegate.Models;
using Tests.Wpf.ModelNotification.Helpers;

namespace Tests.Wpf.ModelNotification.Delegate;

/// <summary>
/// Middle-layer ViewModel with <b>proxy properties</b>. Wraps a POCO <see cref="Models.ActionModel"/>
/// and holds an <see cref="Options"/> collection of <see cref="OptionViewModel"/>.
/// Each scalar property proxies the POCO; nested OptionModel is surfaced as a child
/// <see cref="OptionViewModel"/> rather than a raw Model so that the View binds to VM properties only.
/// 采用<b>代理属性</b>的中层 ViewModel，包裹 POCO <see cref="Models.ActionModel"/>，
/// 并持有 <see cref="OptionViewModel"/> 集合。每个标量属性代理 POCO；
/// 嵌套的 OptionModel 以子 <see cref="OptionViewModel"/> 形式暴露而非原始 Model，使 View 只绑定 VM 属性。
/// <para>
/// <b>订阅链路（与 Direct 版的关键差异）</b>：
/// <list type="bullet">
/// <item><see cref="Subscribe{TSource}"/> 监听<b>自身</b> Name/Enabled 变化（VM 自身引发，非 Model）→ OnPropertyChanged(nameof(Summary))</item>
/// <item><see cref="SubscribeCollection{TSource}"/> 用 <b>identity</b> 重载直接监听 Options 中每个 OptionVm <b>自身</b>
/// 的 Name/Value 变化（而非 ovm.Model） + 集合增删 → OnPropertyChanged(nameof(Summary))</item>
/// </list>
/// 因为 OptionVm 是 ObservableObject 且经代理属性引发通知，所以无需监听 POCO Model。
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

    // ===== 代理属性：读取自 POCO，写入同时更新 POCO 并在 VM 自身引发通知 =====

    /// <summary>Proxy for <see cref="Models.ActionModel.Name"/>. Mutate via this, not via Model.</summary>
    public string Name
    {
        get => Model.Name;
        set
        {
            if (Model.Name != value)
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>Proxy for <see cref="Models.ActionModel.Enabled"/>. Mutate via this, not via Model.</summary>
    public bool Enabled
    {
        get => Model.Enabled;
        set
        {
            if (Model.Enabled != value)
            {
                Model.Enabled = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>Proxy for <see cref="Models.ActionModel.DelayMs"/>. Mutate via this, not via Model.</summary>
    public int DelayMs
    {
        get => Model.DelayMs;
        set
        {
            if (Model.DelayMs != value)
            {
                Model.DelayMs = value;
                OnPropertyChanged();
            }
        }
    }

    // ===== 派生属性（读取 VM 代理属性，而非 Model） =====

    /// <summary>聚合 ActionVm.Name/Enabled + 所有 OptionVm.Name（全部经代理属性）。</summary>
    public string Summary =>
        $"{Name}({(Enabled ? "On" : "Off")}, {Options.Count} opts: {string.Join(", ", Options.Select(o => o.Name))})";

    // ===== 订阅管理 =====

    /// <summary>监听自身（VM）Name/Enabled 变化 → 反射触发 Summary 通知。代理属性 setter 已在 VM 上引发通知。</summary>
    private void BindAction()
    {
        _actionSub?.Dispose();
        _actionSub = PropertyChangeTracker.Subscribe(
            this,
            [nameof(Name), nameof(Enabled)],
            this,
            [nameof(Summary)]);
    }

    /// <summary>
    /// 监听 Options 集合中每个 OptionVm <b>自身</b>的 Name/Value 变化 + 集合增删 → 反射触发 Summary 通知。
    /// selector 为 identity（<c>e => e</c>）直接订阅 OptionViewModel，而非 ovm.Model——因为 OptionVm 是
    /// ObservableObject 且经代理属性引发通知，无需也无法监听 POCO Model。
    /// </summary>
    private void BindOptions()
    {
        _optionsColSub?.Dispose();
        _optionsColSub = PropertyChangeTracker.SubscribeCollection(
            Options,
            (OptionViewModel e) => e,
            [nameof(OptionViewModel.Name), nameof(OptionViewModel.Value)],
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
