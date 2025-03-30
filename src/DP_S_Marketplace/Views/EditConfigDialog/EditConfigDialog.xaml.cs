using DP_S_Marketplace.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Monaco;

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

    private void MonacoEditor_Loaded(object sender, EventArgs e)
    {
        if (ViewModel.EditConfigFile is not null)
        {
            MonacoEditor.EditorContent = ViewModel.EditConfigFile;
        }
        MonacoEditor.Language = "json";
    }

    private void MonacoEditor_EditorContentChanged(object sender, EventArgs e)
    {
        var monacoEditor = sender as MonacoEditor;
        if (monacoEditor is not null)
        {
            var text = monacoEditor.EditorContent;
            ViewModel.EditConfigFile = text; 
        }
    }
}
