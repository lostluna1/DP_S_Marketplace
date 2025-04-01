using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Models;
using DP_S_Marketplace.Services;
using WinRT;

namespace DP_S_Marketplace.ViewModels;

public partial class ScriptMarketplaceViewModel : ObservableRecipient
{
    public IScriptInstaller ScriptInstaller
    {
        get;
    }
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

    public ScriptMarketplaceViewModel(IScriptMarketplaceService scriptMarketplaceService, IScriptInstaller scriptInstaller)
    {
        ScriptMarketplaceService = scriptMarketplaceService;
        ScriptInstaller = scriptInstaller;

    }

    public async Task GetServerPlugins()
    {
        // 将长耗时任务放到后台线程，避免阻塞 UI
        await Task.Run(async () =>
        {
            var installedVersion = await ScriptInstaller.GetInstalledVersion().ConfigureAwait(false);
            if (installedVersion.ProjectVersion == 0)
            {
                // 未连接服务器
                return;
            }

            var pluginsServer = await ScriptMarketplaceService.GetServerPlugins().ConfigureAwait(false);
            var pluginsInstalled = new ObservableCollection<ProjectInfo>(
            await ScriptMarketplaceService.GetServerPluginVersion().ConfigureAwait(false)
        );

            // 比对并更新插件状态
            foreach (var plugin in pluginsServer)
            {
                var installedPlugin = pluginsInstalled.FirstOrDefault(p => p.ProjectName == plugin.ProjectName);
                if (installedPlugin == null)
                {
                    plugin.Status = PluginStatus.NotInstalled;
                }
                else if (installedPlugin.ProjectVersion < plugin.ProjectVersion)
                {
                    plugin.Status = PluginStatus.CanUpdate;
                }
                else
                {
                    plugin.Status = PluginStatus.LatestVersion;
                }
            }

            // 回到 WinUI 3 的 DispatcherQueue 更新 UI（示例用 App.MainWindow）
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ProjectInfos = pluginsServer;
            });
        }).ConfigureAwait(false);
    }




    [RelayCommand]
    public async Task DowloadToLinux()
    {
        await ScriptMarketplaceService.DowloadToLinux(SelectedProjectInfo!);
        // 下载完成后，更新插件的状态
        SelectedProjectInfo!.Status = PluginStatus.LatestVersion;
    }


}
