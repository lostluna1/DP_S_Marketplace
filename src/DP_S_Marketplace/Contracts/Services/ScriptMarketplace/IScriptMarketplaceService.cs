using System.Collections.ObjectModel;
using DP_S_Marketplace.Models;
using DP_S_Marketplace.Services;

namespace DP_S_Marketplace.Contracts.Services;
public interface IScriptMarketplaceService
{
    /// <summary>
    /// 从服务器下载插件到Linux
    /// </summary>
    /// <param name="projectInfo"></param>
    /// <returns></returns>
    Task DowloadToLinux(ProjectInfo projectInfo);

    /// <summary>
    /// 获取磁盘使用情况
    /// </summary>
    /// <returns></returns>
    ObservableCollection<FileSystemUsageModel> GetDiskUsages();

    /// <summary>
    /// 按文件类型获取使用情况
    /// </summary>
    /// <returns></returns>
    List<FileTypeUsage> GetFileTypeUsages();

    /// <summary>
    /// 从Linux服务端获取插件版本信息
    /// </summary>
    /// <returns></returns>
    Task<List<ProjectInfo>> GetServerPluginVersion();

    /// <summary>
    /// 从服务器获取插件
    /// </summary>
    /// <param name="projectInfos"></param>
    /// <returns></returns>
    Task<ObservableCollection<ProjectInfo>> GetServerPlugins();
}
