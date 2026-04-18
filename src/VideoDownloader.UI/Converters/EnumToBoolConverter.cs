using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace VideoDownloader.UI.Converters;

public class EnumToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(parameter) ?? false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is true && parameter is not null)
        {
            return parameter;
        }
        return AvaloniaProperty.UnsetValue;
    }
}
