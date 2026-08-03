using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ModelNotification.Delegate;

/// <summary>
/// Sibling VM (vmB) that depends on a <see cref="SensorViewModel"/> (vmA) but does <b>not</b> subscribe itself,
/// and does <b>not</b> expose any <c>Refresh</c> method. The parent VM wires up the subscription via the
/// <see cref="Helpers.PropertyChangeTracker.Subscribe{TSource, TTarget}"/> reflection overload, which directly
/// invokes <c>OnPropertyChanged(DisplayText)</c> on this VM — no extra plumbing on EchoVm itself.
/// 兄弟 VM（vmB），依赖 <see cref="SensorViewModel"/>（vmA）但<b>不</b>自行订阅，也<b>不</b>暴露任何
/// <c>Refresh</c> 方法。父 VM 通过 <see cref="Helpers.PropertyChangeTracker.Subscribe{TSource, TTarget}"/>
/// 反射重载装配订阅，直接在本 VM 上调用 <c>OnPropertyChanged(DisplayText)</c> —— EchoVm 自身无需任何额外管线。
/// <para>
/// <b>Why no Refresh?</b> <c>ObservableObject.OnPropertyChanged</c> is <c>protected</c>, so external code cannot
/// trigger it directly. The reflection overload in <see cref="Helpers.PropertyChangeTracker"/> bypasses this by
/// compiling a delegate at subscription time that calls the protected method on EchoVm's behalf.
/// </para>
/// <para>
/// <b>为何无需 Refresh？</b> <c>ObservableObject.OnPropertyChanged</c> 是 <c>protected</c>，外部代码无法直接触发。
/// <see cref="Helpers.PropertyChangeTracker"/> 的反射重载绕过此限制：订阅时编译委托，代 EchoVm 调用 protected 方法。
/// </para>
/// </summary>
public partial class EchoViewModel : ObservableObject
{
    private readonly SensorViewModel _sensor;

    /// <summary>Construct with the sensor to echo. The sensor reference is retained for read-back only.</summary>
    internal EchoViewModel(SensorViewModel sensor)
    {
        _sensor = sensor;
    }

    /// <summary>
    /// Derived read-only display that reflects the current <see cref="SensorViewModel.Level"/>.
    /// Refreshed externally via the reflection Subscribe overload (which invokes OnPropertyChanged on this VM).
    /// 只读派生显示，反映当前 <see cref="SensorViewModel.Level"/>。
    /// 由反射 Subscribe 重载外部刷新（该重载在本 VM 上调用 OnPropertyChanged）。
    /// </summary>
    public string DisplayText => $"Echo: Sensor.Level = {_sensor.Level}";
}
