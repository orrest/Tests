using Bogus;

namespace Tests.Wpf.ModelNotification.Delegate.Models;

/// <summary>
/// Top-layer Model. Holds a single nested <see cref="ActionModel"/>.
/// Collection semantics live in the VM layer (StepViewModel.Actions), Model stays pure POCO data.
/// 顶层 Model，持有一个 <see cref="ActionModel"/>。集合语义在 VM 层，Model 保持纯 POCO 数据。
/// </summary>
public class StepModel
{
    /// <summary>Step name.</summary>
    public string Name { get; set; } = "Step";

    /// <summary>Whether this step is enabled.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Nested middle-layer Model. The wrapping VM exposes this via a proxy StepViewModel.Action.</summary>
    public ActionModel Action { get; set; }

    public StepModel()
    {
        Action = new ActionModel();
    }

    public StepModel(string name, bool enabled, ActionModel action)
    {
        Name = name;
        Enabled = enabled;
        Action = action;
    }

    // ===== Bogus 工厂：用 Faker 随机生成实例 =====

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    /// <summary>用 Bogus 随机生成一个 Step 实例（含随机 Action + Option）。</summary>
    public static StepModel CreateRandom() => new(
        Faker.Hacker.Noun(),
        Faker.Random.Bool(),
        ActionModel.CreateRandom());
}
