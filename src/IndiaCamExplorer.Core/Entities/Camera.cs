using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Core.Entities;

public class Camera
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public string SourceUrl { get; set; } = string.Empty;
    public string StreamUrl { get; set; } = string.Empty;
    public StreamType StreamType { get; set; } = StreamType.Hls;

    public string? ThumbnailUrl { get; set; }
    public string Attribution { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public bool IsVerified { get; set; } = true;
    public HealthStatus HealthStatus { get; set; } = HealthStatus.Unknown;

    public DateTime? LastCheckedAt { get; set; }
    public DateTime? LastVerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CameraHealthCheck> HealthChecks { get; set; } = new List<CameraHealthCheck>();
    public ICollection<BrokenStreamReport> Reports { get; set; } = new List<BrokenStreamReport>();
}
