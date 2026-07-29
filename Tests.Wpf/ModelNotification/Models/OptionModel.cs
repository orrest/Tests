using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ModelNotification.Models;

/// <summary>
/// Innermost Model of the three-layer nested scenario. Pure data, observable for binding.
/// 三层嵌套场景的最内层 Model，纯数据，继承 ObservableObject 以支持属性变化通知。
/// </summary>
public partial class OptionModel : ObservableObject
{
    /// <summary>Option name, e.g. "Color" / "Timeout".</summary>
    [ObservableProperty]
    public partial string Name { get; set; } = "Option";

    /// <summary>Option value as string, e.g. "Red" / "30s".</summary>
    [ObservableProperty]
    public partial string Value { get; set; } = "Default";

    /// <summary>Whether this option is enabled.</summary>
    [ObservableProperty]
    public partial bool Enabled { get; set; } = true;

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
