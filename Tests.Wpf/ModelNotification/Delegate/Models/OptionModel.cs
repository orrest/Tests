using Bogus;

namespace Tests.Wpf.ModelNotification.Delegate.Models;

/// <summary>
/// Innermost Model of the three-layer nested scenario. Pure POCO — no INotifyPropertyChanged.
/// Property-change notifications are raised by the wrapping ViewModel via proxy properties,
/// so the Model itself never raises events.
/// 三层嵌套场景的最内层 Model，纯 POCO——不实现 INotifyPropertyChanged。
/// 属性变化通知由外层 ViewModel 通过代理属性触发，Model 自身不引发任何事件。
/// </summary>
public class OptionModel
{
    /// <summary>Option name, e.g. "Color" / "Timeout".</summary>
    public string Name { get; set; } = "Option";

    /// <summary>Option value as string, e.g. "Red" / "30s".</summary>
    public string Value { get; set; } = "Default";

    /// <summary>Whether this option is enabled.</summary>
    public bool Enabled { get; set; } = true;

    // ===== Bogus 工厂：用 Faker 随机生成实例 =====

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    /// <summary>用 Bogus 随机生成一个 Option 实例。</summary>
    public static OptionModel CreateRandom() => new()
    {
        Name = Faker.Hacker.Noun(),
        Value = Faker.Hacker.Adjective(),
        Enabled = Faker.Random.Bool(),
    };
}
