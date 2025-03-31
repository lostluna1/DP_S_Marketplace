using DP_S_Marketplace.Models;
using DP_S_Marketplace.ViewModels;
using Microsoft.UI.Xaml;
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
        Loaded += ScriptMarketplacePage_Loaded;
    }

    private async void ScriptMarketplacePage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.GetServerPlugins();
    }

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is ProjectInfo projectInfo)
        {
            ViewModel.SelectedProjectInfo = projectInfo;
        }

        await ViewModel.DowloadToLinux();
    }

    private void PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
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
}
