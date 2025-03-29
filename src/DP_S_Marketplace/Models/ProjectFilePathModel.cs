using System.Text.Json.Serialization;

namespace DP_S_Marketplace.Models;
public class Data
{
    [JsonPropertyName("content")]
    public List<ContentItem>? Content
    {
        get; set;
    }


    public string? Name
    {
        get; set;
    }
    public long Size
    {
        get; set;
    }


    public string? Sign
    {
        get; set;
    }
    public string? Thumb
    {
        get; set;
    }
    public int Type
    {
        get; set;
    }
    public string? Hashinfo
    {
        get; set;
    }

    //[JsonPropertyName("hash_info")]
    //public object? HashInfo
    //{
    //    get; set;
    //}
    public string? Raw_Url
    {
        get; set;
    }

    public object? Related
    {
        get; set;
    }
}
public class ContentItem
{
    public string? Name
    {
        get; set;
    }
    public long Size
    {
        get; set;
    }

}