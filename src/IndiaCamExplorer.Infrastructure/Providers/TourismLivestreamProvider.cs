using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;

namespace IndiaCamExplorer.Infrastructure.Providers;

public class TourismLivestreamProvider : ICameraSourceProvider
{
    public string ProviderId => "tourism-uttarakhand";
    public string ProviderName => "Uttarakhand Tourism Development Board";

    public Task<IEnumerable<DiscoveredCameraDto>> DiscoverCamerasAsync(CancellationToken cancellationToken = default)
    {
        var discovered = new List<DiscoveredCameraDto>
        {
            new()
            {
                Title = "Har Ki Pauri Ganga Aarti Live Stream",
                Description = "Official evening Ganga Aarti live broadcast from the sacred steps of Har Ki Pauri on the holy banks of the Ganges in Haridwar.",
                CategorySlug = "temples-heritage",
                State = "Uttarakhand",
                City = "Haridwar",
                Latitude = 29.9566,
                Longitude = 78.1706,
                SourceUrl = "https://uttarakhandtourism.gov.in/destination/haridwar",
                StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UCk1rN49N7m4qf8Q8G0M3s8w",
                StreamType = StreamType.OfficialEmbed,
                ThumbnailUrl = "https://images.unsplash.com/photo-1561361513-2d000a50f0dc?w=800&auto=format&fit=crop&q=80",
                Attribution = "Uttarakhand Tourism Development Board"
            },
            new()
            {
                Title = "Rishikesh Triveni Ghat Maha Aarti",
                Description = "Continuous broadcast overlooking Triveni Ghat in Rishikesh where the holy rivers Ganga, Yamuna, and Saraswati converge.",
                CategorySlug = "temples-heritage",
                State = "Uttarakhand",
                City = "Rishikesh",
                Latitude = 30.1033,
                Longitude = 78.2933,
                SourceUrl = "https://uttarakhandtourism.gov.in/destination/rishikesh",
                StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UC9rUj7K9j9d9pM0e2x4vQ8w",
                StreamType = StreamType.OfficialEmbed,
                ThumbnailUrl = "https://images.unsplash.com/photo-1544717305-2782549b5136?w=800&auto=format&fit=crop&q=80",
                Attribution = "Ganga Seva Samiti Rishikesh / UTDB"
            }
        };

        return Task.FromResult<IEnumerable<DiscoveredCameraDto>>(discovered);
    }
}
