using CommunityToolkit.Mvvm.ComponentModel;

namespace DP_S_Marketplace.Models;

/// <summary>
/// 表示文件系统的使用情况。
/// </summary>
public partial class FileSystemUsageModel : ObservableObject
{
    /// <summary>
    /// 获取或设置文件系统的名称或设备名称。
    /// </summary>
    /// 
    [ObservableProperty]
    public partial string? FileSystem
    {
        get; set;
    }

    /// <summary>
    /// 获取或设置文件系统的总大小（以字节为单位）。
    /// </summary>
    [ObservableProperty]
    public partial long Size
    {
        get; set;
    }

    /// <summary>
    /// 获取或设置已使用的空间大小（以字节为单位）。
    /// </summary>
    [ObservableProperty]
    public partial long Used
    {
        get; set;
    }

    /// <summary>
    /// 获取或设置可用的空间大小（以字节为单位）。
    /// </summary>
    [ObservableProperty]
    public partial long Available
    {
        get; set;
    }

    /// <summary>
    /// 获取或设置已使用空间的百分比。
    /// </summary>
    [ObservableProperty]
    public partial int UsePercentage
    {
        get; set;
    }

    /// <summary>
    /// 获取或设置文件系统挂载的挂载点。
    /// </summary>
    [ObservableProperty]
    public partial string? MountedOn
    {
        get; set;
    }
}
