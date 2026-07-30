using CommunityToolkit.Mvvm.ComponentModel;
using Tests.Wpf.ModelNotification.Delegate.Models;

namespace Tests.Wpf.ModelNotification.Delegate;

/// <summary>
/// Innermost ViewModel with <b>proxy properties</b>. The underlying <see cref="Models.OptionModel"/>
/// is a plain POCO and never raises change notifications — every property read goes through the VM
/// getter, and every write goes through the VM setter which updates the POCO <i>and</i> raises
/// <see cref="ObservableObject.OnPropertyChanged(string?)"/> on the VM itself.
/// 采用<b>代理属性</b>的最内层 ViewModel。底层 <see cref="Models.OptionModel"/> 是纯 POCO，
/// 永不引发变化通知——读取经 VM getter，写入经 VM setter：setter 既更新 POCO，
/// 又在 VM 自身上引发 <see cref="ObservableObject.OnPropertyChanged(string?)"/>。
/// <para>
/// Proxy properties cannot use <c>[ObservableProperty]</c> (getter/setter must delegate to the Model),
/// so they are written by hand — this is the essence of the delegate pattern.
/// 代理属性无法使用 <c>[ObservableProperty]</c>（getter/setter 需委托到 Model），故手写——这是代理模式的本质。
/// </para>
/// <para>
/// Run-time mutations <b>must</b> go through the VM (e.g. <c>optionVm.Name = ...</c>); mutating the
/// POCO directly (optionVm.Model.Name) would change the data but raise no notification, so neither
/// the View nor parent VMs would react.
/// 运行时修改<b>必须</b>经 VM（如 <c>optionVm.Name = ...</c>）；直接改 POCO（optionVm.Model.Name）
/// 会改数据但不引发通知，View 与上层 VM 都不会响应。
/// </para>
/// </summary>
public partial class OptionViewModel : ObservableObject
{
    /// <summary>Wrapped POCO Model. Kept public for data read-back, but should not be mutated directly.</summary>
    public OptionModel Model { get; private set; }

    public OptionViewModel()
    {
        Model = OptionModel.CreateRandom();
    }

    /// <summary>Construct with an existing Model (used by parent when creating VM collection).</summary>
    internal OptionViewModel(OptionModel model)
    {
        Model = model;
    }

    // ===== 代理属性：读取自 POCO，写入同时更新 POCO 并在 VM 自身引发通知 =====

    /// <summary>Proxy for <see cref="Models.OptionModel.Name"/>. Set raises change on the VM, not the POCO.</summary>
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

    /// <summary>Proxy for <see cref="Models.OptionModel.Value"/>. Set raises change on the VM, not the POCO.</summary>
    public string Value
    {
        get => Model.Value;
        set
        {
            if (Model.Value != value)
            {
                Model.Value = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>Proxy for <see cref="Models.OptionModel.Enabled"/>. Set raises change on the VM, not the POCO.</summary>
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
}
