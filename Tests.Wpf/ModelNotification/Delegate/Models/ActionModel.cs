using Bogus;

namespace Tests.Wpf.ModelNotification.Delegate.Models;

/// <summary>
/// Middle-layer Model. Holds a single nested <see cref="OptionModel"/>.
/// Collection semantics live in the VM layer (ActionViewModel.Options), Model stays pure POCO data.
/// 中层 Model，持有一个 <see cref="OptionModel"/>。集合语义在 VM 层，Model 保持纯 POCO 数据。
/// </summary>
public class ActionModel
{
    /// <summary>Action name.</summary>
    public string Name { get; set; } = "Action";

    /// <summary>Whether this action is enabled.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Execution delay in milliseconds, for demonstrating int binding.</summary>
    public int DelayMs { get; set; } = 100;

    /// <summary>Nested inner Model. The wrapping VM exposes this via a proxy ActionViewModel.Option.</summary>
    public OptionModel Option { get; set; }

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
