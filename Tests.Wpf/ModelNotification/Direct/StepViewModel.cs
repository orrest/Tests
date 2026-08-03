using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.ModelNotification.Direct.Models;
using Tests.Wpf.ModelNotification.Helpers;

namespace Tests.Wpf.ModelNotification.Direct;

/// <summary>
/// Top-layer ViewModel. Exposes the underlying <see cref="StepModel"/> directly
/// and holds an <see cref="Actions"/> collection of <see cref="ActionViewModel"/>.
/// No proxy properties — the View binds through <c>Model.Name/Enabled</c>.
/// 顶层 ViewModel，直接暴露 <see cref="StepModel"/>，并持有 <see cref="ActionViewModel"/> 集合。
/// 无代理属性——View 通过 <c>Model.Name/Enabled</c> 绑定。
/// <para>
/// <b>订阅链路</b>（逐层向上）：
/// OptionModel 属性变 → ActionVm.SubscribeCollection(selector) 感知 → ActionVm OnPropertyChanged(Summary)
/// → StepVm.SubscribeCollection(identity) 感知 → StepVm OnPropertyChanged(Summary)。
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

    // ===== 派生属性 =====

    /// <summary>聚合 Step.Model.Name/Enabled + 所有 ActionVm.Summary（其内含 Option 聚合）。</summary>
    public string Summary =>
        $"Step[{Model.Name}] ({(Model.Enabled ? "On" : "Off")}, {Actions.Count} actions: " +
        $"{string.Join(" | ", Actions.Select(a => a.Summary).Take(3))})";

    /// <summary>显示当前 StepModel 的 HashCode。</summary>
    public string StepId => $"#{Model.GetHashCode():X}";

    /// <summary>
    /// 聚合所有 Action 下所有 Option 的 Name/Value。
    /// 直接依赖 OptionViewModel 的 Model 属性，通过 BindAllOptions 监听其变更。
    /// </summary>
    public string AllOptionsSummary =>
        $"AllOptions: {string.Join("; ", Actions.SelectMany(a => a.Options).Select(o => $"{o.Model.Name}={o.Model.Value}, Enabled={o.Model.Enabled}"))}";

    // ===== 订阅管理 =====

    /// <summary>监听自身 Model.Name/Enabled 变化 → 反射触发 Summary 通知。</summary>
    private void BindStep()
    {
        _stepSub?.Dispose();
        _stepSub = PropertyChangeTracker.Subscribe(
            Model,
            [nameof(StepModel.Name), nameof(StepModel.Enabled)],
            this,
            [nameof(Summary)]);
    }

    /// <summary>
    /// 监听 Actions 集合中所有 ActionVm 的 Summary 变化 + 集合增删 → 反射触发 Summary 通知。
    /// SubscribeCollection 自动管理元素订阅（Add/Remove/Replace/Reset）。
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
    /// 直接监听所有 Action 下每个 OptionViewModel.Model（OptionModel）的 Name/Value/Enabled 变化，
    /// 以及 Actions 集合的增删变动 → 反射触发 AllOptionsSummary 通知。
    /// <para>
    /// 不同于 BindActions 的逐层通知（Option→Action→Step Summary），此方法直连 Option 层，
    /// 确保 AllOptionsSummary 在 Option 属性变更时即时刷新，无需依赖逐层传递。
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
                (OptionViewModel ovm) => ovm.Model,
                [nameof(OptionModel.Name), nameof(OptionModel.Value), nameof(OptionModel.Enabled)],
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
    /// 从后台线程修改第一个 ActionVm 的第一个 OptionVm 的 Model.Value（用 Bogus 随机生成），验证三层通知链路。
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
            Application.Current.Dispatcher.Invoke(() => Actions[0].Options[0].Model.Value = newValue);
        });
    }
}


