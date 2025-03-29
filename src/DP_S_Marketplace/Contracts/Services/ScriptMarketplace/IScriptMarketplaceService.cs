using System.Collections.ObjectModel;
using DP_S_Marketplace.Models;
using DP_S_Marketplace.Services;

namespace DP_S_Marketplace.Contracts.Services;
public interface IScriptMarketplaceService
{
    Task DowloadToLinux(ProjectInfo projectInfo);
    Task GetServerPlugins(ObservableCollection<ProjectInfo> projectInfos);

    ObservableCollection<FileSystemUsageModel> GetDiskUsages();
    List<FileTypeUsage> GetFileTypeUsages();
    Task<List<ProjectInfo>> GetServerPluginVersion();
}
