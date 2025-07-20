using System.Text.Json.Serialization;

namespace Modloader.DataStructures;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(GetFileResponse))]
[JsonSerializable(typeof(GetFilesResponse))]
[JsonSerializable(typeof(ManifestStructure))]
[JsonSerializable(typeof(ModpackFileTag))]
[JsonSerializable(typeof(SearchResponse))]
public partial class DataStructuresContext : JsonSerializerContext;