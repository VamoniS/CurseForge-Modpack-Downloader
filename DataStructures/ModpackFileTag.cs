
namespace Modloader.DataStructures;

public struct ModpackFileTag
{
    public int ModId { get; set; }
    
    public int FileId { get; set; }
    
    public string Filename { get; set; }
    
    public string DisplayName { get; set; }
}