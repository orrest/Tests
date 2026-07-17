using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ModelNotification.Models;

/// <summary>
/// 嵌套的 Address Model，作为 <see cref="UserModel"/> 的子对象。
/// 继承 <see cref="ObservableObject"/> 以支持自身属性变化通知。
/// </summary>
public partial class AddressModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullAddress))]
    public partial string City { get; set; } = "北京";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullAddress))]
    public partial string Country { get; set; } = "中国";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullAddress))]
    public partial string ZipCode { get; set; } = "100000";

    public AddressModel()
    {
    }

    public AddressModel(string city, string country, string zipCode)
    {
        City = city;
        Country = country;
        ZipCode = zipCode;
    }

    /// <summary>
    /// Model 层计算属性：由 City/Country/ZipCode 拼接而成。
    /// 通过 <c>[NotifyPropertyChangedFor]</c> 挂在三个源属性上，
    /// 任一源属性变化时 Model 自身自动通知 FullAddress 变化，无需手写转发。
    /// </summary>
    public string FullAddress => $"{Country}, {City} ({ZipCode})";

    // ===== Bogus 工厂：用 Faker 随机生成实例 =====

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    /// <summary>用 Bogus 随机生成一个 Address 实例。</summary>
    public static AddressModel CreateRandom() => new(
        Faker.Address.City(),
        Faker.Address.Country(),
        Faker.Address.ZipCode());
}
