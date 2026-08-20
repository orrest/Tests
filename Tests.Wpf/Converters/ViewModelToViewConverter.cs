using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tests.Wpf.Converters;

// Resolves a transient view from DI for the bound viewmodel, by naming convention
// (XxxViewModel -> XxxView in the same namespace and assembly).
// 按命名约定（同命名空间 XxxViewModel -> XxxView）从 DI 容器解析 transient View。
public class ViewModelToViewConverter : IValueConverter
{
    // When used on a direct Content binding (outside any DataTemplate) the resolved view
    // would inherit the parent's DataContext; pass ConverterParameter=AttachDataContext
    // to attach the bound viewmodel to the view explicitly.
    // 直接用于 Content 绑定（不在 DataTemplate 内）时，View 会继承父级 DataContext；
    // 传 ConverterParameter=AttachDataContext 可显式挂上绑定的 VM。
    public const string ATTACH_DATA_CONTEXT_PARAMETER = "AttachDataContext";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        var viewModelType = value.GetType();
        if (viewModelType.FullName is not { } fullName ||
            !fullName.EndsWith("ViewModel", StringComparison.Ordinal))
        {
            return null;
        }

        var viewTypeName = fullName[..^"ViewModel".Length] + "View";
        var viewType = viewModelType.Assembly.GetType(viewTypeName);
        if (viewType is null)
        {
            return null;
        }

        var view = App.Services.GetService(viewType);
        if (view is FrameworkElement element &&
            string.Equals(parameter as string, ATTACH_DATA_CONTEXT_PARAMETER, StringComparison.Ordinal))
        {
            element.DataContext = value;
        }

        return view;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
