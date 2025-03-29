using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Models;

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
        await ScriptMarketplaceService.GetServerPlugins(ProjectInfos);
    }


    [RelayCommand]
    public async Task DowloadToLinux()
    {
        await ScriptMarketplaceService.DowloadToLinux(SelectedProjectInfo!);
    }


}
