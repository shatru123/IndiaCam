using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace IndiaCamExplorer.Tests.Services;

public class HealthCheckTests
{
    private class FakeValidator : ISecurityValidator
    {
        private readonly bool _isValid;
        public FakeValidator(bool isValid) => _isValid = isValid;

        public bool IsSafePublicUrl(string? url, out string? failureReason)
        {
            if (_isValid)
            {
                failureReason = null;
                return true;
            }
            failureReason = "Blocked internal URL";
            return false;
        }
    }

    private class FakeHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new HttpClient();
    }

    [Fact]
    public async Task CheckCameraHealthAsync_WhenSecurityValidatorFails_ReturnsOfflineWithoutNetworkCall()
    {
        var validator = new FakeValidator(false);
        var factory = new FakeHttpClientFactory();
        var service = new HealthCheckService(factory, validator, NullLogger<HealthCheckService>.Instance);

        var camera = new Camera
        {
            Id = 1,
            Title = "Internal Probe Test",
            StreamUrl = "http://127.0.0.1:8080/stream"
        };

        var result = await service.CheckCameraHealthAsync(camera);

        Assert.Equal(HealthStatus.Offline, result.Status);
        Assert.Contains("Security check failed", result.ErrorMessage);
    }
}
