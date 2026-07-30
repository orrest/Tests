using CommunityToolkit.Mvvm.ComponentModel;
using Tests.Wpf.ModelNotification.Direct.Models;

namespace Tests.Wpf.ModelNotification.Direct;

/// <summary>
/// Innermost ViewModel. Exposes the underlying <see cref="OptionModel"/> directly.
/// No proxy properties — the View binds through <c>Model.Name/Value/Enabled</c>
/// and reads property-change notifications directly from the Model.
/// 最内层 ViewModel，直接暴露 <see cref="OptionModel"/>。
/// 无代理属性——View 通过 <c>Model.Name/Value/Enabled</c> 绑定，
/// 属性变化通知直接来自 Model。
/// </summary>
public partial class OptionViewModel : ObservableObject
{
    /// <summary>Wrapped Model. Public so the View and parent VM can read/write its observable properties.</summary>
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
}
