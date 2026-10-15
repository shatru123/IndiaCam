using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Core.Entities;

public class BrokenStreamReport
{
    public int Id { get; set; }
    public int CameraId { get; set; }
    public Camera Camera { get; set; } = null!;

    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ReporterIpHash { get; set; }

    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public string? AdminResolutionNotes { get; set; }
}
