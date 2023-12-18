using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VideoDownloader.Converters
{
    /// <summary>
    /// 是否显示转换，true 显示， false 不显示，参数 1 代表反转
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            if (parameter != null && parameter.Equals("1"))
            {
                b = !b;
            }

            if (b)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }
}
