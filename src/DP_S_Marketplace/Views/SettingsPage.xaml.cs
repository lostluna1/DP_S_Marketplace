using DP_S_Marketplace.Helpers;
using DP_S_Marketplace.Models;
using DP_S_Marketplace.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DP_S_Marketplace.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
        Loaded += SettingsPage_Loaded;
    }

    private async void AddNewConnection(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var dialog = new ConnectionInfoDialog(ViewModel)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot

        };

        await dialog.ShowAsync();
    }
    private async void SettingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Connections = await ConnectionHelper.LoadConnectionsAsync();
    }
    private void InvertedListView_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        if (sender is ListView listView)
        {
            if (e.OriginalSource is FrameworkElement { DataContext: ConnectionInfo clickedItem })
            {
                listView.SelectedItem = clickedItem;
            }
        }
    }
}
