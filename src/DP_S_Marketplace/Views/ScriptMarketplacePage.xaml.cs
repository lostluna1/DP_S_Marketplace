using DP_S_Marketplace.Models;
using DP_S_Marketplace.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace DP_S_Marketplace.Views;

public sealed partial class ScriptMarketplacePage : Page
{
    public ScriptMarketplaceViewModel ViewModel
    {
        get;
    }

    public ScriptMarketplacePage()
    {
        ViewModel = App.GetService<ScriptMarketplaceViewModel>();
        InitializeComponent();
    }

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is ProjectInfo projectInfo)
        {
            ViewModel.SelectedProjectInfo = projectInfo;
        }

        await ViewModel.DowloadToLinux();
    }

}
