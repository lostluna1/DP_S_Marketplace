using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DP_S_Marketplace.Models;

public partial class ApiResponse<T> : ObservableValidator
{

    [ObservableProperty]
    public partial int Code
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial string? Message
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial T? Data
    {
        get;
        set;
    }
}