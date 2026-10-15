using System.ComponentModel.DataAnnotations;

namespace IndiaCamExplorer.Core.Models;

public class BrokenStreamReportRequest
{
    [Required]
    public int CameraId { get; set; }

    [Required]
    [StringLength(100)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Notes { get; set; }
}
