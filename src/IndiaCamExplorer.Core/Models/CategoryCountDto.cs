namespace IndiaCamExplorer.Core.Models;

public class CategoryCountDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconName { get; set; } = string.Empty;
    public int CameraCount { get; set; }
    public int OnlineCount { get; set; }
}
