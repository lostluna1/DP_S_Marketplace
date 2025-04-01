using System.Text.Json;
using System.Text.RegularExpressions;
using DP_S_Marketplace.Helpers;
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
        Loaded += ServerPage_Loaded;
    }

    private async void ServerPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitializeAsync();
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

            try
            {
                var dialog = new EditConfigDialog(ViewModel)
                {
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };

                await dialog.ShowAsync();

                var rawJson = await ViewModel.GetConfigContentAsync(selectedItem);
                if (rawJson is null)
                {
                    return;
                }
                ViewModel.EditConfigFile = rawJson;
            }
            catch (Exception ex)
            {
                GrowlMsg.Show(ex.Message,false);
                throw new Exception(ex.Message);
            }
            
            

        }

    }
    private void ServerTableView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement element && element.DataContext != null)
        {
            ServerTableView.SelectedItem = element.DataContext;
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is ProjectInfo projectInfo)
        {
            ViewModel.SlectedProjectInfo = projectInfo;
        }
        await ViewModel.DeleteFromLinux();
    }
}
