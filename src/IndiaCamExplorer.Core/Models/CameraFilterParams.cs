using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Core.Models;

public class CameraFilterParams
{
    public string? Search { get; set; }
    public string? CategorySlug { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public StreamType? StreamType { get; set; }
    public HealthStatus? HealthStatus { get; set; }
    public string? SortBy { get; set; } = "popular"; // popular, recent, name, state
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
