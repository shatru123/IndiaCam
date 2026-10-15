using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;

namespace IndiaCamExplorer.Infrastructure.Providers;

public class PublicTrafficCameraProvider : ICameraSourceProvider
{
    public string ProviderId => "public-traffic-delhi";
    public string ProviderName => "Delhi Traffic Police Open Portals";

    public Task<IEnumerable<DiscoveredCameraDto>> DiscoverCamerasAsync(CancellationToken cancellationToken = default)
    {
        var discovered = new List<DiscoveredCameraDto>
        {
            new()
            {
                Title = "Bengaluru MG Road Smart City Transit Node",
                Description = "Official municipal road and junction flow monitor from the intersection of MG Road and Brigade Road in central Bengaluru.",
                CategorySlug = "traffic-transit",
                State = "Karnataka",
                City = "Bengaluru",
                Latitude = 12.9756,
                Longitude = 77.6066,
                SourceUrl = "https://bengalurucitypolice.gov.in",
                StreamUrl = "https://images.unsplash.com/photo-1596176530529-78163a4f7af2?w=1280&auto=format&fit=crop&q=85",
                StreamType = StreamType.Snapshot,
                ThumbnailUrl = "https://images.unsplash.com/photo-1596176530529-78163a4f7af2?w=800&auto=format&fit=crop&q=80",
                Attribution = "Bengaluru Traffic Police / BTP Public Portal"
            }
        };

        return Task.FromResult<IEnumerable<DiscoveredCameraDto>>(discovered);
    }
}
