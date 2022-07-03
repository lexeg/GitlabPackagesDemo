using System.Windows;
using System.Windows.Data;

namespace GitlabPackagesDemo.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {

        public bool Inverted { get; set; }
        
        public object Convert(object value, System.Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return (visibility == Visibility.Visible) ^ Inverted;
            }

            return value is bool b && b ^ Inverted ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ^ Inverted ? Visibility.Visible : Visibility.Collapsed;
            }
            return value != null && ((Visibility)value == Visibility.Visible) ^ Inverted;
        }
    }
}
