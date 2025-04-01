using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Core.Helpers;
using DP_S_Marketplace.Models;
using Renci.SshNet;
using System.Text.RegularExpressions;
using DP_S_Marketplace.Helpers;

namespace DP_S_Marketplace.Services;
public partial class ScriptInstaller : IScriptInstaller
{
    public IApiService ApiService
    {
        get;
    }
    public ScriptInstaller(IApiService apiService)
    {
        ApiService = apiService;
    }

    public async Task EditCurrentRunScriptLinux()
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

        var port = 22;
        var tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolderPath);
        var tempFilePath = Path.Combine(tempFolderPath, "run");

        try
        {
            using var sftp = new SftpClient(host, port, username, password);
            sftp.Connect();

            var remoteFilePath = $"/{username}/run";

            // 1. 下载
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                sftp.DownloadFile(remoteFilePath, fileStream);
            }

            // 2. 读取并修改脚本
            string fileContent;
            using (var reader = new StreamReader(tempFilePath))
            {
                fileContent = await reader.ReadToEndAsync();
            }
            var scriptLines = fileContent.Split('\n').ToList();
            var sleepRegex = new Regex(@"^\s*sleep\s+\d+\s*$");
            var dfGameRegex = new Regex(@"\./df_game_r");

            for (int i = 0; i < scriptLines.Count - 1; i++)
            {
                // 跳过注释行
                if (scriptLines[i].TrimStart().StartsWith('#'))
                {
                    continue;
                }

                // 若匹配到 sleep 行
                if (sleepRegex.IsMatch(scriptLines[i]))
                {
                    // 查找下一个非注释行
                    int nextLineIndex = i + 1;
                    while (nextLineIndex < scriptLines.Count && scriptLines[nextLineIndex].TrimStart().StartsWith('#'))
                    {
                        nextLineIndex++;
                    }

                    if (nextLineIndex >= scriptLines.Count)
                    {
                        // 已到达文件末尾，停止处理
                        break;
                    }

                    var nextLine = scriptLines[nextLineIndex];

                    if (dfGameRegex.IsMatch(nextLine))
                    {
                        // 如果下一行包含 ./df_game_r，处理该行
                        var ldPreloadFixRegex = new Regex(
                            @"^\s*LD_PRELOAD\s*=\s*(""(?<sos>[^""]*)""|(?<sos>[^\s]+))\s+(?<cmd>.+)$",
                            RegexOptions.IgnoreCase
                        );

                        if (ldPreloadFixRegex.IsMatch(nextLine))
                        {
                            var match = ldPreloadFixRegex.Match(nextLine);
                            var originalSoList = match.Groups["sos"].Value.Trim();
                            var cmdPart = match.Groups["cmd"].Value.Trim();

                            if (!originalSoList.Contains("/dp_s/lib/libAurora.so"))
                            {
                                originalSoList = "/dp_s/lib/libAurora.so " + originalSoList;
                            }

                            nextLine = $@"LD_PRELOAD=""{originalSoList}"" {cmdPart}";
                        }
                        else
                        {
                            nextLine = $@"LD_PRELOAD=""/dp_s/lib/libAurora.so"" {nextLine.Trim()}";
                        }

                        // 更新脚本行
                        scriptLines[nextLineIndex] = nextLine;
                    }
                    // 更新 i 的值，跳过已处理的行
                    i = nextLineIndex - 1;
                }
            }

            var newScript = string.Join('\n', scriptLines);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(newScript));
            sftp.UploadFile(stream, remoteFilePath, true);

            sftp.Disconnect();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"发生异常：{ex.Message}");
        }
        finally
        {
            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
            }
        }
    }




    public async Task<DP_SVersion> GetInstalledVersion()
    {
        var globalVariables = GlobalVariables.Instance;
        if (globalVariables.ConnectionInfo == null)
        {
            return new DP_SVersion();
        }

        var connectionInfo = globalVariables.ConnectionInfo;
        if (connectionInfo.Ip is null || connectionInfo.User is null || connectionInfo.Password is null)
        {
            return new DP_SVersion();
        }

        var host = connectionInfo.Ip;
        var username = connectionInfo.User;
        var password = connectionInfo.Password;

        // 定义本地临时文件夹路径
        var tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolderPath);
        var tempFilePath = Path.Combine(tempFolderPath, "DP-S服务端插件.json");

        try
        {
            // 将 SFTP 操作放到后台线程中执行，避免在当前线程上阻塞
            await Task.Run(() =>
            {
                using var sftp = new SftpClient(host, 22, username, password);
                sftp.Connect();

                var remoteFilePath = $"/{username}/DP-S服务端插件.json";

                if (!sftp.Exists(remoteFilePath))
                {
                    Debug.WriteLine($"远程文件不存在：{remoteFilePath}");
                    // 在多线程场景中被迫中止时无法直接返回，只能通过抛异常或设置信号量
                    throw new FileNotFoundException($"远程文件不存在：{remoteFilePath}");
                }

                // 下载远程文件到本地
                using var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
                sftp.DownloadFile(remoteFilePath, fileStream);

                sftp.Disconnect();
            })
            .ConfigureAwait(false);

            // 异步读取下载后的文件
            string fileContent;
            using (var reader = new StreamReader(tempFilePath))
            {
                fileContent = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            // 反序列化为 DP_SVersion 对象
            var data = await Json.ToObjectAsync<DP_SVersion>(fileContent).ConfigureAwait(false);
            return data;
        }
        catch (FileNotFoundException fnfEx)
        {
            Debug.WriteLine(fnfEx.Message);
            return new DP_SVersion();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"发生异常：{ex.Message}");
            return new DP_SVersion();
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



    public async Task<DP_SVersion> GetLatestDP_SVersionInfo()
    {
        var globalVariables = GlobalVariables.Instance;
        if (globalVariables.ConnectionInfo == null)
        {
            return new DP_SVersion();
        }
        var connectionInfo = globalVariables.ConnectionInfo;
        if (connectionInfo.Ip is null || connectionInfo.User is null || connectionInfo.Password is null)
        {
            return new DP_SVersion();
        }


        // 定义本地临时文件夹路径
        var tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolderPath);
        var DP_SVersionInfoDownloadLink = await ApiService.PostAsync<Data>($"/api/fs/get?path=/Application/Proj.ifo", null, true);
        var url = DP_SVersionInfoDownloadLink?.Data?.Raw_Url;
        if (url is not null)
        {
            var tempFilePath = Path.Combine(tempFolderPath, "Proj.ifo");
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
                await response.Content.CopyToAsync(fileStream);
            }

            // 读取文件内容
            string fileContent;

            using var reader = new StreamReader(tempFilePath);
            fileContent = await reader.ReadToEndAsync();
            var data = await Json.ToObjectAsync<DP_SVersion>(fileContent);

            //// 使用 SFTP 将版本信息文件上传到用户的 Linux 服务器
            //using (var sftp = new SftpClient(host, 22, username, password))
            //{
            //    sftp.Connect();

            //    var projectInfoJson = JsonSerializer.Serialize(data, new JsonSerializerOptions
            //    {
            //        WriteIndented = true,
            //        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 允许直接输出非 ASCII 字符
            //    });

            //    // 将文件上传到用户的 Linux 根目录
            //    var projectInfoFilePath = $"/{username}/{data.ProjectName}.json";

            //    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(projectInfoJson));
            //    sftp.UploadFile(stream, projectInfoFilePath, true);

            //    sftp.Disconnect();
            //}

            return data;

        }
        else
        {
            Debug.WriteLine("下载链接无效。");
        }
        return new DP_SVersion();
    }


}
