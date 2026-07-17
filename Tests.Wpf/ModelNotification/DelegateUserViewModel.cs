using System.Windows;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.ModelNotification.Helpers;
using Tests.Wpf.ModelNotification.Models;

namespace Tests.Wpf.ModelNotification;

/// <summary>
/// 方式③：代理属性包装 Model（含嵌套 Address），用 <see cref="ModelBinder.ForwardProperties"/>
/// 把"Model 属性变化 → VM 代理属性/派生属性通知"的样板收敛为声明式映射。
/// <para>
/// 与方式②功能等价（同样支持嵌套 Address 的实时通知 + 实例替换），
/// 区别只在工程化：方式②的 <c>switch (e.PropertyName)</c> 随属性数增长而冗长，
/// 这里用一行 <c>ForwardProperties</c> 描述所有映射（含派生联动），更易维护。
/// </para>
/// <para>
/// 嵌套场景下需要分别为外层 UserModel 和内层 AddressModel 各调用一次 ForwardProperties，
/// 且替换实例时重新调用以重新订阅新对象。
/// </para>
/// </summary>
public partial class DelegateUserViewModel : ObservableObject
{
    private UserModel _user;

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    public DelegateUserViewModel()
    {
        _user = UserModel.CreateRandom();
        BindUser(_user);
        BindAddress(_user.Address);
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

    public string FullAddress => _user.Address.FullAddress;

    // ===== 派生 / 计算属性 =====

    public string Greeting => $"Hello, {_user.Name}!";

    public string Summary => $"{_user.Name} ({_user.Age}岁) 来自 {_user.Address.Country}, {_user.Address.City}";

    public string ModelId => $"#{_user.GetHashCode():X}";

    public string AddressId => $"#{_user.Address.GetHashCode():X}";

    // ===== 用通用转发器建立 Model → VM 的通知映射 =====

    /// <summary>建立外层 UserModel 属性 → VM 代理/派生属性 的映射。</summary>
    private void BindUser(UserModel user)
    {
        ModelBinder.ForwardProperties(
            this,
            user,
            // (Model 属性, VM 代理属性, 附带的派生属性数组)
            (m => m.Name, () => Name, [() => Greeting, () => Summary]),
            (m => m.Age, () => Age, [() => IsAdult, () => Summary]));
    }

    /// <summary>建立嵌套 AddressModel 属性 → VM 代理/派生属性 的映射。</summary>
    private void BindAddress(AddressModel address)
    {
        ModelBinder.ForwardProperties(
            this,
            address,
            (m => m.City, () => City, [() => Summary]),
            (m => m.Country, () => Country, [() => Summary]),
            // ZipCode 不影响 Summary，只影响 FullAddress；FullAddress 也可由 Address 自身联动通知
            (m => m.ZipCode, () => ZipCode, [() => FullAddress]));
    }

    // ===== 命令 =====

    /// <summary>替换整个 Model 实例（用 Bogus 随机生成）：重新绑定 User + Address，刷新所有。</summary>
    [RelayCommand]
    private void ReplaceUser()
    {
        _user = UserModel.CreateRandom();
        BindUser(_user);
        BindAddress(_user.Address);
        RefreshAll();
    }

    /// <summary>替换嵌套 Address 实例（用 Bogus 随机生成）：重新绑定 Address，刷新相关属性。</summary>
    [RelayCommand]
    private void ReplaceAddress()
    {
        _user.Address = AddressModel.CreateRandom();
        BindAddress(_user.Address);
        foreach (string p in new[]
        {
            nameof(City), nameof(Country), nameof(ZipCode), nameof(FullAddress),
            nameof(Summary), nameof(AddressId)
        })
        {
            OnPropertyChanged(p);
        }
    }

    /// <summary>从后台线程修改 Model.Name（用 Bogus 随机生成新名字），验证转发链路。</summary>
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
