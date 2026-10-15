using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Core.Entities;

public class CameraHealthCheck
{
    public int Id { get; set; }
    public int CameraId { get; set; }
    public Camera Camera { get; set; } = null!;

    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public HealthStatus Status { get; set; }
    public long ResponseTimeMs { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? ErrorMessage { get; set; }
}
