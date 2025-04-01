using Microsoft.UI.Xaml.Data;

namespace DP_S_Marketplace.Converters;

public partial class NullableLongToDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is long longValue)
        {
            return (double)longValue;
        }
        return 0.0;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            return (long)doubleValue;
        }
        return null;
    }
}
