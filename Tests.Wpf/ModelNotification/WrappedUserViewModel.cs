using System.ComponentModel;
using System.Windows;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.ModelNotification.Models;

namespace Tests.Wpf.ModelNotification;

/// <summary>
/// 方式②：ViewModel 用代理属性包装 Model（含嵌套 Address），View 通过 <c>{Binding Name}</c>、<c>{Binding City}</c> 访问。
/// <para>
/// 嵌套场景的核心要点：Address 是 Model 内部的子对象，要让 View 绑定的代理属性 <c>City</c> 能响应 Address 内部变化，
/// 必须额外**订阅 Address 自身的 PropertyChanged**（而不是只订阅外层 UserModel）。
/// </para>
/// <para>
/// <b>替换 Address 时</b>：必须先解绑旧 Address 的事件，再绑新 Address，并主动刷新所有相关代理属性 + 派生属性。
/// 这里展示的是"手写 switch 转发"写法 —— 代码量随嵌套深度和属性数量线性增长，是此方式的代价。
/// 方式③用 <c>ModelBinder.ForwardProperties</c> 收敛这些样板。
/// </para>
/// </summary>
public partial class WrappedUserViewModel : ObservableObject
{
    private UserModel _user;

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    public WrappedUserViewModel()
    {
        _user = UserModel.CreateRandom();
        HookUser(_user);
        HookAddress(_user.Address);
    }

    // ===== 代理属性：UserModel =====

    public string Name
    {
        get => _user.Name;
        set => _user.Name = value;
    }

    public int Age
    {
        get => _user.Age;
        set => _user.Age = value;
    }

    /// <summary>转发自 Model 的计算属性（Model 层已用 NotifyPropertyChangedFor 自动联动）。</summary>
    public bool IsAdult => _user.IsAdult;

    // ===== 代理属性：嵌套 AddressModel =====

    public string City
    {
        get => _user.Address.City;
        set => _user.Address.City = value;
    }

    public string Country
    {
        get => _user.Address.Country;
        set => _user.Address.Country = value;
    }

    public string ZipCode
    {
        get => _user.Address.ZipCode;
        set => _user.Address.ZipCode = value;
    }

    /// <summary>转发自 Address 的计算属性。</summary>
    public string FullAddress => _user.Address.FullAddress;

    // ===== 派生 / 计算属性 =====

    /// <summary>VM 层计算属性：跨 User.Name/Age + Address.City/Country。</summary>
    public string Greeting => $"Hello, {_user.Name}!";

    /// <summary>VM 层跨层计算属性。</summary>
    public string Summary => $"{_user.Name} ({_user.Age}岁) 来自 {_user.Address.Country}, {_user.Address.City}";

    public string ModelId => $"#{_user.GetHashCode():X}";

    public string AddressId => $"#{_user.Address.GetHashCode():X}";

    // ===== 事件订阅 / 转发 =====

    private void HookUser(UserModel user) => user.PropertyChanged += User_PropertyChanged;
    private void UnhookUser(UserModel user) => user.PropertyChanged -= User_PropertyChanged;

    private void HookAddress(AddressModel address) => address.PropertyChanged += Address_PropertyChanged;
    private void UnhookAddress(AddressModel address) => address.PropertyChanged -= Address_PropertyChanged;

    /// <summary>转发外层 UserModel 的属性变化。</summary>
    private void User_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(UserModel.Name):
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Greeting));
                OnPropertyChanged(nameof(Summary));
                break;
            case nameof(UserModel.Age):
                OnPropertyChanged(nameof(Age));
                OnPropertyChanged(nameof(IsAdult));   // Model 层计算属性联动
                OnPropertyChanged(nameof(Summary));
                break;
            case nameof(UserModel.Address):
                // 嵌套实例被替换：解绑旧的，绑新的（ReplaceAddress 命令里已处理，这里作为防御性兜底）
                OnPropertyChanged(nameof(AddressId));
                break;
        }
    }

    /// <summary>★ 核心：转发嵌套 AddressModel 的属性变化。</summary>
    private void Address_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(AddressModel.City):
                OnPropertyChanged(nameof(City));
                OnPropertyChanged(nameof(Summary));
                break;
            case nameof(AddressModel.Country):
                OnPropertyChanged(nameof(Country));
                OnPropertyChanged(nameof(Summary));
                break;
            case nameof(AddressModel.ZipCode):
                OnPropertyChanged(nameof(ZipCode));
                OnPropertyChanged(nameof(FullAddress)); // Model 层计算属性联动
                break;
            case nameof(AddressModel.FullAddress):
                // Address 自身 [NotifyPropertyChangedFor] 会通知 FullAddress，转发到 VM
                OnPropertyChanged(nameof(FullAddress));
                break;
        }
    }

    // ===== 命令 =====

    /// <summary>替换整个 Model 实例（用 Bogus 随机生成）：解绑旧 User + 旧 Address，绑新 User + 新 Address，刷新所有。</summary>
    [RelayCommand]
    private void ReplaceUser()
    {
        UnhookUser(_user);
        UnhookAddress(_user.Address);
        _user = UserModel.CreateRandom();
        HookUser(_user);
        HookAddress(_user.Address);
        RefreshAll();
    }

    /// <summary>替换嵌套 Address 实例（用 Bogus 随机生成）：解绑旧 Address，绑新 Address，刷新相关属性。</summary>
    [RelayCommand]
    private void ReplaceAddress()
    {
        UnhookAddress(_user.Address);
        _user.Address = AddressModel.CreateRandom();
        HookAddress(_user.Address);
        // 刷新所有受 Address 影响的代理 + 派生属性
        OnPropertyChanged(nameof(City));
        OnPropertyChanged(nameof(Country));
        OnPropertyChanged(nameof(ZipCode));
        OnPropertyChanged(nameof(FullAddress));
        OnPropertyChanged(nameof(Summary));
        OnPropertyChanged(nameof(AddressId));
    }

    /// <summary>从后台线程修改 Model.Name（用 Bogus 随机生成新名字），验证转发通知链路。</summary>
    [RelayCommand]
    private async Task ModifyFromBackground()
    {
        string newName = Faker.Name.FullName();
        await Task.Run(() =>
        {
            Application.Current.Dispatcher.Invoke(() => _user.Name = newName);
        });
    }

    private void RefreshAll()
    {
        foreach (string p in new[]
        {
            nameof(Name), nameof(Age), nameof(IsAdult),
            nameof(City), nameof(Country), nameof(ZipCode), nameof(FullAddress),
            nameof(Greeting), nameof(Summary),
            nameof(ModelId), nameof(AddressId)
        })
        {
            OnPropertyChanged(p);
        }
    }
}
