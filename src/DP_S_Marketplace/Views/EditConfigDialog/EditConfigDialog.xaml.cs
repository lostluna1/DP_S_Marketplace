using DP_S_Marketplace.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DP_S_Marketplace.Views;

public sealed partial class EditConfigDialog : ContentDialog
{

    public ServerViewModel ViewModel
    {
        get;
    }

    public EditConfigDialog(ServerViewModel serverViewModel)
    {
        InitializeComponent();
        ViewModel = serverViewModel;
        DataContext = ViewModel;
    }

}
