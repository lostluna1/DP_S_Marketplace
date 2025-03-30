using DP_S_Marketplace.Models;

namespace DP_S_Marketplace.Contracts.Services;
public interface IScriptInstaller
{
    /// <summary>
    /// 获取最新版本信息并将其写入到Linux服务器的用户根目录下的DP-S服务端插件.json文件中
    /// </summary>
    /// <returns></returns>
    Task<DP_SVersion> GetLatestDP_SVersionInfo();

    /// <summary>
    /// 获取已安装的版本信息
    /// </summary>
    /// <returns></returns>
    Task<DP_SVersion> GetInstalledVersion();

}
