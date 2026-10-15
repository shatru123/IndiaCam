using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Core.Models;

public class CameraMarkerDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public StreamType StreamType { get; set; }
    public HealthStatus HealthStatus { get; set; }
    public string? ThumbnailUrl { get; set; }
}
