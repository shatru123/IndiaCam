using IndiaCamExplorer.Core.Entities;

namespace IndiaCamExplorer.Core.Interfaces;

public interface IHealthCheckService
{
    Task<CameraHealthCheck> CheckCameraHealthAsync(Camera camera, CancellationToken cancellationToken = default);
}
