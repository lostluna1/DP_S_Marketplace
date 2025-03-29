using CommunityToolkit.Mvvm.ComponentModel;

namespace DP_S_Marketplace.Models;

public partial class ProjectInfo : ObservableValidator
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
}
