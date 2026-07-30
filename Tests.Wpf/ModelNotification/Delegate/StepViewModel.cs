using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.ModelNotification.Delegate.Models;
using Tests.Wpf.ModelNotification.Helpers;

namespace Tests.Wpf.ModelNotification.Delegate;

/// <summary>
/// Top-layer ViewModel with <b>proxy properties</b>. Wraps a POCO <see cref="Models.StepModel"/>
/// and holds an <see cref="Actions"/> collection of <see cref="ActionViewModel"/>.
/// View binds through VM proxy properties (Name/Enabled), never through the POCO Model.
/// 采用<b>代理属性</b>的顶层 ViewModel，包裹 POCO <see cref="Models.StepModel"/>，
/// 并持有 <see cref="ActionViewModel"/> 集合。View 只经 VM 代理属性（Name/Enabled）绑定，绝不经 POCO Model。
/// <para>
/// <b>订阅链路（与 Direct 版的关键差异，逐层向上）</b>：
/// OptionVm 代理属性变 → ActionVm.SubscribeCollection(identity) 监听 OptionVm 自身 → ActionVm OnPropertyChanged(Summary)
/// → StepVm.SubscribeCollection 监听 ActionVm.Summary → StepVm OnPropertyChanged(Summary)。
/// 全程订阅的是<b>下层 VM</b>（ObservableObject），而非下层 POCO Model（无法被监听）。
/// </para>
/// </summary>
public partial class StepViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StepId))]
    public partial StepModel Model { get; private set; }

    /// <summary>Holds the下层 VM 集合. View binds ItemsControl to this.</summary>
    public ObservableCollection<ActionViewModel> Actions { get; }

    private PropertyChangeTracker.ModelSubscription? _stepSub;
    private PropertyChangeTracker.ModelSubscription? _actionsColSub;
    private PropertyChangeTracker.ModelSubscription? _allOptionsSub;

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    public StepViewModel()
    {
        Model = StepModel.CreateRandom();
        Actions = [new ActionViewModel(Model.Action)];
        BindStep();
        BindActions();
        BindAllOptions();
    }

    // ===== 代理属性：读取自 POCO，写入同时更新 POCO 并在 VM 自身引发通知 =====

    /// <summary>Proxy for <see cref="Models.StepModel.Name"/>. Mutate via this, not via Model.</summary>
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

    /// <summary>Proxy for <see cref="Models.StepModel.Enabled"/>. Mutate via this, not via Model.</summary>
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

    // ===== 派生属性（读取 VM 代理属性，而非 Model） =====

    /// <summary>聚合 StepVm.Name/Enabled + 所有 ActionVm.Summary（其内含 Option 聚合）。</summary>
    public string Summary =>
        $"Step[{Name}] ({(Enabled ? "On" : "Off")}, {Actions.Count} actions: " +
        $"{string.Join(" | ", Actions.Select(a => a.Summary).Take(3))})";

    /// <summary>显示当前 StepModel 的 HashCode。</summary>
    public string StepId => $"#{Model.GetHashCode():X}";

    /// <summary>
    /// 聚合所有 Action 下所有 Option 的 Name/Value（全部经 OptionVm 代理属性读取）。
    /// 通过 BindAllOptions 监听 OptionVm 自身的属性变更。
    /// </summary>
    public string AllOptionsSummary =>
        $"AllOptions: {string.Join("; ", Actions.SelectMany(a => a.Options).Select(o => $"{o.Name}={o.Value}, Enabled={o.Enabled}"))}";

    // ===== 订阅管理 =====

    /// <summary>监听自身（VM）Name/Enabled 变化 → 刷新 Summary。代理属性 setter 已在 VM 上引发通知。</summary>
    private void BindStep()
    {
        _stepSub?.Dispose();
        _stepSub = PropertyChangeTracker.Subscribe(
            this,
            [nameof(Name), nameof(Enabled)],
            _ => OnPropertyChanged(nameof(Summary)));
    }

    /// <summary>
    /// 监听 Actions 集合中所有 ActionVm 的 Summary 变化 + 集合增删 → 刷新 Summary。
    /// Summary 是 ActionVm 属性，ActionVm 是 ObservableObject，可直接订阅——与 Direct 版一致。
    /// </summary>
    private void BindActions()
    {
        _actionsColSub?.Dispose();
        _actionsColSub = PropertyChangeTracker.SubscribeCollection(
            Actions,
            [nameof(ActionViewModel.Summary)],
            () => OnPropertyChanged(nameof(Summary)));
    }

    /// <summary>
    /// 直接监听所有 Action 下每个 OptionVm <b>自身</b>的 Name/Value/Enabled 变化（用 identity 重载，
    /// 而非 ovm.Model），以及 Actions 集合的增删变动 → 刷新 AllOptionsSummary。
    /// <para>
    /// 不同于 BindActions 的逐层通知（Option→Action→Step Summary），此方法直连 OptionVm 层，
    /// 确保 AllOptionsSummary 在 OptionVm 代理属性变更时即时刷新，无需依赖逐层传递。
    /// </para>
    /// </summary>
    private void BindAllOptions()
    {
        _allOptionsSub?.Dispose();

        var innerSubs = new List<IDisposable>();
        foreach (var action in Actions)
        {
            var sub = PropertyChangeTracker.SubscribeCollection(
                action.Options,
                [nameof(OptionViewModel.Name), nameof(OptionViewModel.Value), nameof(OptionViewModel.Enabled)],
                () => OnPropertyChanged(nameof(AllOptionsSummary)));
            if (sub is not null)
                innerSubs.Add(sub);
        }

        // Actions 集合变动时重建所有 Option 订阅
        NotifyCollectionChangedEventHandler onActionsChanged = (_, _) =>
        {
            BindAllOptions();
            OnPropertyChanged(nameof(AllOptionsSummary));
        };
        Actions.CollectionChanged += onActionsChanged;

        _allOptionsSub = new PropertyChangeTracker.ModelSubscription(() =>
        {
            foreach (var sub in innerSubs)
                sub.Dispose();
            Actions.CollectionChanged -= onActionsChanged;
        });
    }

    // ===== 命令 =====

    /// <summary>
    /// Add a new random ActionVm; SubscribeCollection auto-subscribes via CollectionChanged.
    /// 新增随机 ActionVm；通过 SubscribeCollection 自动订阅。
    /// </summary>
    [RelayCommand]
    private void AddAction()
    {
        Actions.Add(new ActionViewModel());
    }

    /// <summary>
    /// Remove the first ActionVm; SubscribeCollection auto-unsubscribes via CollectionChanged.
    /// 移除第一个 ActionVm；通过 SubscribeCollection 自动解绑。
    /// </summary>
    [RelayCommand]
    private void RemoveAction()
    {
        if (Actions.Count > 0)
        {
            Actions.RemoveAt(0);
        }
    }

    /// <summary>
    /// Replace the whole StepModel + rebuild Actions collection (with one initial ActionVm).
    /// 替换 StepModel + 重建 Actions 集合（含一个初始 ActionVm）。
    /// </summary>
    [RelayCommand]
    private void ReplaceStep()
    {
        // 清空 Actions（触发 SubscribeCollection Reset → 自动解绑所有元素订阅 + 重建）
        Actions.Clear();
        Model = StepModel.CreateRandom();
        BindStep();
        Actions.Add(new ActionViewModel(Model.Action));
        BindAllOptions();
    }

    /// <summary>
    /// 从后台线程修改第一个 ActionVm 的第一个 OptionVm 的 <b>VM 代理属性</b> Value（用 Bogus 随机生成），验证三层通知链路。
    /// 注意：必须经 VM（optionVm.Value）修改，不能直接改 POCO（optionVm.Model.Value 会改数据但不引发通知）。
    /// WPF 绑定的属性变化通知必须在 UI 线程触发，因此用 <see cref="Dispatcher.Invoke"/> 切回 UI 线程。
    /// </summary>
    [RelayCommand]
    private async Task ModifyFromBackground()
    {
        if (Actions.Count == 0 || Actions[0].Options.Count == 0)
        {
            return;
        }

        string newValue = Faker.Hacker.Adjective();
        await Task.Run(() =>
        {
            Application.Current.Dispatcher.Invoke(() => Actions[0].Options[0].Value = newValue);
        });
    }
}
