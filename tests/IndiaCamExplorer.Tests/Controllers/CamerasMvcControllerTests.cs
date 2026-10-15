using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;
using IndiaCamExplorer.Web.Controllers;
using IndiaCamExplorer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace IndiaCamExplorer.Tests.Controllers;

public class CamerasMvcControllerTests
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

        public Task<Camera?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult<Camera?>(null);
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

    private class FakeCategoryRepo : ICategoryRepository
    {
        public Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Category>>(new List<Category>
        {
            new() { Id = 1, Name = "Temples & Heritage", Slug = "temples-heritage" }
        });
        public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult<Category?>(null);
        public Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            if (slug == "temples-heritage")
                return Task.FromResult<Category?>(new Category { Id = 1, Name = "Temples & Heritage", Slug = "temples-heritage" });
            return Task.FromResult<Category?>(null);
        }
        public Task<IEnumerable<CategoryCountDto>> GetCategoriesWithCountsAsync(CancellationToken cancellationToken = default) => Task.FromResult(Enumerable.Empty<CategoryCountDto>());
    }

    private class FakeLocationRepo : ILocationRepository
    {
        public Task<IEnumerable<Location>> GetAllLocationsAsync(CancellationToken cancellationToken = default) => Task.FromResult(Enumerable.Empty<Location>());
        public Task<IEnumerable<string>> GetAllStatesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<string>>(new List<string> { "Gujarat", "Uttarakhand" });
        public Task<IEnumerable<string>> GetCitiesByStateAsync(string state, CancellationToken cancellationToken = default) => Task.FromResult(Enumerable.Empty<string>());
    }

    [Fact]
    public async Task Category_ExplicitlyReturnsIndexView_PreventingViewNotFoundException()
    {
        var controller = new CamerasController(new FakeCameraRepo(), new FakeCategoryRepo(), new FakeLocationRepo());

        var result = await controller.Category("temples-heritage");

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
        var vm = Assert.IsType<CameraCatalogViewModel>(viewResult.Model);
        Assert.NotNull(vm.ActiveCategory);
        Assert.Equal("temples-heritage", vm.ActiveCategory.Slug);
    }

    [Fact]
    public async Task State_ExplicitlyReturnsIndexView_PreventingViewNotFoundException()
    {
        var controller = new CamerasController(new FakeCameraRepo(), new FakeCategoryRepo(), new FakeLocationRepo());

        var result = await controller.State("Gujarat");

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
        var vm = Assert.IsType<CameraCatalogViewModel>(viewResult.Model);
        Assert.Equal("Gujarat", vm.ActiveState);
    }

    [Fact]
    public async Task Index_ExplicitlyReturnsIndexView()
    {
        var controller = new CamerasController(new FakeCameraRepo(), new FakeCategoryRepo(), new FakeLocationRepo());

        var result = await controller.Index(new CameraFilterParams());

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
    }
}
