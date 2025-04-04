﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Helpers;
using DP_S_Marketplace.Models;
using DP_S_Marketplace.Services;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Renci.SshNet;

namespace DP_S_Marketplace.ViewModels;

public partial class ServerViewModel : ObservableRecipient
{
    public IScriptMarketplaceService ScriptMarketplaceService
    {
        get;
    }

    public IScriptInstaller ScriptInstaller
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
    public partial ProjectInfo? SlectedProjectInfo { get; set; } = new();

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
    public partial DP_SVersion? DP_SVersion
    {
        get; set;
    }

    [ObservableProperty]
    public partial string? InstalledVersion
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial string? InstallBtnContent
    {
        get; set;
    } 

    /// <summary>
    /// 要编辑的脚本配置文件内容
    /// </summary>
    [ObservableProperty]
    public partial string? EditConfigFile
    {
        get;
        set;
    }
    /// <summary>
    /// 获取或设置安装进度
    /// </summary>
    [ObservableProperty]
    public partial double InstallProgressValue
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial string? OperationType
    {
        get;
        set;
    }

    public ServerViewModel(IApiService apiService, IScriptMarketplaceService scriptMarketplaceService, IScriptInstaller scriptInstaller)
    {
        ApiService = apiService;
        ScriptInstaller = scriptInstaller;
        ScriptMarketplaceService = scriptMarketplaceService;
        //_ = InitializeAsync();
    }


    /// <summary>
    /// 取得脚本配置文件内容
    /// </summary>
    /// <param name="projectInfo"></param>
    /// <returns></returns>
    public async Task<string?> GetConfigContentAsync(ProjectInfo? projectInfo)
    {
        if (projectInfo is not null && projectInfo.ProjectConfig is not null)
        {
            //_= ScriptInstaller.EditCurrentRunScriptLinux();
            var a = await ScriptMarketplaceService.GetRemoteConfigFileAsync(projectInfo.ProjectConfig);
            return a;
        }
        else
        {
            //throw new Exception("该插件没有配置信息");
            return null;

        }
    }

    [RelayCommand]
    public void SaveConfigFile()
    {
        var a = SlectedProjectInfo;
        if (a is not null && a.ProjectConfig is not null && EditConfigFile is not null)
        {
            if (IsValidJson(EditConfigFile))
            {
                ScriptMarketplaceService.SaveJsonToRemoteFile(a.ProjectConfig, EditConfigFile);
            }
            else
            {
                GrowlMsg.Show("配置文件内容不是合法的 JSON 格式。", false);
            }
        }
    }

    /// <summary>
    /// 验证字符串是否为合法的 JSON 格式
    /// </summary>
    /// <param name="strInput"></param>
    /// <returns></returns>
    private static bool IsValidJson(string strInput)
    {
        strInput = strInput.Trim();
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || // 对象
            (strInput.StartsWith("[") && strInput.EndsWith("]")))   // 数组
        {
            try
            {
                var obj = JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public async Task DeleteFromLinux()
    {
        if (SlectedProjectInfo is not null)
        {
            await ScriptMarketplaceService.DeleteFromLinux(SlectedProjectInfo);
            ProjectInfos.Remove(SlectedProjectInfo);
        }
    }

    [RelayCommand]
    public async Task DownloadDPS(string operationType)
    {
        var globalVariables = GlobalVariables.Instance;
        if (globalVariables.ConnectionInfo == null)
        {
            return;
        }
        var connectionInfo = globalVariables.ConnectionInfo;
        if (connectionInfo.Ip is null || connectionInfo.User is null || connectionInfo.Password is null)
        {
            return;
        }

        var host = connectionInfo.Ip;
        var username = connectionInfo.User;
        var password = connectionInfo.Password;

        // 定义本地临时文件夹路径
        var tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolderPath);
        var tempFilePath = Path.Combine(tempFolderPath, "dp_s.tar");

        var DP_SDownloadLink = await ApiService.PostAsync<Data>($"/api/fs/get?path=/Application/dp_s.tar", null, true);
        var url = DP_SDownloadLink?.Data?.Raw_Url;
        if (url is not null)
        {
            using (var httpClient = new HttpClient())
            {
                using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
                var totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault();
                var buffer = new byte[8192];
                var bytesRead = 0;
                var totalRead = 0;

                using var contentStream = await response.Content.ReadAsStreamAsync();
                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    totalRead += bytesRead;
                    Debug.WriteLine($"下载进度: {totalRead}/{totalBytes} ({(totalRead * 100.0 / totalBytes):F2}%)");
                    InstallProgressValue = totalRead * 100.0 / totalBytes;
                }
                InstallProgressValue = 100;
                if (InstallProgressValue==100)
                {
                    InstallBtnContent = "重新安装";
                }
            }

            // 使用 SFTP 将文件上传到服务器的临时目录，例如 /tmp
            using (var sftp = new SftpClient(host, 22, username, password))
            {
                sftp.Connect();

                using (var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read))
                {
                    var remoteFilePath = $"/tmp/dp_s.tar";
                    sftp.UploadFile(fileStream, remoteFilePath, true);
                }

                sftp.Disconnect();
            }

            // 使用 SSH 解压文件到根目录的上一层，并设置权限
            using (var sshClient = new SshClient(host, username, password))
            {
                sshClient.Connect();

                // 获取根目录的上一层路径（通常为根目录）
                var parentDirCommand = sshClient.CreateCommand("dirname /");
                var parentDir = parentDirCommand.Execute().Trim();

                // 检查并删除已存在的 dp_s 文件夹
                if (operationType == "下载")
                {
                    var removeDpSCommand = sshClient.CreateCommand($"if [ -d \"{parentDir}dp_s\" ]; then sudo rm -rf \"{parentDir}dp_s\"; fi");
                    removeDpSCommand.Execute(); 
                }

                // 解压文件到父目录
                var extractCommand = sshClient.CreateCommand($"sudo tar -xvf /tmp/dp_s.tar -C \"{parentDir}\"");
                var result = extractCommand.Execute();
                var exitStatus = extractCommand.ExitStatus;

                if (exitStatus == 0)
                {
                    Debug.WriteLine("解压成功");
                    Debug.WriteLine($"解压结果:\n{result}");

                    // 设置权限，递归修改解压后的文件夹权限为 777
                    var chmodCommand = sshClient.CreateCommand($"sudo chmod -R 777 \"{parentDir}dp_s\"");
                    chmodCommand.Execute();

                    Debug.WriteLine("权限设置成功");
                }
                else
                {
                    Debug.WriteLine($"解压失败，退出状态码：{exitStatus}");
                    GrowlMsg.Show($"解压失败，退出状态码：{exitStatus}", false);
                    sshClient.Disconnect();
                    return;
                }

                // 删除临时文件
                sshClient.RunCommand($"sudo rm /tmp/dp_s.tar");

                sshClient.Disconnect();
            }

            DP_SVersion = await ScriptInstaller.GetLatestDP_SVersionInfo();
            
            InstalledVersion = DP_SVersion.ProjectVersion.ToString();

            // 使用 SFTP 将版本信息文件上传到用户的 Linux 服务器
            
                using var sftp1 = new SftpClient(host, 22, username, password);
                sftp1.Connect();

                var projectInfoJson = System.Text.Json.JsonSerializer.Serialize(DP_SVersion, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 允许直接输出非 ASCII 字符
                });

