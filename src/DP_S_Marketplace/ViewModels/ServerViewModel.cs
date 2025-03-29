using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Models;
using DP_S_Marketplace.Services;
using Microsoft.UI.Xaml;

namespace DP_S_Marketplace.ViewModels;

public partial class ServerViewModel : ObservableRecipient
{
    public IScriptMarketplaceService ScriptMarketplaceService
    {
        get;
    }
    public IApiService ApiService
    {
        get;
    }

    [ObservableProperty]
    public partial ObservableCollection<ProjectInfo> ProjectInfos { get; set; } = new();

    [ObservableProperty]
    public partial ObservableCollection<FileSystemUsageModel> FileSystemUsages { get; set; } = new();

    [ObservableProperty]
    public partial FileSystemUsageModel? MainDisks
    {
        get; set;
    }

    [ObservableProperty]
    public partial FileTypeUsage? FileTypeUsages_Documents
    {
        get; set;
    }

    [ObservableProperty]
    public partial FileTypeUsage? FileTypeUsages_Videos
    {
        get; set;
    }

    [ObservableProperty]
    public partial FileTypeUsage? FileTypeUsages_Nut
    {
        get; set;
    }

    [ObservableProperty]
    public partial FileTypeUsage? FileTypeUsages_Unkown
    {
        get; set;
    }

    [ObservableProperty]
    public partial double AnimationValue { get; set; } = 0;

    private readonly DispatcherTimer _timer;
    private double _animationStartValue;
    private double _animationEndValue;
    private readonly double _animationDuration = 0.4; 
    private DateTime _animationStartTime; // 动画开始时间
    private double _animationElapsedTime; // 动画已运行的时间

    public ServerViewModel(IApiService apiService, IScriptMarketplaceService scriptMarketplaceService)
    {
        ApiService = apiService;
        ScriptMarketplaceService = scriptMarketplaceService;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; // 60 FPS
        _timer.Tick += OnAnimationTick;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await GetInstalledServerPlugins();
        await GetDiskUsagesAsync();
        await GetFileTypeUsagesAsync();
    }

    [RelayCommand]
    public async Task GetInstalledServerPlugins()
    {
        var plugins = await ScriptMarketplaceService.GetServerPluginVersion();
        ProjectInfos = new ObservableCollection<ProjectInfo>(plugins);
    }

    public async Task GetDiskUsagesAsync()
    {
        var diskUsages = await Task.Run(() => ScriptMarketplaceService.GetDiskUsages());
        FileSystemUsages = new ObservableCollection<FileSystemUsageModel>(diskUsages);
        MainDisks = diskUsages.FirstOrDefault(a => a.FileSystem == "/dev/sda3");
        StartAnimation(0, MainDisks?.Used ?? 0);
    }

    public async Task GetFileTypeUsagesAsync()
    {
        var fileTypeUsages = await Task.Run(() => ScriptMarketplaceService.GetFileTypeUsages());
        FileTypeUsages_Documents = fileTypeUsages.FirstOrDefault(a => a.FileType == "文档类型");
        FileTypeUsages_Videos = fileTypeUsages.FirstOrDefault(a => a.FileType == "媒体类型");
        FileTypeUsages_Nut = fileTypeUsages.FirstOrDefault(a => a.FileType == ".nut 文件");
        FileTypeUsages_Unkown = fileTypeUsages.FirstOrDefault(a => a.FileType == "未知类型");
    }

    private void StartAnimation(double from, double to)
    {
        _animationStartValue = from;
        _animationEndValue = to;
        _animationStartTime = DateTime.Now; // 记录开始时间
        _timer.Start();
    }

    private void OnAnimationTick(object sender, object e)
    {
        _animationElapsedTime = (DateTime.Now - _animationStartTime).TotalSeconds; // 计算已用时间

        if (_animationElapsedTime >= _animationDuration)
        {
            AnimationValue = _animationEndValue;
            _timer.Stop();
        }
        else
        {
            var progress = _animationElapsedTime / _animationDuration;
            AnimationValue = _animationStartValue + (progress * (_animationEndValue - _animationStartValue));
        }
        Debug.WriteLine($"AnimationValue: {AnimationValue}, ElapsedTime: {_animationElapsedTime}");
    }
}
