using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Models;
using WinRT;

namespace DP_S_Marketplace.ViewModels;

public partial class ScriptMarketplaceViewModel : ObservableRecipient
{
    public IScriptMarketplaceService ScriptMarketplaceService
    {
        get;
    }

    [ObservableProperty]
    public partial ObservableCollection<ProjectInfo> ProjectInfos { get; set; } = new();

    [ObservableProperty]
    public partial ProjectInfo? SelectedProjectInfo
    {
        get; set;
    } = new();

    public ScriptMarketplaceViewModel(IScriptMarketplaceService scriptMarketplaceService)
    {
        ScriptMarketplaceService = scriptMarketplaceService;
        _ = GetServerPlugins();
    }


    [RelayCommand]
    public async Task GetServerPlugins()
    {
        var pluginsServer = await ScriptMarketplaceService.GetServerPlugins();
        var pluginsInstalled = new ObservableCollection<ProjectInfo>(await ScriptMarketplaceService.GetServerPluginVersion());

        foreach (var pluginServer in pluginsServer)
        {
            var pluginInstalled = pluginsInstalled.FirstOrDefault(p => p.ProjectName == pluginServer.ProjectName);
            if (pluginInstalled == null)
            {
                // 插件未安装
                pluginServer.Status = PluginStatus.NotInstalled;
            }
            else if (pluginInstalled.ProjectVersion < pluginServer.ProjectVersion)
            {
                // 可以更新
                pluginServer.Status = PluginStatus.CanUpdate;
            }
            else
            {
                // 已是最新版本
                pluginServer.Status = PluginStatus.LatestVersion;
            }
        }

        ProjectInfos = pluginsServer;
    }



    [RelayCommand]
    public async Task DowloadToLinux()
    {
        await ScriptMarketplaceService.DowloadToLinux(SelectedProjectInfo!);
        // 下载完成后，更新插件的状态
        SelectedProjectInfo!.Status = PluginStatus.LatestVersion;
    }


}
