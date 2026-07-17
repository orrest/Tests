using System.Windows;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.ModelNotification.Models;

namespace Tests.Wpf.ModelNotification;

/// <summary>
/// 方式①：ViewModel 直接暴露整个 Model，View 通过 <c>{Binding User.Name}</c>、<c>{Binding User.Address.City}</c> 路径访问。
/// <para>
/// 因为 <see cref="UserModel"/> 和 <see cref="AddressModel"/> 自身都实现了 <c>INotifyPropertyChanged</c>，
/// 所以嵌套链路上的属性变化会**直接**通知 View，ViewModel 完全不需要做转发。
/// 这是 Direct 方式的最大优势：嵌套深链路 <c>User.Address.City</c> 自动可用。
/// </para>
/// <para>
/// <b>局限演示</b>：<see cref="Summary"/> 是 ViewModel 层的计算属性，依赖 Model 内部多个属性（Name/Age/City/Country）。
/// 但 Direct 方式下 VM 没有订阅 Model 的 PropertyChanged，所以 Model 内部变化**不会**自动刷新 Summary，
/// 只有在替换整个 User 时（<see cref="OnUserChanged"/>）Summary 才会刷新一次。
/// 这是 Direct 方式"无法响应 VM 层派生计算"的根本局限 —— 详见方式②③的解决。
/// </para>
/// </summary>
public partial class DirectUserViewModel : ObservableObject
{
    [ObservableProperty]
    public partial UserModel User { get; set; }

    /// <summary>显示当前 Model 的 HashCode，用于直观观察"替换 Model"是否生效。</summary>
    public string ModelId => $"#{User?.GetHashCode():X}";

    /// <summary>显示当前 Address 的 HashCode，用于观察"替换 Address"是否生效。</summary>
    public string AddressId => $"#{User?.Address?.GetHashCode():X}";

    /// <summary>
    /// VM 层计算属性：跨 Name/Age/City/Country 拼接。
    /// <b>注意</b>：由于 Direct 方式不订阅 Model 内部变化，此属性只在替换 User 时刷新，
    /// 不能随 Model 内部编辑实时变化 —— 这正是 Direct 方式的局限。
    /// </summary>
    public string Summary =>
        $"{User?.Name} ({User?.Age}岁) 来自 {User?.Address?.Country}, {User?.Address?.City}";

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    public DirectUserViewModel()
    {
        User = UserModel.CreateRandom();
    }

    /// <summary>
    /// 当 <see cref="User"/> 整体被替换时，源生成器会调用此 partial 方法。
    /// 用于刷新派生属性。
    /// </summary>
    partial void OnUserChanged(UserModel value)
    {
        OnPropertyChanged(nameof(ModelId));
        OnPropertyChanged(nameof(AddressId));
        OnPropertyChanged(nameof(Summary));
    }

    /// <summary>替换整个 Model 实例（用 Bogus 随机生成），验证旧绑定自动失效、新绑定建立。</summary>
    [RelayCommand]
    private void ReplaceUser()
    {
        User = UserModel.CreateRandom();
    }

    /// <summary>
    /// 替换嵌套的 Address 实例（用 Bogus 随机生成）。由于 <see cref="UserModel.Address"/> 也是 <c>[ObservableProperty]</c>，
    /// 赋值后 Model 会自动通知 <c>Address</c> 路径变化，View 的 <c>{Binding User.Address.City}</c> 自动重读。
    /// 同时主动刷新派生属性。
    /// </summary>
    [RelayCommand]
    private void ReplaceAddress()
    {
        User.Address = AddressModel.CreateRandom();
        OnPropertyChanged(nameof(AddressId));
        OnPropertyChanged(nameof(Summary));
    }

    /// <summary>
    /// 从后台线程修改 Model.Name（用 Bogus 随机生成新名字）。WPF 绑定的属性变化通知必须在 UI 线程触发，
    /// 因此用 <see cref="Dispatcher.Invoke"/> 切回 UI 线程。
    /// </summary>
    [RelayCommand]
    private async Task ModifyFromBackground()
    {
        string newName = Faker.Name.FullName();
        await Task.Run(() =>
        {
            Application.Current.Dispatcher.Invoke(() => User.Name = newName);
        });
    }
}
