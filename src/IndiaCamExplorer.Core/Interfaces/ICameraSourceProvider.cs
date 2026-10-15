using IndiaCamExplorer.Core.Models;

namespace IndiaCamExplorer.Core.Interfaces;

public interface ICameraSourceProvider
{
    string ProviderId { get; }
    string ProviderName { get; }
    Task<IEnumerable<DiscoveredCameraDto>> DiscoverCamerasAsync(CancellationToken cancellationToken = default);
}
