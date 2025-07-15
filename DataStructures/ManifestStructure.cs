

using System.Text.Json.Serialization;

namespace Modloader.DataStructures;


public class ManifestStructure
{
    [JsonPropertyName("files")]
    public ModFile[]? Files { get; set; }
}
public class ModFile
{
    [JsonPropertyName("projectID")]
    public int? ProjectID { get; set; }
    [JsonPropertyName("fileID")]
    public int? FileID { get; set; }
}