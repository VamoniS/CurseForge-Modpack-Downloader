using System.Text.Json.Serialization;

namespace Modloader.DataStructures;

public struct ManifestStructure
{
    [JsonPropertyName("files")]
    public ModFile[] Files { get; set; }
}
public struct ModFile
{
    [JsonPropertyName("projectID")]
    public int Projectid { get; set; }
    
    [JsonPropertyName("fileID")]
    public int Fileid { get; set; }
}