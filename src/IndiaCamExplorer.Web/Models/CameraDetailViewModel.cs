using IndiaCamExplorer.Core.Entities;

namespace IndiaCamExplorer.Web.Models;

public class CameraDetailViewModel
{
    public Camera Camera { get; set; } = null!;
    public IEnumerable<CameraHealthCheck> RecentHealthChecks { get; set; } = Enumerable.Empty<CameraHealthCheck>();
    public IEnumerable<Camera> RelatedCameras { get; set; } = Enumerable.Empty<Camera>();
}
