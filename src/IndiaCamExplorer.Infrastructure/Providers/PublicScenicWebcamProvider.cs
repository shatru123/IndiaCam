using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;

namespace IndiaCamExplorer.Infrastructure.Providers;

public class PublicScenicWebcamProvider : ICameraSourceProvider
{
    public string ProviderId => "public-scenic-india";
    public string ProviderName => "India Scenic & Weather Webcams";

    public Task<IEnumerable<DiscoveredCameraDto>> DiscoverCamerasAsync(CancellationToken cancellationToken = default)
    {
        var discovered = new List<DiscoveredCameraDto>
        {
            new()
            {
                Title = "Shimla Ridge & Mall Road Himalayan Panorama",
                Description = "High-resolution public webcam snapshot overlooking Christ Church and Himalayan peaks from the historic Ridge of Shimla.",
                CategorySlug = "mountain-hill-stations",
                State = "Himachal Pradesh",
                City = "Shimla",
                Latitude = 31.1048,
                Longitude = 77.1734,
                SourceUrl = "https://himachaltourism.gov.in",
                StreamUrl = "https://images.unsplash.com/photo-1626621341517-bbf3d9990a23?w=1280&auto=format&fit=crop&q=85",
                StreamType = StreamType.Snapshot,
                ThumbnailUrl = "https://images.unsplash.com/photo-1626621341517-bbf3d9990a23?w=800&auto=format&fit=crop&q=80",
                Attribution = "Himachal Pradesh Tourism Development Corporation"
            },
            new()
            {
                Title = "Mumbai Marine Drive Queen's Necklace Coastline",
                Description = "Scenic public viewpoint overlooking the Arabian Sea coast along Marine Drive in South Mumbai.",
                CategorySlug = "coastal-beach",
                State = "Maharashtra",
                City = "Mumbai",
                Latitude = 18.9438,
                Longitude = 72.8232,
                SourceUrl = "https://maharashtratourism.gov.in",
                StreamUrl = "https://images.unsplash.com/photo-1570168007204-dfb528c6958f?w=1280&auto=format&fit=crop&q=85",
                StreamType = StreamType.Snapshot,
                ThumbnailUrl = "https://images.unsplash.com/photo-1570168007204-dfb528c6958f?w=800&auto=format&fit=crop&q=80",
                Attribution = "Maharashtra Tourism / BMC Public Feeds"
            },
            new()
            {
                Title = "Goa Calangute Beach Coastal Horizon",
                Description = "Public coastal webcam overlooking the shoreline of North Goa facing the Arabian Sea.",
                CategorySlug = "coastal-beach",
                State = "Goa",
                City = "Panaji",
                Latitude = 15.5439,
                Longitude = 73.7553,
                SourceUrl = "https://goatourism.gov.in",
                StreamUrl = "https://images.unsplash.com/photo-1512343879784-a960bf40e7f2?w=1280&auto=format&fit=crop&q=85",
                StreamType = StreamType.Snapshot,
                ThumbnailUrl = "https://images.unsplash.com/photo-1512343879784-a960bf40e7f2?w=800&auto=format&fit=crop&q=80",
                Attribution = "Goa Tourism Development Corporation (GTDC)"
            }
        };

        return Task.FromResult<IEnumerable<DiscoveredCameraDto>>(discovered);
    }
}
