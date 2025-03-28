using DP_S_Marketplace.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace DP_S_Marketplace.Views;

public sealed partial class ServerPage : Page
{
    public ServerViewModel ViewModel
    {
        get;
    }

    public ServerPage()
    {
        ViewModel = App.GetService<ServerViewModel>();
        InitializeComponent();
    }
}
