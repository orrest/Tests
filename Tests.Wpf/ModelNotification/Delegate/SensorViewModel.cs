using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ModelNotification.Delegate;

/// <summary>
/// Sibling VM (vmA) used to demo cross-VM subscription inside a parent VM.
/// Holds a single scalar <see cref="Level"/> whose setter raises change notification on the VM itself,
/// so a sibling <see cref="EchoViewModel"/> can subscribe to it via <c>PropertyChangeTracker.Subscribe</c>.
/// 兄弟 VM（vmA），用于演示父 VM 内的跨 VM 订阅。
/// 仅持有一个标量 <see cref="Level"/>，其 setter 在 VM 自身引发通知，
/// 使兄弟 <see cref="EchoViewModel"/> 能通过 <c>PropertyChangeTracker.Subscribe</c> 订阅它。
/// <para>
/// Kept intentionally lightweight (no backing POCO Model) to focus on the cross-VM subscription scenario;
/// the <c>Level</c> setter mirrors the proxy-property idiom used elsewhere in the Delegate tab.
/// 特意保持轻量（无底层 POCO Model）以聚焦跨 VM 订阅场景；
/// <c>Level</c> setter 沿用 Delegate 选项卡中其他地方使用的代理属性写法。
/// </para>
/// </summary>
public partial class SensorViewModel : ObservableObject
{
    private int _level = 50;

    /// <summary>
    /// Sensor level. Setter raises <see cref="ObservableObject.OnPropertyChanged(string?)"/> on the VM,
    /// enabling sibling VMs to subscribe via <see cref="Helpers.PropertyChangeTracker"/>.
    /// 传感器等级。setter 在 VM 自身引发通知，使兄弟 VM 可通过 <see cref="Helpers.PropertyChangeTracker"/> 订阅。
    /// </summary>
    public int Level
    {
        get => _level;
        set
        {
            if (_level != value)
            {
                _level = value;
                OnPropertyChanged();
            }
        }
    }
}
