using CommunityToolkit.Mvvm.ComponentModel;

namespace DP_S_Marketplace.Models;
public enum PluginStatus
{
    NotInstalled,   // 未安装
    CanUpdate,      // 可以更新
    LatestVersion   // 已是最新版本
}
public partial class ProjectInfo : ObservableObject
{
    [ObservableProperty]
    public partial PluginStatus Status
    {
        get;set;
    }

    [ObservableProperty]
    public partial string? FilePath
    {
        get; set;
    }

    [ObservableProperty]
    public partial float? ProjectVersion
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? ProjectName
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? ProjectAuthor
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? Raw_Url
    {
        get; set;
    }

    public List<string>? ProjectFiles
    {
        get;
        set;
    }

    public string? ProjectConfig
    {
        get;
        set;
    }
    public string? ProjectDescribe
    {
        get;
        set;
    }
    public string? ProjectRunFunc
    {
        get;
        set;
    }
}
