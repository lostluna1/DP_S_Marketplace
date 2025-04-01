using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Helpers;
using DP_S_Marketplace.Messages;
using DP_S_Marketplace.Models;
using DP_S_Marketplace.Views;

using Microsoft.UI.Xaml.Navigation;

namespace DP_S_Marketplace.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial bool IsBackEnabled
    {
    get; set;
    }

    [ObservableProperty]
    public partial object? Selected
    {
        get;set;
    }

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }
    [ObservableProperty]
    public partial ObservableCollection<ConnectionInfo>? Connections { get; set; } = new ObservableCollection<ConnectionInfo>();

    [ObservableProperty]
    public partial ConnectionInfo? SelectedConnection
    {
        get; set;
    }
    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
        Initialize();
    }

    /// <summary>
    /// 接收另一个视图模型的消息并设置连接信息
    /// </summary>
    public void Initialize()
    {
        // 注册消息接收器
        WeakReferenceMessenger.Default.Register<ConnectionsUpdatedMessage>(this, async (r, m) =>
        {
            // 加载连接信息
            Connections = await ConnectionHelper.LoadConnectionsAsync();
        });
    }
    partial void OnSelectedConnectionChanged(ConnectionInfo? value)
    {
        //IsConnecting = true;
        if (value == null)
        {
            return;
        }
        GlobalVariables.Instance.ConnectionInfo = value;

        //try
        //{
        //    DatabaseHelper.ResetConnections();
        //    var d_taiwan = DatabaseHelper.DTaiwan;
        //    d_taiwan.CodeFirst.SyncStructure<Accounts>();
        //    IsConnecting = false;
        //    AccountInfos = new ObservableCollection<Accounts>(d_taiwan.Select<Accounts>().ToList());
        //}
        //catch (Exception ex)
        //{
        //    IsConnecting = false;
        //    _ = Logger.Instance.WriteLogAsync($"OnSelectedConnectionChanged : {ex}", "connectionLog.txt");
        //    throw new Exception($"{GlobalVariables.Instance.ConnectionInfo.Name} : 连接失败，请检查连接信息", ex);
        //}
    }
    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        if (e.SourcePageType == typeof(SettingsPage))
        {
            Selected = NavigationViewService.SettingsItem;
            return;
        }

        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }
}
