using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace DP_S_Marketplace.Helpers;
public partial class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is bool boolValue ? boolValue ? Visibility.Visible : Visibility.Collapsed : (object)Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is Visibility visibility ? visibility == Visibility.Visible : (object)false;
    }
}