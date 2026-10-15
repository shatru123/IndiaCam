using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;

namespace IndiaCamExplorer.Infrastructure.Providers;

public class OfficialEmbedLivestreamProvider : ICameraSourceProvider
{
    public string ProviderId => "shri-somnath-trust";
    public string ProviderName => "Shri Somnath Trust & Temple Trusts";

    public Task<IEnumerable<DiscoveredCameraDto>> DiscoverCamerasAsync(CancellationToken cancellationToken = default)
    {
        var discovered = new List<DiscoveredCameraDto>
        {
            new()
            {
                Title = "Shri Somnath Jyotirlinga Live Darshan",
                Description = "Official live stream of Shri Somnath Mandir situated on the sacred shores of the Arabian Sea at Prabhas Patan, Gujarat.",
                CategorySlug = "temples-heritage",
                State = "Gujarat",
                City = "Prabhas Patan",
                Latitude = 20.8880,
                Longitude = 70.4012,
                SourceUrl = "https://somnath.org/live-darshan",
                StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UC4g10B0wPjA8e-K5r0M_Z8w",
                StreamType = StreamType.OfficialEmbed,
                ThumbnailUrl = "https://images.unsplash.com/photo-1590050752117-238cb0fb12b1?w=800&auto=format&fit=crop&q=80",
                Attribution = "Shri Somnath Trust"
            },
            new()
            {
                Title = "DD News India 24x7 Official Public Live Stream",
                Description = "Official public live stream from Prasar Bharati, providing 24x7 national coverage and capital viewpoints.",
                CategorySlug = "city-viewpoints",
                State = "Delhi",
                City = "New Delhi",
                Latitude = 28.6139,
                Longitude = 77.2090,
                SourceUrl = "https://prasarbharati.gov.in",
                StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UCv_A89F0x-88R4iA1wKq4rg",
                StreamType = StreamType.OfficialEmbed,
                ThumbnailUrl = "https://images.unsplash.com/photo-1587474260584-136574528ed5?w=800&auto=format&fit=crop&q=80",
                Attribution = "Prasar Bharati / Doordarshan National"
            }
        };

        return Task.FromResult<IEnumerable<DiscoveredCameraDto>>(discovered);
    }
}