                // 将文件上传到用户的 Linux 根目录
                var projectInfoFilePath = $"/{username}/{DP_SVersion.ProjectName}.json";

                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(projectInfoJson));
                sftp1.UploadFile(stream, projectInfoFilePath, true);

                sftp1.Disconnect();
            

            GrowlMsg.Show($"成功安装DP_S: {InstalledVersion}", true);
            Debug.WriteLine("文件已成功上传并解压到 Linux 服务器。");
        }
        else
        {
            Debug.WriteLine("下载链接无效。");
            GrowlMsg.Show("下载链接无效。", false);
        }

        // 删除本地临时文件夹
        if (Directory.Exists(tempFolderPath))
        {
            Directory.Delete(tempFolderPath, true);
        }

        // 修改 run 脚本以运行 DP_S
        await ScriptInstaller.EditCurrentRunScriptLinux();
    }


    public async Task InitializeAsync()
    {
        var latestVersion = await ScriptInstaller.GetLatestDP_SVersionInfo();
        var a = await ScriptInstaller.GetInstalledVersion();
        if (a.ProjectVersion == 0)
        {
            InstalledVersion = "未安装";
            InstallBtnContent = "一键安装";
            OperationType = "下载";
        }
        else if (latestVersion.ProjectVersion > a.ProjectVersion)
        {
            InstallBtnContent = "更新";
            OperationType = "更新";
            //InstalledVersion = "";
        }
        else
        {
            InstalledVersion = a.ProjectVersion.ToString();
            InstallBtnContent = "重新安装";
            OperationType = "下载";
        }
        await GetInstalledServerPlugins();
        await GetDiskUsagesAsync();
        await GetFileTypeUsagesAsync();

        
    }

    [RelayCommand]
    public async Task GetInstalledServerPlugins()
    {
        var plugins = await ScriptMarketplaceService.GetServerPluginVersion();
        ProjectInfos = [.. plugins];
    }

    public async Task GetDiskUsagesAsync()
    {
        var diskUsages = await Task.Run(() => ScriptMarketplaceService.GetDiskUsagesAsync());
        FileSystemUsages = diskUsages;
        MainDisks = diskUsages.FirstOrDefault(a => a.FileSystem == "/dev/sda3");

    }

    public async Task GetFileTypeUsagesAsync()
    {
        var fileTypeUsages = await Task.Run(() => ScriptMarketplaceService.GetFileTypeUsagesAsync());
        FileTypeUsages_Documents = fileTypeUsages.FirstOrDefault(a => a.FileType == "文档类型");
        FileTypeUsages_Videos = fileTypeUsages.FirstOrDefault(a => a.FileType == "媒体类型");
        FileTypeUsages_Nut = fileTypeUsages.FirstOrDefault(a => a.FileType == ".nut 文件");
        FileTypeUsages_Unkown = fileTypeUsages.FirstOrDefault(a => a.FileType == "未知类型");
    }

}
