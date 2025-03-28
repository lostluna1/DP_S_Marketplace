using DP_S_Marketplace.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace DP_S_Marketplace.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
