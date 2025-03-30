using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Helpers;
using DP_S_Marketplace.Models;
using Renci.SshNet;
using DP_S_Marketplace.Core.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media;

namespace DP_S_Marketplace.Services;
public class ScriptMarketplaceService(IApiService apiService) : IScriptMarketplaceService
{
    private const string DownloadedProjectsFilePath = "/dp_s/script_info/downloaded_projects.json";

    public IApiService ApiService
    {
        get;
        set;
    } = apiService;
    private const int Port = 22;

    public async Task<string> GetRemoteConfigFileAsync(string remoteFilePath)
    {
        var globalVariables = GlobalVariables.Instance;
        var connectionInfo = globalVariables.ConnectionInfo;
        if (connectionInfo == null || string.IsNullOrEmpty(connectionInfo.Ip) || string.IsNullOrEmpty(connectionInfo.User) || string.IsNullOrEmpty(connectionInfo.Password))
        {
            return "连接信息无效";
        }

        using var sftp = new SftpClient(connectionInfo.Ip, Port, connectionInfo.User, connectionInfo.Password);
        sftp.Connect();

        // 打开远程文件的读取流
        using var readStream = sftp.OpenRead($"/dp_s/OffcialConfig/{remoteFilePath}");
        using var reader = new StreamReader(readStream);
        // 读取文件内容
        var fileContent = await reader.ReadToEndAsync();
        sftp.Disconnect();

        return fileContent;

    }
    public void SaveJsonToRemoteFile(string remoteFilePath, string jsonContent)
    {
        var globalVariables = GlobalVariables.Instance;
        var connectionInfo = globalVariables.ConnectionInfo;
        if (connectionInfo == null || string.IsNullOrEmpty(connectionInfo.Ip) || string.IsNullOrEmpty(connectionInfo.User) || string.IsNullOrEmpty(connectionInfo.Password))
        {
            throw new InvalidOperationException("连接信息无效");
        }

        using var sftp = new SftpClient(connectionInfo.Ip, Port, connectionInfo.User, connectionInfo.Password);
        sftp.Connect();

        var remoteFullPath = $"/dp_s/OffcialConfig/{remoteFilePath}";
        var remoteDirectory = Path.GetDirectoryName(remoteFullPath);

        // 确保远程目录存在
        CreateDirectoryRecursive1(sftp, remoteDirectory);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
        sftp.UploadFile(stream, remoteFullPath, true);
        GrowlMsg.Show("配置文件已成功保存到 Linux 服务器。", true);
        sftp.Disconnect();
    }



    public List<FileTypeUsage> GetFileTypeUsages()
    {
        var globalVariables = GlobalVariables.Instance;
        if (globalVariables.ConnectionInfo == null)
        {
            return [];
        }
        var connectionInfo = globalVariables.ConnectionInfo;
        if (connectionInfo.Ip is null || connectionInfo.User is null || connectionInfo.Password is null)
        {
            return [];
        }
        using var client = new SshClient(connectionInfo.Ip, Port, connectionInfo.User, connectionInfo.Password);
        client.Connect();
        var command = client.CreateCommand("find / -type f -exec du -b {} + | awk '{print $1, $2}'");
        var result = command.Execute();
        client.Disconnect();

        return ParseFileTypeUsage(result);
    }

    /// <summary>
    /// 解析 du 命令的输出，返回文件类型使用情况
    /// </summary>
    /// <param name="commandOutput"></param>
    /// <returns></returns>
    private static List<FileTypeUsage> ParseFileTypeUsage(string commandOutput)
    {
        var fileTypeUsages = new Dictionary<string, long>();
        var lines = commandOutput.Split('\n');

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(' ');
            if (parts.Length == 2)
            {
                var size = long.Parse(parts[0]);
                var filePath = parts[1];
                var fileType = GetFileType(filePath);

                if (fileTypeUsages.ContainsKey(fileType))
                {
                    fileTypeUsages[fileType] += size;
                }
                else
                {
                    fileTypeUsages[fileType] = size;
                }
            }
        }

        var result = new List<FileTypeUsage>();
        foreach (var kvp in fileTypeUsages)
        {
            result.Add(new FileTypeUsage { FileType = kvp.Key, Size = ConvertToGB(kvp.Value) });
        }

