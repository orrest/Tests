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
/// <para>
/// <b>跨兄弟 VM 订阅演示</b>（见 <see cref="Sensor"/> / <see cref="Echo"/> 与 <see cref="BindSensor"/>）：
/// SensorVm.Level 变 → StepVm 用反射重载 Subscribe 监听 SensorVm → 反射委托直接调用 EchoVm.OnPropertyChanged(DisplayText)
/// → EchoVm 的派生属性通知 View。父 VM 充当编排者，把两个平级兄弟 VM 间的依赖装配起来，EchoVm 自身无需 Refresh 方法。
/// Cross-sibling VM subscription demo: SensorVm.Level change is observed by StepVm via the reflection
/// Subscribe overload, whose compiled delegate invokes EchoVm.OnPropertyChanged(DisplayText) directly,
/// so EchoVm needs no Refresh method. The parent VM orchestrates the dependency between two sibling VMs
/// that know nothing of each other's lifecycle.
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

    /// <summary>
    /// Sibling vmA for the cross-VM subscription demo. Its <see cref="SensorViewModel.Level"/> changes
    /// are observed via <see cref="BindSensor"/> and forwarded to <see cref="Echo"/>.
    /// 跨 VM 订阅演示用的兄弟 vmA；其 <see cref="SensorViewModel.Level"/> 变化经
    /// <see cref="BindSensor"/> 监听并转发给 <see cref="Echo"/>。
    /// </summary>
    public SensorViewModel Sensor { get; }

    /// <summary>
    /// Sibling vmB whose <see cref="EchoViewModel.DisplayText"/> is refreshed whenever <see cref="Sensor"/> changes.
    /// EchoVm itself never subscribes; the parent VM wires the subscription in <see cref="BindSensor"/>.
    /// 兄弟 vmB，每当 <see cref="Sensor"/> 变化时刷新其 <see cref="EchoViewModel.DisplayText"/>。
    /// EchoVm 自身不订阅；订阅由父 VM 在 <see cref="BindSensor"/> 中装配。
    /// </summary>
    public EchoViewModel Echo { get; }

    private PropertyChangeTracker.ModelSubscription? _sensorSub;

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    public StepViewModel()
    {
        Model = StepModel.CreateRandom();
        Actions = [new ActionViewModel(Model.Action)];
        Sensor = new SensorViewModel();
        Echo = new EchoViewModel(Sensor);
        BindStep();
        BindActions();
        BindAllOptions();
        BindSensor();
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

    /// <summary>监听自身（VM）Name/Enabled 变化 → 反射触发 Summary 通知。代理属性 setter 已在 VM 上引发通知。</summary>
    private void BindStep()
    {
        _stepSub?.Dispose();
        _stepSub = PropertyChangeTracker.Subscribe(
            this,
            [nameof(Name), nameof(Enabled)],
            this,
            [nameof(Summary)]);
    }

    /// <summary>
    /// 监听 Actions 集合中所有 ActionVm 的 Summary 变化 + 集合增删 → 反射触发 Summary 通知。
    /// Summary 是 ActionVm 属性，ActionVm 是 ObservableObject，可直接订阅——与 Direct 版一致。
    /// </summary>
    private void BindActions()
    {
        _actionsColSub?.Dispose();
        _actionsColSub = PropertyChangeTracker.SubscribeCollection(
            Actions,
            (ActionViewModel e) => e,
            [nameof(ActionViewModel.Summary)],
            this,
            [nameof(Summary)]);
    }

    /// <summary>
    /// 直接监听所有 Action 下每个 OptionVm <b>自身</b>的 Name/Value/Enabled 变化（selector 为 identity，
    /// 而非 ovm.Model），以及 Actions 集合的增删变动 → 反射触发 AllOptionsSummary 通知。
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
                (OptionViewModel e) => e,
                [nameof(OptionViewModel.Name), nameof(OptionViewModel.Value), nameof(OptionViewModel.Enabled)],
                this,
                [nameof(AllOptionsSummary)]);
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

    /// <summary>
    /// Wire the cross-sibling-VM subscription: observe <see cref="Sensor"/>.<see cref="SensorViewModel.Level"/>
    /// via the reflection overload <see cref="PropertyChangeTracker.Subscribe{TSource, TTarget}"/>, which
    /// directly invokes <c>OnPropertyChanged(DisplayText)</c> on <see cref="Echo"/> — EchoVm needs no
    /// <c>Refresh</c> method. The reflection delegate is compiled once at subscription time, so per-callback
    /// cost is close to a direct method call.
    /// 装配跨兄弟 VM 订阅：通过反射重载 <see cref="PropertyChangeTracker.Subscribe{TSource, TTarget}"/>
    /// 监听 <see cref="Sensor"/>.<see cref="SensorViewModel.Level"/>，直接在 <see cref="Echo"/> 上调用
    /// <c>OnPropertyChanged(DisplayText)</c> —— EchoVm 无需任何 <c>Refresh</c> 方法。
    /// 反射委托在订阅时一次性编译，单次回调开销接近直接方法调用。
    /// <para>
    /// Pattern note: same shape as <see cref="BindStep"/> (single-source Subscribe), but uses the reflection
    /// overload to bypass the <c>protected</c> accessibility of <c>OnPropertyChanged</c> on EchoVm.
    /// 模式提示：与 <see cref="BindStep"/>（单源 Subscribe）形态相同，但使用反射重载绕过 EchoVm 上
    /// <c>OnPropertyChanged</c> 的 <c>protected</c> 可见性限制。
    /// </para>
    /// </summary>
    private void BindSensor()
    {
        _sensorSub?.Dispose();
        _sensorSub = PropertyChangeTracker.Subscribe(
            Sensor,
            [nameof(SensorViewModel.Level)],
            Echo,
            [nameof(EchoViewModel.DisplayText)]);
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
