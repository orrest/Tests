using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Wpf.Extensions;

// Markup extension that resolves a UserControl (with its ctor-injected viewmodel) directly
// from the DI container while parsing XAML; typically assigned to a placeholder
// ContentControl's Content inside a TabItem.
// Usage: Content="{extensions:UserControlBinding ServiceType={x:Type ns:MyView}, Services={x:Static app:App.Services}}"
// 在 XAML 解析时直接从 DI 容器解析 UserControl（VM 仍由 View 构造函数注入）的标记扩展，
// 通常赋给 TabItem 内占位 ContentControl 的 Content。
public class UserControlBindingExtension : MarkupExtension
{
    public Type? ServiceType { get; set; }

    public IServiceProvider Services { get; set; }

    public UserControlBindingExtension()
    {
        Services = null!;
    }

    // Code-side convenience constructor with validation.
    // 代码侧便捷构造，带参数校验。
    public UserControlBindingExtension(Type serviceType, IServiceProvider services)
    {
        if (!typeof(UserControl).IsAssignableFrom(serviceType))
        {
            throw new ArgumentException("ServiceType must be a UserControl type.", nameof(serviceType));
        }

        ServiceType = serviceType;
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (ServiceType is null)
        {
            throw new InvalidOperationException("ServiceType cannot be null.");
        }

        return Services.GetRequiredService(ServiceType);
    }
}
