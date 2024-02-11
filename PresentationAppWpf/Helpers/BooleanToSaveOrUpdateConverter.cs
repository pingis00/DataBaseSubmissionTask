using System.Globalization;
using System.Windows.Data;

namespace PresentationAppWpf.Helpers;

public class BooleanToSaveOrUpdateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "Uppdatera" : "Spara";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("BooleanToSaveOrUpdateConverter is a one-way converter.");
    }
}