        return result;
    }

    /// <summary>
    /// 将字节大小转换为 GB 单位
    /// </summary>
    /// <param name="sizeInBytes"></param>
    /// <returns></returns>
    private static double ConvertToGB(long sizeInBytes)
    {
        return Math.Round( sizeInBytes / (1024.0 * 1024.0 * 1024.0),6);
    }

    /// <summary>
    /// 根据文件路径获取文件类型
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static string GetFileType(string filePath)
    {
        var extension = System.IO.Path.GetExtension(filePath).ToLower();
        var documentExtensions = new[] { ".txt", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx" };
        var mediaExtensions = new[] { ".mp3", ".mp4", ".avi", ".mkv", ".mov", ".jpg", ".jpeg", ".png", ".gif" };

        if (documentExtensions.Contains(extension))
        {
            return "文档类型";
        }
        else if (mediaExtensions.Contains(extension))
        {
            return "媒体类型";
        }
        else if (extension == ".nut")
        {
            return ".nut 文件";
        }
        else
        {
            return "未知类型";
        }
    }

    public ObservableCollection<FileSystemUsageModel> GetDiskUsages()
    {
        var globalVariables = GlobalVariables.Instance;
        if (globalVariables.ConnectionInfo == null)
        {
            return [];
        }
        var connectionInfo = globalVariables.ConnectionInfo;
        if (connectionInfo.Ip is null || connectionInfo.User is null || connectionInfo.Password is null)
        {
            return [];
        }
        using var client = new SshClient(connectionInfo.Ip, Port, connectionInfo.User, connectionInfo.Password);
        client.Connect();
        var command = client.CreateCommand("df -h");
        var result = command.Execute();
        client.Disconnect();

        return ParseDiskUsage(result);
    }

    /// <summary>
    /// 解析 df 命令的输出，返回文件系统使用情况
    /// </summary>
    /// <param name="dfOutput"></param>
    /// <returns></returns>
    private static ObservableCollection<FileSystemUsageModel> ParseDiskUsage(string dfOutput)
    {
        var lines = dfOutput.Split('\n').Skip(1); // Skip the header line
        var fileSystemUsages = new List<FileSystemUsageModel>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (columns.Length >= 6)
            {
                var usage = new FileSystemUsageModel
                {
                    FileSystem = columns[0],
                    Size = ParseSize(columns[1]),
                    Used = ParseSize(columns[2]),
                    Available = ParseSize(columns[3]),
                    UsePercentage = int.Parse(columns[4].TrimEnd('%')),
                    MountedOn = columns[5]
                };
                fileSystemUsages.Add(usage);
            }
        }

        return new ObservableCollection<FileSystemUsageModel>(fileSystemUsages);
    }

    /// <summary>
    /// 解析文件大小字符串，返回以字节为单位的大小
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private static long ParseSize(string size)
    {
        if (string.IsNullOrWhiteSpace(size))
        {
            return 0;
        }

        // 去掉单位部分，只保留数值部分
        var numericPart = new string(size.TakeWhile(char.IsDigit).ToArray());
        return long.TryParse(numericPart, out var result) ? result : 0;
    }

    public async Task DowloadToLinux(ProjectInfo projectInfo)
    {
        var link = projectInfo?.Raw_Url;
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

        // 定义本地临时文件夹路径
        var tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolderPath);
        // 定义远程文件路径
        var remoteProjectDirectory = $"/dp_s/OfficialProject/{projectInfo?.ProjectName}";
        var remoteConfigDirectory = "/dp_s/OffcialConfig/";
        try
        {
            // 下载项目文件
            if (projectInfo?.ProjectFiles?.Count > 0)
            {
                foreach (var item in projectInfo.ProjectFiles)
                {
                    var nutFileDownloadLink = await ApiService.PostAsync<Data>($"/api/fs/get?path=/Script/{projectInfo?.ProjectName}/{item}", null, true);
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(nutFileDownloadLink?.Data?.Raw_Url);
                    response.EnsureSuccessStatusCode();

                    var tempFilePath = Path.Combine(tempFolderPath, item);
                    using var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
                    await response.Content.CopyToAsync(fileStream);
                }
            }

            // 下载配置文件
            if (!string.IsNullOrEmpty(projectInfo?.ProjectConfig))
            {
                var nutConfigDownloadLink = await ApiService.PostAsync<Data>($"/api/fs/get?path=/Script/{projectInfo?.ProjectName}/{projectInfo?.ProjectConfig}", null, true);
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(nutConfigDownloadLink?.Data?.Raw_Url);
                    response.EnsureSuccessStatusCode();

                    var tempFilePath = Path.Combine(tempFolderPath, projectInfo?.ProjectConfig);
                    using var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
                    await response.Content.CopyToAsync(fileStream);
                }
            }

            // 下载主文件
            if (!string.IsNullOrEmpty(link))
            {
                var tempFilePath = Path.Combine(tempFolderPath, "Proj.ifo");
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(link);
                response.EnsureSuccessStatusCode();

                using var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
                await response.Content.CopyToAsync(fileStream);
            }

            // 配置 SFTP 连接信息
            var host = connectionInfo.Ip;
            var username = connectionInfo.User;
            var password = connectionInfo.Password;

            // 使用 SFTP 上传文件
            using (var sftp = new SftpClient(host, Port, username, password))
            {
                sftp.Connect();

                // 确保远程项目目录存在
                CreateDirectoryRecursive(sftp, remoteProjectDirectory);

                // 确保远程配置文件目录存在
                CreateDirectoryRecursive(sftp, remoteConfigDirectory);

                // 上传项目文件到项目目录
                foreach (var filePath in Directory.GetFiles(tempFolderPath))
                {
                    var fileName = Path.GetFileName(filePath);
                    var remoteFilePath = fileName == projectInfo?.ProjectConfig
                        ? Path.Combine(remoteConfigDirectory, fileName).Replace("\\", "/")
                        : Path.Combine(remoteProjectDirectory, fileName).Replace("\\", "/");

                    using var fileStream = new FileStream(filePath, FileMode.Open);
                    sftp.UploadFile(fileStream, remoteFilePath);
                }

                // 将已下载的项目名称存储到 JSON 文件中
                await SaveDownloadedProjectNames(sftp, new List<ProjectInfo> { projectInfo! });

                sftp.Disconnect();
            }

            GrowlMsg.Show("文件已成功上传到 Linux 服务器。", true);
            Debug.WriteLine("文件已成功上传到 Linux 服务器。");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"发生异常：{ex.Message}");
            GrowlMsg.Show($"发生异常：{ex.Message}", false);
        }
        finally
        {
            // 删除本地临时文件夹
            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
            }
        }
    }

    private void CreateDirectoryRecursive(SftpClient sftp, string path)
    {
        var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        var currentPath = "/";

        foreach (var part in parts)
        {
            currentPath = Path.Combine(currentPath, part).Replace("\\", "/");
            if (!sftp.Exists(currentPath))
            {
                sftp.CreateDirectory(currentPath);
            }
        }
    }




    public async Task<ObservableCollection<ProjectInfo>> GetServerPlugins()
    {
        try
        {
            var projectInfos = new ObservableCollection<ProjectInfo>();
            // 获取文件列表
            var pathResponse = await ApiService.PostAsync<Data>("/api/fs/list?path=/Script", null, true);
            var dataList = pathResponse.Data;

            if (dataList?.Content != null && dataList.Content.Count > 0)
            {
                foreach (var item in dataList.Content)
                {
                    var fileName = item.Name;

                    // 获取文件信息，得到下载链接
                    var response = await ApiService.PostAsync<Data>($"/api/fs/get?path=/Script/{fileName}/Proj.ifo&password=", null, true);
                    var data = response.Data;
                    var link = data?.Raw_Url;

                    if (!string.IsNullOrEmpty(link))
                    {
                        // 使用 ApiService 获取文件内容
                        var fileContent = await ApiService.GetStringAsync(link, true);

                        // 反序列化为 ProjectInfo 对象
                        var projectInfo = await Json.ToObjectAsync<ProjectInfo>(fileContent);
                        projectInfo.Raw_Url = link;
                        var a = new ProjectInfo
                        {
                            ProjectName = "123",
                            ProjectVersion = 1.0f,
                            FilePath = "123",
                            Raw_Url = "http://103.36.223.176:5244/p/DP_S/InfoDB/%E5%8F%B2%E8%AF%97%E8%8D%AF%E5%89%82.info?sign=Fojch8FHRNaz9yHTCZ_50rb9zOJr2AhaIp9ldp15qBU=:0",
                        };
                        projectInfos.Add(projectInfo);
                        //projectInfos.Add(a);
                        //projectInfos.Add(a);
                        //projectInfos.Add(a);
                    }
                    else
                    {
                        GrowlMsg.Show("下载链接无效。", false);
                    }
                }
                return projectInfos;
            }
            else
            {
                GrowlMsg.Show("未找到任何文件。", false);
            }
        }
        catch (Exception ex)
        {
            // 输出详细的异常信息
            GrowlMsg.Show($"发生异常：{ex.Message}", false);
        }
        return [];
    }

    public async Task<List<ProjectInfo>> GetServerPluginVersion()
    {
        var globalVariables = GlobalVariables.Instance;
        if (globalVariables.ConnectionInfo == null)
        {
            return [];
        }
        var connectionInfo = globalVariables.ConnectionInfo;
        if (connectionInfo.Ip is null || connectionInfo.User is null || connectionInfo.Password is null)
        {
            return [];
        }
        using var client = new SshClient(connectionInfo.Ip, Port, connectionInfo.User, connectionInfo.Password);
        client.Connect();

        // 检查文件是否存在，存在则读取内容，否则返回空字符串
        var command = client.CreateCommand($"if [ -f \"{DownloadedProjectsFilePath}\" ]; then cat \"{DownloadedProjectsFilePath}\"; else echo ''; fi");
        var result = command.Execute();
        client.Disconnect();

        if (string.IsNullOrWhiteSpace(result))
        {
            // 文件不存在或内容为空，返回空的 List<ProjectInfo>
            return [];
        }
        else
        {
            // 文件存在且有内容，反序列化 JSON
            return await Json.ToObjectAsync<List<ProjectInfo>>(result);
        }
    }

    /// <summary>
    /// 将下载的项目名称保存到 JSON 文件中
    /// </summary>
    /// <param name="sftp"></param>
    /// <param name="projectInfos"></param>
    /// <returns></returns>

    private static async Task SaveDownloadedProjectNames(SftpClient sftp, IEnumerable<ProjectInfo> projectInfos)
    {
        if (projectInfos == null || !projectInfos.Any())
        {
            return;
        }

        var downloadedProjects = new List<ProjectInfo>();

        if (sftp.Exists(DownloadedProjectsFilePath))
        {
            using var stream = new MemoryStream();
            sftp.DownloadFile(DownloadedProjectsFilePath, stream);
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var json = await reader.ReadToEndAsync();
            downloadedProjects = JsonSerializer.Deserialize<List<ProjectInfo>>(json) ?? new List<ProjectInfo>();
        }

        foreach (var projectInfo in projectInfos)
        {
            var existingProject = downloadedProjects.FirstOrDefault(p => p.ProjectName == projectInfo.ProjectName && p.ProjectVersion == projectInfo.ProjectVersion);

            if (existingProject == null)
            {
                downloadedProjects.Add(projectInfo);
            }

            // 将 ProjectInfo 对象保存到 JSON 文件中
            var projectInfoJson = JsonSerializer.Serialize(projectInfo, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 允许直接输出非 ASCII 字符
            });

            var projectInfoFilePath = Path.Combine(Path.GetDirectoryName(DownloadedProjectsFilePath) ?? string.Empty, $"{projectInfo.ProjectName}_{projectInfo.ProjectVersion}.json").Replace("\\", "/");

            // 确保远程目录存在
            CreateDirectoryRecursive1(sftp, Path.GetDirectoryName(projectInfoFilePath));

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(projectInfoJson));
            sftp.UploadFile(stream, projectInfoFilePath, true);
        }

        var updatedJson = JsonSerializer.Serialize(downloadedProjects, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 允许直接输出非 ASCII 字符
        });

        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(updatedJson)))
        {
            sftp.UploadFile(stream, DownloadedProjectsFilePath, true);
        }
    }

    private static void CreateDirectoryRecursive1(SftpClient sftp, string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        var currentPath = "/";

        foreach (var part in parts)
        {
            currentPath = Path.Combine(currentPath, part).Replace("\\", "/");
            if (!sftp.Exists(currentPath))
            {
                sftp.CreateDirectory(currentPath);
            }
        }
    }



}
public class FileTypeUsage
{
    public string? FileType
    {
        get; set;
    }
    public double Size
    {
        get; set;
    } // 使用 double 类型来表示 GB 单位的大小
}
