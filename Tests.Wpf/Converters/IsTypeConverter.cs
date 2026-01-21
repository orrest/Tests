using System.Globalization;
using System.Windows.Data;

namespace Tests.Wpf.Converters;


public sealed class IsTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null || parameter is null)
            return false;

        var targetTypeParam = parameter as Type;

        return targetTypeParam?.IsInstanceOfType(value) == true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
