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
}
