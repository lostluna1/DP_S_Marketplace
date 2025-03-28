using DP_S_Marketplace.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace DP_S_Marketplace.Views;

public sealed partial class AboutMePage : Page
{
    public AboutMeViewModel ViewModel
    {
        get;
    }

    public AboutMePage()
    {
        ViewModel = App.GetService<AboutMeViewModel>();
        InitializeComponent();
    }
}
