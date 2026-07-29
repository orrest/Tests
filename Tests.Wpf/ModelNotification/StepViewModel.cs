using System.Windows;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.ModelNotification.Helpers;
using Tests.Wpf.ModelNotification.Models;

namespace Tests.Wpf.ModelNotification;

/// <summary>
/// Top-layer ViewModel of the three-layer nested scenario. Exposes <see cref="StepModel"/> and nested
/// <see cref="ActionModel"/> / <see cref="OptionModel"/> directly; View binds deep paths.
/// Demonstrates ModelBinder cross-layer aggregation: <see cref="Summary"/> depends on properties
/// spanning Step / Action / Option models (three layers), all subscribed via ForwardDerived.
/// 三层嵌套场景的顶层 ViewModel，直接暴露 <see cref="StepModel"/> 及内层 <see cref="ActionModel"/> / <see cref="OptionModel"/>。
/// 演示 ModelBinder 跨层聚合派生：<see cref="Summary"/> 依赖跨越 Step / Action / Option 三层 Model 的属性，
/// 全部通过 ForwardDerived 订阅 Model（不订阅下层 VM）。
/// <para>
/// <b>通知链路</b>：OptionModel.Name 变 → ActionVm.ForwardDerived 监听到 → ActionVm.Summary 刷新
/// → StepVm.ForwardDerived 监听到 ActionModel → StepVm.Summary 刷新。逐层向上，全链路订阅 Model。
/// </para>
/// </summary>
public partial class StepViewModel : ObservableObject
{
    /// <summary>
    /// Wrapped Model, exposed as ObservableProperty so replacement auto-notifies View + refreshes derived.
    /// 包装的 Model，作为 ObservableProperty 暴露：替换时自动通知 View 并刷新派生属性。
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    [NotifyPropertyChangedFor(nameof(StepId))]
    [NotifyPropertyChangedFor(nameof(ActionModel))]
    [NotifyPropertyChangedFor(nameof(OptionModel))]
    public partial StepModel Model { get; set; }

    /// <summary>Nested ActionModel, kept in sync with <see cref="StepModel.Action"/>. Exposed for View binding.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    [NotifyPropertyChangedFor(nameof(ActionId))]
    [NotifyPropertyChangedFor(nameof(OptionModel))]
    public partial ActionModel ActionModel { get; set; }

    /// <summary>Deepest OptionModel, kept in sync with <see cref="ActionModel.Option"/>. Exposed for View binding.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    [NotifyPropertyChangedFor(nameof(OptionId))]
    public partial OptionModel OptionModel { get; set; }

    private ModelBinder.ModelSubscription? _stepSub;
    private ModelBinder.ModelSubscription? _actionSub;
    private ModelBinder.ModelSubscription? _optionSub;

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    public StepViewModel()
    {
        Model = StepModel.CreateRandom();
        ActionModel = Model.Action;
        OptionModel = ActionModel.Option;
        BindStep(Model);
        BindAction(ActionModel);
        BindOption(OptionModel);
    }

    /// <summary>Auto-rebind ActionModel subscription when replaced.</summary>
    partial void OnActionModelChanged(ActionModel value)
    {
        BindAction(value);
    }

    /// <summary>Auto-rebind OptionModel subscription when replaced.</summary>
    partial void OnOptionModelChanged(OptionModel value)
    {
        BindOption(value);
    }

    // ===== 派生 / 计算属性 =====

    /// <summary>
    /// 顶层聚合派生：依赖跨越三层的属性。
    /// StepModel.Name/Enabled + ActionModel.Name + OptionModel.Name/Value。
    /// </summary>
    public string Summary =>
        $"Step[{Model.Name}] ({(Model.Enabled ? "On" : "Off")}) → {ActionModel.Name} (Opt={OptionModel.Name}={OptionModel.Value})";

    /// <summary>显示当前 StepModel 的 HashCode。</summary>
    public string StepId => $"#{Model.GetHashCode():X}";

    /// <summary>显示当前 ActionModel 的 HashCode。</summary>
    public string ActionId => $"#{ActionModel.GetHashCode():X}";

    /// <summary>显示当前 OptionModel 的 HashCode。</summary>
    public string OptionId => $"#{OptionModel.GetHashCode():X}";

    // ===== 用通用转发器建立 Model → VM 派生属性 的通知映射 =====

    /// <summary>建立 StepModel 属性 → VM 派生属性（Summary）的映射。</summary>
    private void BindStep(StepModel step)
    {
        _stepSub?.Dispose();
        _stepSub = ModelBinder.ForwardDerived(
            this,
            step,
            (m => m.Name, [() => Summary]),
            (m => m.Enabled, [() => Summary]));
    }

    /// <summary>建立 ActionModel 属性 → VM 派生属性（Summary）的映射。</summary>
    private void BindAction(ActionModel action)
    {
        _actionSub?.Dispose();
        _actionSub = ModelBinder.ForwardDerived(
            this,
            action,
            (m => m.Name, [() => Summary]),
            (m => m.Enabled, [() => Summary]));
    }

    /// <summary>建立 OptionModel 属性 → VM 派生属性（Summary）的映射。</summary>
    private void BindOption(OptionModel option)
    {
        _optionSub?.Dispose();
        _optionSub = ModelBinder.ForwardDerived(
            this,
            option,
            (m => m.Name, [() => Summary]),
            (m => m.Value, [() => Summary]));
    }

    // ===== 命令：替换各层实例（演示订阅迁移） =====

    /// <summary>
    /// Replace the whole Step with a random one; sync ActionModel/OptionModel + auto-rebind subscriptions.
    /// 替换 Step：同步内层 Model 引用，ObservableProperty 自动通知 View + 自动重订阅 + 刷新派生。
    /// </summary>
    [RelayCommand]
    private void ReplaceStep()
    {
        Model = StepModel.CreateRandom();
        ActionModel = Model.Action;        // 触发 OnActionModelChanged 重订阅
        OptionModel = ActionModel.Option;  // 触发 OnOptionModelChanged 重订阅
    }

    /// <summary>
    /// Replace the nested ActionModel with a random one; sync OptionModel + auto-rebind subscriptions.
    /// 替换 Action：同步 OptionModel 引用，自动重订阅。
    /// </summary>
    [RelayCommand]
    private void ReplaceAction()
    {
        Model.Action = ActionModel.CreateRandom();
        ActionModel = Model.Action;        // 触发 OnActionModelChanged 重订阅
        OptionModel = ActionModel.Option;  // 触发 OnOptionModelChanged 重订阅
    }

    /// <summary>
    /// 从后台线程修改最内层 OptionModel.Value（用 Bogus 随机生成新值），验证三层通知链路。
    /// WPF 绑定的属性变化通知必须在 UI 线程触发，因此用 <see cref="Dispatcher.Invoke"/> 切回 UI 线程。
    /// </summary>
    [RelayCommand]
    private async Task ModifyFromBackground()
    {
        string newValue = Faker.Hacker.Adjective();
        await Task.Run(() =>
        {
            Application.Current.Dispatcher.Invoke(() => OptionModel.Value = newValue);
        });
    }
}
