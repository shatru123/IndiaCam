using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Core.Models;

public class DiscoveredCameraDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CategorySlug { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string SourceUrl { get; set; } = string.Empty;
    public string StreamUrl { get; set; } = string.Empty;
    public StreamType StreamType { get; set; } = StreamType.Hls;
    public string? ThumbnailUrl { get; set; }
    public string Attribution { get; set; } = string.Empty;
}
