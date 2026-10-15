using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IndiaCamExplorer.Infrastructure.BackgroundServices;

public class CameraHealthCheckBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CameraHealthCheckBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10);

    public CameraHealthCheckBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<CameraHealthCheckBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Camera Health Check Background Service is starting.");

        // Wait initial delay after app start before running first health probe
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformHealthChecksAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Camera Health Check Background Service");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Camera Health Check Background Service is stopping.");
    }

    public async Task PerformHealthChecksAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var healthProber = scope.ServiceProvider.GetRequiredService<IHealthCheckService>();

        var activeCameras = await dbContext.Cameras
            .Where(c => c.IsActive)
            .Take(50)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Running automated health checks for {Count} active cameras...", activeCameras.Count);

        foreach (var camera in activeCameras)
        {
            if (cancellationToken.IsCancellationRequested) break;

            try
            {
                var result = await healthProber.CheckCameraHealthAsync(camera, cancellationToken);
                camera.HealthStatus = result.Status;
                camera.LastCheckedAt = DateTime.UtcNow;

                dbContext.CameraHealthChecks.Add(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Health check failed for camera {Id}: {Error}", camera.Id, ex.Message);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Completed health check batch.");
    }
}
