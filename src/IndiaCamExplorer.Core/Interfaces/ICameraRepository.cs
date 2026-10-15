using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Models;

namespace IndiaCamExplorer.Core.Interfaces;

public interface ICameraRepository
{
    Task<PagedResult<Camera>> GetCamerasAsync(CameraFilterParams filterParams, CancellationToken cancellationToken = default);
    Task<Camera?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Camera?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Camera>> GetFeaturedCamerasAsync(int count = 6, CancellationToken cancellationToken = default);
    Task<IEnumerable<Camera>> GetRecentlyAddedCamerasAsync(int count = 6, CancellationToken cancellationToken = default);
    Task<IEnumerable<CameraMarkerDto>> GetMarkersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StateSummaryDto>> GetStateSummariesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CameraHealthCheck>> GetRecentHealthChecksAsync(int cameraId, int count = 10, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetOnlineCountAsync(CancellationToken cancellationToken = default);
    Task<Camera> AddAsync(Camera camera, CancellationToken cancellationToken = default);
    Task UpdateAsync(Camera camera, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
