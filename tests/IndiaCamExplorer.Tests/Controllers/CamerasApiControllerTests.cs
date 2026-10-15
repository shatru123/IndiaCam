using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;
using IndiaCamExplorer.Web.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace IndiaCamExplorer.Tests.Controllers;

public class CamerasApiControllerTests
{
    private class FakeCameraRepo : ICameraRepository
    {
        public Task<PagedResult<Camera>> GetCamerasAsync(CameraFilterParams filterParams, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedResult<Camera>
            {
                Items = new List<Camera>
                {
                    new() { Id = 1, Title = "Somnath Live", IsActive = true, HealthStatus = HealthStatus.Online }
                },
                TotalItems = 1,
                Page = 1,
                PageSize = 12
            });
        }

        public Task<Camera?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id == 1)
                return Task.FromResult<Camera?>(new Camera { Id = 1, Title = "Somnath Live", IsActive = true, HealthStatus = HealthStatus.Online });
            return Task.FromResult<Camera?>(null);
        }

        public Task<Camera?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) => Task.FromResult<Camera?>(null);
        public Task<IEnumerable<Camera>> GetFeaturedCamerasAsync(int count = 6, CancellationToken cancellationToken = default) => Task.FromResult(Enumerable.Empty<Camera>());
        public Task<IEnumerable<Camera>> GetRecentlyAddedCamerasAsync(int count = 6, CancellationToken cancellationToken = default) => Task.FromResult(Enumerable.Empty<Camera>());
        public Task<IEnumerable<CameraMarkerDto>> GetMarkersAsync(CancellationToken cancellationToken = default) => Task.FromResult(Enumerable.Empty<CameraMarkerDto>());
        public Task<IEnumerable<StateSummaryDto>> GetStateSummariesAsync(CancellationToken cancellationToken = default) => Task.FromResult(Enumerable.Empty<StateSummaryDto>());
        public Task<IEnumerable<CameraHealthCheck>> GetRecentHealthChecksAsync(int cameraId, int count = 10, CancellationToken cancellationToken = default) => Task.FromResult(Enumerable.Empty<CameraHealthCheck>());
        public Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<int> GetOnlineCountAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<Camera> AddAsync(Camera camera, CancellationToken cancellationToken = default) => Task.FromResult(camera);
        public Task UpdateAsync(Camera camera, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private class FakeReportRepo : IBrokenStreamReportRepository
    {
        public List<BrokenStreamReport> Reports = new();

        public Task<BrokenStreamReport> AddAsync(BrokenStreamReport report, CancellationToken cancellationToken = default)
        {
            Reports.Add(report);
            return Task.FromResult(report);
        }

        public Task<IEnumerable<BrokenStreamReport>> GetAllAsync(ReportStatus? status = null, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<BrokenStreamReport>>(Reports);
        public Task<BrokenStreamReport?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult(Reports.FirstOrDefault(r => r.Id == id));
        public Task UpdateAsync(BrokenStreamReport report, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default) => Task.FromResult(Reports.Count);
    }

    [Fact]
    public async Task GetCameras_ReturnsOkWithPagedResult()
    {
        var cameraRepo = new FakeCameraRepo();
        var reportRepo = new FakeReportRepo();
        var controller = new CamerasApiController(cameraRepo, reportRepo);

        var actionResult = await controller.GetCameras(new CameraFilterParams());

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var paged = Assert.IsType<PagedResult<Camera>>(okResult.Value);
        Assert.Single(paged.Items);
    }

    [Fact]
    public async Task ReportBrokenStream_WhenValid_ReturnsOkAndPersistsReport()
    {
        var cameraRepo = new FakeCameraRepo();
        var reportRepo = new FakeReportRepo();
        var controller = new CamerasApiController(cameraRepo, reportRepo);

        var request = new BrokenStreamReportRequest
        {
            CameraId = 1,
            Reason = "Stream is black / frozen",
            Notes = "Nothing displays when playing"
        };

        var actionResult = await controller.ReportBrokenStream(1, request);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Single(reportRepo.Reports);
        Assert.Equal("Stream is black / frozen", reportRepo.Reports.First().Reason);
    }

    [Fact]
    public async Task ReportBrokenStream_WhenCameraNotFound_ReturnsNotFound()
    {
        var cameraRepo = new FakeCameraRepo();
        var reportRepo = new FakeReportRepo();
        var controller = new CamerasApiController(cameraRepo, reportRepo);

        var request = new BrokenStreamReportRequest
        {
            CameraId = 999,
            Reason = "Test",
            Notes = "Test"
        };

        var actionResult = await controller.ReportBrokenStream(999, request);

        Assert.IsType<NotFoundObjectResult>(actionResult);
    }
}
