using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Models;
using IndiaCamExplorer.Infrastructure.Data;
using IndiaCamExplorer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IndiaCamExplorer.Tests.Repositories;

public class CameraRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CameraRepository _repository;

    public CameraRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CameraRepository(_context);

        SeedSampleData();
    }

    private void SeedSampleData()
    {
        var catTemples = new Category { Id = 1, Name = "Temples & Heritage", Slug = "temples-heritage", DisplayOrder = 1 };
        var catCoastal = new Category { Id = 2, Name = "Coastal & Beach", Slug = "coastal-beach", DisplayOrder = 2 };
        _context.Categories.AddRange(catTemples, catCoastal);

        var locGujarat = new Location { Id = 1, State = "Gujarat", City = "Prabhas Patan", Latitude = 20.8880, Longitude = 70.4012, Region = "West" };
        var locGoa = new Location { Id = 2, State = "Goa", City = "Panaji", Latitude = 15.4909, Longitude = 73.8278, Region = "West" };
        var locUP = new Location { Id = 3, State = "Uttar Pradesh", City = "Varanasi", Latitude = 25.3176, Longitude = 82.9739, Region = "North" };
        _context.Locations.AddRange(locGujarat, locGoa, locUP);

        var cameras = new List<Camera>
        {
            new()
            {
                Id = 1,
                Title = "Shri Somnath Jyotirlinga Live Darshan",
                Slug = "shri-somnath-jyotirlinga-live-darshan",
                CategoryId = 1,
                State = "Gujarat",
                City = "Prabhas Patan",
                Latitude = 20.8880,
                Longitude = 70.4012,
                SourceUrl = "https://somnath.org/live",
                StreamUrl = "https://somnath.org/stream.m3u8",
                StreamType = StreamType.Hls,
                Attribution = "Shri Somnath Trust",
                IsActive = true,
                IsVerified = true,
                HealthStatus = HealthStatus.Online
            },
            new()
            {
                Id = 2,
                Title = "Goa Calangute Beach Coastal Horizon",
                Slug = "goa-calangute-beach-coastal-horizon",
                CategoryId = 2,
                State = "Goa",
                City = "Panaji",
                Latitude = 15.5439,
                Longitude = 73.7553,
                SourceUrl = "https://goatourism.gov.in",
                StreamUrl = "https://goatourism.gov.in/cam.jpg",
                StreamType = StreamType.Snapshot,
                Attribution = "Goa Tourism",
                IsActive = true,
                IsVerified = true,
                HealthStatus = HealthStatus.Online
            },
            new()
            {
                Id = 3,
                Title = "Kashi Vishwanath Temple Varanasi",
                Slug = "kashi-vishwanath-temple-varanasi",
                CategoryId = 1,
                State = "Uttar Pradesh",
                City = "Varanasi",
                Latitude = 25.3109,
                Longitude = 83.0107,
                SourceUrl = "https://shrikashivishwanath.org",
                StreamUrl = "https://youtube.com/embed/xyz",
                StreamType = StreamType.OfficialEmbed,
                Attribution = "Shri Kashi Vishwanath Special Area Development Board",
                IsActive = false, // Inactive camera
                IsVerified = true,
                HealthStatus = HealthStatus.Offline
            }
        };

        _context.Cameras.AddRange(cameras);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetCamerasAsync_ReturnsOnlyActiveCameras()
    {
        var result = await _repository.GetCamerasAsync(new CameraFilterParams());
        Assert.Equal(2, result.TotalItems);
        Assert.All(result.Items, c => Assert.True(c.IsActive));
    }

    [Fact]
    public async Task GetCamerasAsync_FilterByCategory_ReturnsMatchingCameras()
    {
        var result = await _repository.GetCamerasAsync(new CameraFilterParams
        {
            CategorySlug = "coastal-beach"
        });

        Assert.Single(result.Items);
        Assert.Equal("Goa Calangute Beach Coastal Horizon", result.Items.First().Title);
    }

    [Fact]
    public async Task GetCamerasAsync_FilterByState_ReturnsMatchingCameras()
    {
        var result = await _repository.GetCamerasAsync(new CameraFilterParams
        {
            State = "Gujarat"
        });

        Assert.Single(result.Items);
        Assert.Equal("Gujarat", result.Items.First().State);
    }

    [Fact]
    public async Task GetCamerasAsync_SearchQuery_MatchesTitleOrCity()
    {
        var result = await _repository.GetCamerasAsync(new CameraFilterParams
        {
            Search = "somnath"
        });

        Assert.Single(result.Items);
        Assert.Equal("Shri Somnath Jyotirlinga Live Darshan", result.Items.First().Title);
    }

    [Fact]
    public async Task GetMarkersAsync_ReturnsActiveMarkersProjection()
    {
        var markers = (await _repository.GetMarkersAsync()).ToList();
        Assert.Equal(2, markers.Count);
        Assert.Contains(markers, m => m.Title == "Shri Somnath Jyotirlinga Live Darshan");
        Assert.Contains(markers, m => m.Title == "Goa Calangute Beach Coastal Horizon");
    }

    [Fact]
    public async Task GetTotalCountAsync_And_GetOnlineCountAsync_CalculatesAccurately()
    {
        var totalActive = await _repository.GetTotalCountAsync();
        var onlineCount = await _repository.GetOnlineCountAsync();

        Assert.Equal(2, totalActive);
        Assert.Equal(2, onlineCount);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
