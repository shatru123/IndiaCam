using System.Diagnostics;
using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace IndiaCamExplorer.Infrastructure.Services;

public class HealthCheckService : IHealthCheckService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISecurityValidator _securityValidator;
    private readonly ILogger<HealthCheckService> _logger;

    public HealthCheckService(
        IHttpClientFactory httpClientFactory,
        ISecurityValidator securityValidator,
        ILogger<HealthCheckService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _securityValidator = securityValidator;
        _logger = logger;
    }

    public async Task<CameraHealthCheck> CheckCameraHealthAsync(Camera camera, CancellationToken cancellationToken = default)
    {
        var healthCheck = new CameraHealthCheck
        {
            CameraId = camera.Id,
            CheckedAt = DateTime.UtcNow
        };

        var targetUrl = !string.IsNullOrWhiteSpace(camera.StreamUrl) ? camera.StreamUrl : camera.SourceUrl;

        if (!_securityValidator.IsSafePublicUrl(targetUrl, out var failureReason))
        {
            healthCheck.Status = HealthStatus.Offline;
            healthCheck.ErrorMessage = $"Security check failed: {failureReason}";
            healthCheck.ResponseTimeMs = 0;
            return healthCheck;
        }

        var client = _httpClientFactory.CreateClient("CameraHealthProber");
        client.Timeout = TimeSpan.FromSeconds(5);

        var sw = Stopwatch.StartNew();
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, targetUrl);
            request.Headers.Add("User-Agent", "IndiaCamExplorer/1.0 (+https://github.com/shatru123/IndiaCam; HealthProber)");
            request.Headers.Add("Accept", "*/*");

            // Only fetch a small initial byte range to avoid high bandwidth usage
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, 2048);

            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            sw.Stop();

            healthCheck.ResponseTimeMs = sw.ElapsedMilliseconds;
            healthCheck.HttpStatusCode = (int)response.StatusCode;

            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.PartialContent)
            {
                if (sw.ElapsedMilliseconds > 2500)
                {
                    healthCheck.Status = HealthStatus.Degraded;
                    healthCheck.ErrorMessage = $"High response latency ({sw.ElapsedMilliseconds}ms)";
                }
                else
                {
                    healthCheck.Status = HealthStatus.Online;
                }
            }
            else if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
            {
                healthCheck.Status = HealthStatus.Degraded;
                healthCheck.ErrorMessage = $"Stream endpoint redirected with HTTP {(int)response.StatusCode}";
            }
            else
            {
                healthCheck.Status = HealthStatus.Offline;
                healthCheck.ErrorMessage = $"Endpoint returned HTTP {(int)response.StatusCode}";
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            healthCheck.ResponseTimeMs = sw.ElapsedMilliseconds;
            healthCheck.Status = HealthStatus.Offline;
            healthCheck.ErrorMessage = $"Probe error: {ex.Message}";
            _logger.LogWarning("Health check failed for camera {Id} ({Title}): {Error}", camera.Id, camera.Title, ex.Message);
        }

        return healthCheck;
    }
}
