using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Models;

namespace IndiaCamExplorer.Web.Models;

public class HomeViewModel
{
    public IEnumerable<Camera> FeaturedCameras { get; set; } = Enumerable.Empty<Camera>();
    public IEnumerable<Camera> RecentlyAdded { get; set; } = Enumerable.Empty<Camera>();
    public IEnumerable<CategoryCountDto> Categories { get; set; } = Enumerable.Empty<CategoryCountDto>();
    public IEnumerable<StateSummaryDto> StateSummaries { get; set; } = Enumerable.Empty<StateSummaryDto>();
    public int TotalCameras { get; set; }
    public int OnlineCameras { get; set; }
    public int TotalStates { get; set; }
}
