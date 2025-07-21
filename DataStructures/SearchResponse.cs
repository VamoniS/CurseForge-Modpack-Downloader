using System.Text.Json.Serialization;

namespace Modloader.DataStructures;

public struct SearchResponse
{
    [JsonPropertyName("data")]
    public SearchData[] Data { get; set; }
}

public struct SearchData
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("downloadCount")]
    public int DownloadCount { get; set; }
    
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("logo")]
    public LogoData Logo { get; set; }
}

public struct LogoData
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}