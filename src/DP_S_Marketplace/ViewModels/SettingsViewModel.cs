using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Helpers;
using DP_S_Marketplace.Messages;
using DP_S_Marketplace.Models;
using Microsoft.UI.Xaml;

using Windows.ApplicationModel;

namespace DP_S_Marketplace.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    public ICommand SwitchThemeCommand
    {
        get;
    }
    [ObservableProperty]
    public partial string? NewConnectionName
    {
        get; set;
    }
    [ObservableProperty]
    public partial string? NewConnectionIp
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? NewConnectionPort { get; set; } = "3306";

    [ObservableProperty]
    public partial string? NewConnectionUser
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? NewConnectionPassword { get; set; }
    [ObservableProperty]
    public partial string? SelectedPassword
    {
        get; set;
    }

    [ObservableProperty]
    public partial ConnectionInfo? SelectedConnection
    {
        get; set;
    }

    /// <summary>
    /// 测试连接是否成功
    /// </summary>
    [ObservableProperty]
    public partial bool TestFail { get; set; } = false;

    [ObservableProperty]
    public partial string? TestResult { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<ConnectionInfo> Connections { get; set; } = [];
    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
    }
    [RelayCommand]
    public async Task SaveNewConnectionAsync()
    {
        if (string.IsNullOrWhiteSpace(NewConnectionName) || string.IsNullOrWhiteSpace(NewConnectionIp) ||
            string.IsNullOrWhiteSpace(NewConnectionPort) || string.IsNullOrWhiteSpace(NewConnectionUser) ||
            string.IsNullOrWhiteSpace(NewConnectionPassword))
        {
            throw new Exception("检查连接信息是否完整");
        }
        var connection = new ConnectionInfo
        {
            Name = NewConnectionName,
            Ip = NewConnectionIp,
            Port = NewConnectionPort,
            User = NewConnectionUser,
            Password = NewConnectionPassword
        };

        await ConnectionHelper.AddConnectionAsync(connection);
        Connections = await ConnectionHelper.LoadConnectionsAsync();

        // 发送消息通知其他 ViewModel
        WeakReferenceMessenger.Default.Send(new ConnectionsUpdatedMessage(true));
    }

    [RelayCommand]
    public async Task SetDefaultConnectionAsync()
    {
        if (SelectedConnection != null)
        {
            await ConnectionHelper.SetDefaultConnectionAsync(SelectedConnection);
            await ConnectionHelper.LoadConnectionsAsync();

            // 发送消息通知其他 ViewModel
            WeakReferenceMessenger.Default.Send(new ConnectionsUpdatedMessage(true));
        }
    }
    [RelayCommand]
    public async Task TestConnection()
    {
        var connection = new ConnectionInfo
        {
            Name = NewConnectionName,
            Ip = NewConnectionIp,
            Port = NewConnectionPort,
            User = NewConnectionUser,
            Password = NewConnectionPassword
        };

        //var isSuccess = await DatabaseHelper.TestDatabaseConnectionAsync(connection);
        //GrowlMsg.Show("", isSuccess);
    }
    [RelayCommand]
    public async Task DeleteSelectedConnectionAsync()
    {
        if (SelectedConnection != null)
        {
            await ConnectionHelper.DeleteConnectionAsync(SelectedConnection);
            Connections = await ConnectionHelper.LoadConnectionsAsync();

            // 发送消息通知其他 ViewModel
            WeakReferenceMessenger.Default.Send(new ConnectionsUpdatedMessage(true));
        }
    }


    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
