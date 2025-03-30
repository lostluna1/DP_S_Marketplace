using DP_S_Marketplace.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
namespace DP_S_Marketplace.Helpers;
public class StatusToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is PluginStatus status && parameter is string targetStatus)
        {
            return status.ToString() == targetStatus ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
