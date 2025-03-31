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

    /// <summary>
    /// 从Linux获取nut脚本配置
    /// </summary>
    /// <param name="remoteFilePath"></param>
    /// <returns></returns>
    Task<string> GetRemoteConfigFileAsync(string remoteFilePath);

    /// <summary>
    /// 保存nut脚本配置
    /// </summary>
    /// <param name="remoteFilePath"></param>
    /// <param name="jsonContent"></param>
    void SaveJsonToRemoteFile(string remoteFilePath, string jsonContent);

    /// <summary>
    /// 卸载插件
    /// </summary>
    /// <param name="projectInfo"></param>
    /// <returns></returns>
    Task DeleteFromLinux(ProjectInfo projectInfo);

    /// <summary>
    /// 获取磁盘使用情况
    /// </summary>
    /// <returns></returns>
    Task<ObservableCollection<FileSystemUsageModel>> GetDiskUsagesAsync();

    /// <summary>
    /// 按文件类型获取使用情况
    /// </summary>
    /// <returns></returns>
    Task<List<FileTypeUsage>> GetFileTypeUsagesAsync();
}
