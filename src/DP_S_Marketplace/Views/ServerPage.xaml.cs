using System.Text.Json;
using DP_S_Marketplace.Models;
using DP_S_Marketplace.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

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

    private async void EditScripConfig_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ServerTableView.SelectedItem is ProjectInfo selectedItem)
        {
            // 假设您从某个方法获取原始的 JSON 内容
            string rawJson = await ViewModel.GetConfigContentAsync(selectedItem);

            // 使用 System.Text.Json 格式化 JSON 字符串
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(rawJson);
            string formattedJson = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });

            ViewModel.EditConfigFile = formattedJson;

            var dialog = new EditConfigDialog(ViewModel)
            {
                XamlRoot = App.MainWindow.Content.XamlRoot
            };

            await dialog.ShowAsync();

            // 如果用户点击了“确定”，您可以获取更新后的内容
            // 并进行相应的处理，例如保存回服务器
        }

    }
    private async void ServerTableView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement element && element.DataContext != null)
        {
            ServerTableView.SelectedItem = element.DataContext;
            //ViewModel.EditConfigFile = await ViewModel.get((ProjectInfo)ServerTableView.SelectedItem);
        }
    }

}
