using System.Collections.ObjectModel;
using System.Diagnostics;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Helpers;
using DP_S_Marketplace.Models;
using DP_S_Marketplace.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Windows.System;

namespace DP_S_Marketplace.Views;

// TODO: Update NavigationViewItem titles and icons in ShellPage.xaml.
public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel
    {
        get;
    }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
        // A custom title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        //App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        //AppTitleBarText.Text = "AppDisplayName".GetLocalized();
    }

    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);

        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));

        ObservableCollection<ConnectionInfo> connections = await ConnectionHelper.LoadConnectionsAsync();
        // 直接更新 ViewModel
        ViewModel.Connections = connections ?? [];
        if (connections?.Count == 0)
        {
            // 需要弹出一个dialog，提示用户设置连接信息
            var dialog = new ContentDialog
            {
                Title = "提示",
                Content = "首次使用请设置数据库连接",
                CloseButtonText = "现在设置",
                XamlRoot = /*App.CurrentWindow.Content.XamlRoot//*/App.MainWindow.Content.XamlRoot
            };

            await dialog.ShowAsync();
            ViewModel.NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
        }
        //else
        //{
        //    try
        //    {

        //        ViewModel.SelectedConnection = ViewModel.Connections.FirstOrDefault(c => c.IsSelected) ?? ViewModel.Connections.FirstOrDefault();
             
        //    }
        //    catch (Exception ex)
        //    {
        //        //_ = Logger.Instance.WriteLogAsync($"ShellPage.OnLoaded : {ex}", "connectionLog.txt");
        //        throw new Exception($"{ViewModel.SelectedConnection?.Name} : 连接失败，请检查连接信息", ex);
        //    }
        //}
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        //App.AppTitlebar = AppTitleBarText as UIElement;
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        //AppTitleBar.Margin = new Thickness()
        //{
        //    Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
        //    Top = AppTitleBar.Margin.Top,
        //    Right = AppTitleBar.Margin.Right,
        //    Bottom = AppTitleBar.Margin.Bottom
        //};
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }
}
