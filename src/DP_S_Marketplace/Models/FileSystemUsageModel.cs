using CommunityToolkit.Mvvm.ComponentModel;

namespace DP_S_Marketplace.Models;

/// <summary>
/// ��ʾ�ļ�ϵͳ��ʹ�������
/// </summary>
public partial class FileSystemUsageModel : ObservableObject
{
    /// <summary>
    /// ��ȡ�������ļ�ϵͳ�����ƻ��豸���ơ�
    /// </summary>
    /// 
    [ObservableProperty]
    public partial string? FileSystem
    {
        get; set;
    }

    /// <summary>
    /// ��ȡ�������ļ�ϵͳ���ܴ�С�����ֽ�Ϊ��λ����
    /// </summary>
    [ObservableProperty]
    public partial long Size
    {
        get; set;
    }

    /// <summary>
    /// ��ȡ��������ʹ�õĿռ��С�����ֽ�Ϊ��λ����
    /// </summary>
    [ObservableProperty]
    public partial long Used
    {
        get; set;
    }

    /// <summary>
    /// ��ȡ�����ÿ��õĿռ��С�����ֽ�Ϊ��λ����
    /// </summary>
    [ObservableProperty]
    public partial long Available
    {
        get; set;
    }

    /// <summary>
    /// ��ȡ��������ʹ�ÿռ�İٷֱȡ�
    /// </summary>
    [ObservableProperty]
    public partial int UsePercentage
    {
        get; set;
    }

    /// <summary>
    /// ��ȡ�������ļ�ϵͳ���صĹ��ص㡣
    /// </summary>
    [ObservableProperty]
    public partial string? MountedOn
    {
        get; set;
    }
}
