using System.Text.Json;
using System.Text.RegularExpressions;
using DP_S_Marketplace.Models;
using DP_S_Marketplace.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    private new void PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var stackPanel = sender as StackPanel;
        var toolTip = ToolTipService.GetToolTip(stackPanel) as ToolTip;
        if (toolTip != null)
        {
            toolTip.IsOpen = true;
        }
    }

    private void StackPanel_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var stackPanel = sender as StackPanel;
        var toolTip = ToolTipService.GetToolTip(stackPanel) as ToolTip;
        if (toolTip != null)
        {
            toolTip.IsOpen = false;
        }
    }
    private async void EditScripConfig_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ServerTableView.SelectedItem is ProjectInfo selectedItem)
        {

            var rawJson = await ViewModel.GetConfigContentAsync(selectedItem);
            

            ViewModel.EditConfigFile = rawJson;

            var dialog = new EditConfigDialog(ViewModel)
            {
                XamlRoot = App.MainWindow.Content.XamlRoot
            };

            await dialog.ShowAsync();

        }

    }
    private void ServerTableView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement element && element.DataContext != null)
        {
            ServerTableView.SelectedItem = element.DataContext;
        }
    }

}
