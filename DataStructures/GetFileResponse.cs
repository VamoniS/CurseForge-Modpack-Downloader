
using System.Text.Json.Serialization;

namespace Modloader.DataStructures;


class GetFileResponse
{
    [JsonPropertyName("data")]
    public GetFileData? Data { get; set; }
}

class GetFileData
{
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
    
    [JsonPropertyName("modules")]
    public Module[]? Modules { get; set; }
}

public class Module
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}