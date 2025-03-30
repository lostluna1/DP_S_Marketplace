using CommunityToolkit.Mvvm.ComponentModel;

namespace DP_S_Marketplace.Models;

public partial class ProjectInfo : ObservableObject
{

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
