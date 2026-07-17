using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ModelNotification.Models;

/// <summary>
/// 可观察的 Model。继承 <see cref="ObservableObject"/> 即自动实现 <c>INotifyPropertyChanged</c>，
/// 这样无论是 View 直接绑定 <c>User.Name</c>，还是 ViewModel 通过代理属性转发，
/// 都能在 Model 属性变化时拿到通知。
/// <para>
/// 本类还包含一个嵌套 Model <see cref="Address"/>，用于演示嵌套通知链路。
/// </para>
/// </summary>
public partial class UserModel : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; } = "张三";

    /// <summary>
    /// 挂 <c>[NotifyPropertyChangedFor(nameof(IsAdult))]</c> 后，
    /// Age 变化时 Model 会自动通知计算属性 <see cref="IsAdult"/> 变化。
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAdult))]
    public partial int Age { get; set; } = 25;

    /// <summary>嵌套 Model：替换此实例会触发 <c>Address</c> 属性通知（因为它是 ObservableProperty）。</summary>
    [ObservableProperty]
    public partial AddressModel Address { get; set; }

    /// <summary>Model 层计算属性：是否成年。</summary>
    public bool IsAdult => Age >= 18;

    public UserModel()
    {
        Address = new AddressModel();
    }

    public UserModel(string name, int age)
    {
        Name = name;
        Age = age;
        Address = new AddressModel();
    }

    public UserModel(string name, int age, AddressModel address)
    {
        Name = name;
        Age = age;
        Address = address;
    }

    // ===== Bogus 工厂：用 Faker 随机生成实例 =====

    private static readonly Faker Faker = Tests.Wpf.Helpers.FakerHelper.INSTANCE;

    /// <summary>用 Bogus 随机生成一个 User 实例（含随机 Address）。</summary>
    public static UserModel CreateRandom() => new(
        Faker.Name.FullName(),
        Faker.Random.Int(10, 80),
        AddressModel.CreateRandom());
}
