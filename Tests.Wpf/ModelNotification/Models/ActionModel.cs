using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ModelNotification.Models;

/// <summary>
/// Middle layer Model of the three-layer nested scenario. Holds an inner <see cref="OptionModel"/>.
/// 三层嵌套场景的中层 Model，持有内层 <see cref="OptionModel"/>。
/// </summary>
public partial class ActionModel : ObservableObject
{
    /// <summary>Action name.</summary>
    [ObservableProperty]
    public partial string Name { get; set; } = "Action";

    /// <summary>Whether this action is enabled.</summary>
    [ObservableProperty]
    public partial bool Enabled { get; set; } = true;

    /// <summary>Execution delay in milliseconds, for demonstrating int binding.</summary>
    [ObservableProperty]
    public partial int DelayMs { get; set; } = 100;

    /// <summary>Nested inner Model. Replacing this instance triggers property-changed notification.</summary>
    [ObservableProperty]
    public partial OptionModel Option { get; set; }

    public ActionModel()
    {
        Option = new OptionModel();
    }

    public ActionModel(string name, bool enabled, int delayMs, OptionModel option)
    {
        Name = name;
        Enabled = enabled;
        DelayMs = delayMs;
        Option = option;
    }

    // ===== Bogus 工厂：用 Faker 随机生成实例 =====

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    /// <summary>用 Bogus 随机生成一个 Action 实例（含随机 Option）。</summary>
    public static ActionModel CreateRandom() => new(
        Faker.Hacker.Verb(),
        Faker.Random.Bool(),
        Faker.Random.Int(50, 1000),
        OptionModel.CreateRandom());
}
