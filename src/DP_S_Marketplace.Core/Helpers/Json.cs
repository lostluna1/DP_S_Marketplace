using System.Text.Json;

namespace DP_S_Marketplace.Core.Helpers;

public static class Json
{
    // 配置序列化选项
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true, // 忽略属性名的大小写
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyProperties = true,    // 忽略只读属性
        IgnoreReadOnlyFields = true,        // 忽略只读字段
        AllowTrailingCommas = true,         // 允许结尾的逗号
        WriteIndented = false,              // 缩进格式化输出

    };

    public static async Task<T> ToObjectAsync<T>(string value)
    {
        return await Task.Run(() =>
        {
            return JsonSerializer.Deserialize<T>(value, _options);
        });
    }

    public static async Task<string> StringifyAsync(object value)
    {
        return await Task.Run(() =>
        {
            return JsonSerializer.Serialize(value, _options);
        });
    }
}
