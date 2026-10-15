using IndiaCamExplorer.Core.Entities;

namespace IndiaCamExplorer.Web.Models;

public class AdminDashboardViewModel
{
    public int TotalCameras { get; set; }
    public int OnlineCameras { get; set; }
    public int DegradedCameras { get; set; }
    public int OfflineCameras { get; set; }
    public int PendingReports { get; set; }
    public int PendingSuggestions { get; set; }
    public IEnumerable<SourceProvider> Providers { get; set; } = Enumerable.Empty<SourceProvider>();
    public IEnumerable<BrokenStreamReport> RecentReports { get; set; } = Enumerable.Empty<BrokenStreamReport>();
    public IEnumerable<Camera> RecentCameras { get; set; } = Enumerable.Empty<Camera>();
}
