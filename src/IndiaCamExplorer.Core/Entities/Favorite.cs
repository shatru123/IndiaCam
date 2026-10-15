namespace IndiaCamExplorer.Core.Entities;

public class Favorite
{
    public int Id { get; set; }
    public int CameraId { get; set; }
    public Camera Camera { get; set; } = null!;
    public string SessionId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
