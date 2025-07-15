using System.Text.Json.Serialization;

namespace Modloader.DataStructures;

public class GetFilesResponse
{
    [JsonPropertyName("data")]
    public GetFilesData[]? Data { get; set; }
}

public class GetFilesData
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }
    
    [JsonPropertyName("modId")]
    public int? ModId { get; set; }
    
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }
    
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
}