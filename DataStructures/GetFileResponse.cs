using System.Text.Json.Serialization;

namespace Modloader.DataStructures;

public struct GetFileResponse
{
    [JsonPropertyName("data")]
    public GetFileData Data { get; set; }
}

public struct GetFileData
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }
    
    [JsonPropertyName("modules")]
    public Module[] Modules { get; set; }
}

public struct Module
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}