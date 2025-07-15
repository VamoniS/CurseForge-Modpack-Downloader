

using System.Text.Json.Serialization;

namespace Modloader.DataStructures;


class SearchResponse
{
    [JsonPropertyName("data")]
    public SearchData[]? Data { get; set; }
}

class SearchData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("downloadCount")]
    public int? DownloadCount { get; set; }
    [JsonPropertyName("id")]
    public int? Id { get; set; }
}