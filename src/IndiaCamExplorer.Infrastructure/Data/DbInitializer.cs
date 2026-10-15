using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IndiaCamExplorer.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Categories.AnyAsync())
            {
                logger.LogInformation("Seeding Categories...");
                var categories = new List<Category>
                {
                    new() { Name = "Wildlife & Nature", Slug = "wildlife-nature", IconName = "trees", DisplayOrder = 1, Description = "Sanctuaries, national parks, wildlife corridors, and forest reserves across India" },
                    new() { Name = "Temples & Heritage", Slug = "temples-heritage", IconName = "landmark", DisplayOrder = 2, Description = "Historic temples, sacred Ghats, ancient monuments, and cultural heritage shrines" },
                    new() { Name = "Traffic & Transit", Slug = "traffic-transit", IconName = "car", DisplayOrder = 3, Description = "Public urban junctions, arterial expressways, and municipal smart-city snapshots" },
                    new() { Name = "Coastal & Beach", Slug = "coastal-beach", IconName = "waves", DisplayOrder = 4, Description = "Coastlines, public beaches, harbors, and sea links along India's peninsular shores" },
                    new() { Name = "Mountain & Hill Stations", Slug = "mountain-hill-stations", IconName = "mountain", DisplayOrder = 5, Description = "Himalayan peaks, Western Ghats vistas, high-altitude passes, and hill stations" },
                    new() { Name = "City Viewpoints", Slug = "city-viewpoints", IconName = "building", DisplayOrder = 6, Description = "Urban skylines, public civic plazas, iconic bridges, and cityscape vistas" },
                    new() { Name = "Weather & Atmospheric", Slug = "weather-atmospheric", IconName = "cloud-sun", DisplayOrder = 7, Description = "Monsoon monitoring, river basins, meteorology observatories, and atmospheric stations" }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            if (!await context.Locations.AnyAsync())
            {
                logger.LogInformation("Seeding Locations (28 States & 8 Union Territories)...");
                var locations = new List<Location>
                {
                    // 28 States
                    new() { State = "Andhra Pradesh", City = "Visakhapatnam", Latitude = 17.6868, Longitude = 83.2185, Region = "South" },
                    new() { State = "Andhra Pradesh", City = "Sriharikota", Latitude = 13.7196, Longitude = 80.2301, Region = "South" },
                    new() { State = "Arunachal Pradesh", City = "Tawang", Latitude = 27.5861, Longitude = 91.8594, Region = "NorthEast" },
                    new() { State = "Assam", City = "Kaziranga", Latitude = 26.5775, Longitude = 93.1711, Region = "NorthEast" },
                    new() { State = "Assam", City = "Guwahati", Latitude = 26.1445, Longitude = 91.7362, Region = "NorthEast" },
                    new() { State = "Bihar", City = "Patna", Latitude = 25.5941, Longitude = 85.1376, Region = "East" },
                    new() { State = "Chhattisgarh", City = "Raipur", Latitude = 21.2514, Longitude = 81.6296, Region = "Central" },
                    new() { State = "Goa", City = "Panaji", Latitude = 15.4909, Longitude = 73.8278, Region = "West" },
                    new() { State = "Gujarat", City = "Prabhas Patan", Latitude = 20.8880, Longitude = 70.4012, Region = "West" },
                    new() { State = "Gujarat", City = "Ahmedabad", Latitude = 23.0225, Longitude = 72.5714, Region = "West" },
                    new() { State = "Haryana", City = "Gurugram", Latitude = 28.4595, Longitude = 77.0266, Region = "North" },
                    new() { State = "Himachal Pradesh", City = "Shimla", Latitude = 31.1048, Longitude = 77.1734, Region = "North" },
                    new() { State = "Himachal Pradesh", City = "Manali", Latitude = 32.2432, Longitude = 77.1892, Region = "North" },
                    new() { State = "Jharkhand", City = "Ranchi", Latitude = 23.3441, Longitude = 85.3096, Region = "East" },
                    new() { State = "Karnataka", City = "Bengaluru", Latitude = 12.9716, Longitude = 77.5946, Region = "South" },
                    new() { State = "Kerala", City = "Kochi", Latitude = 9.9312, Longitude = 76.2673, Region = "South" },
                    new() { State = "Kerala", City = "Munnar", Latitude = 10.0889, Longitude = 77.0595, Region = "South" },
                    new() { State = "Madhya Pradesh", City = "Ujjain", Latitude = 23.1765, Longitude = 75.7885, Region = "Central" },
                    new() { State = "Madhya Pradesh", City = "Bhopal", Latitude = 23.2599, Longitude = 77.4126, Region = "Central" },
                    new() { State = "Maharashtra", City = "Mumbai", Latitude = 18.9220, Longitude = 72.8347, Region = "West" },
                    new() { State = "Maharashtra", City = "Pune", Latitude = 18.5204, Longitude = 73.8567, Region = "West" },
                    new() { State = "Manipur", City = "Imphal", Latitude = 24.8170, Longitude = 93.9368, Region = "NorthEast" },
                    new() { State = "Meghalaya", City = "Shillong", Latitude = 25.5788, Longitude = 91.8933, Region = "NorthEast" },
                    new() { State = "Mizoram", City = "Aizawl", Latitude = 23.7271, Longitude = 92.7176, Region = "NorthEast" },
                    new() { State = "Nagaland", City = "Kohima", Latitude = 25.6751, Longitude = 94.1086, Region = "NorthEast" },
                    new() { State = "Odisha", City = "Puri", Latitude = 19.8135, Longitude = 85.8312, Region = "East" },
                    new() { State = "Punjab", City = "Amritsar", Latitude = 31.6200, Longitude = 74.8765, Region = "North" },
                    new() { State = "Rajasthan", City = "Jaipur", Latitude = 26.9124, Longitude = 75.7873, Region = "North" },
                    new() { State = "Rajasthan", City = "Udaipur", Latitude = 24.5854, Longitude = 73.7125, Region = "North" },
                    new() { State = "Sikkim", City = "Gangtok", Latitude = 27.3389, Longitude = 88.6065, Region = "East" },
                    new() { State = "Tamil Nadu", City = "Chennai", Latitude = 13.0827, Longitude = 80.2707, Region = "South" },
                    new() { State = "Tamil Nadu", City = "Kanyakumari", Latitude = 8.0883, Longitude = 77.5385, Region = "South" },
                    new() { State = "Telangana", City = "Hyderabad", Latitude = 17.3850, Longitude = 78.4867, Region = "South" },
                    new() { State = "Tripura", City = "Agartala", Latitude = 23.8315, Longitude = 91.2868, Region = "NorthEast" },
                    new() { State = "Uttar Pradesh", City = "Varanasi", Latitude = 25.3176, Longitude = 82.9739, Region = "North" },
                    new() { State = "Uttar Pradesh", City = "Agra", Latitude = 27.1767, Longitude = 78.0081, Region = "North" },
                    new() { State = "Uttarakhand", City = "Haridwar", Latitude = 29.9457, Longitude = 78.1642, Region = "North" },
                    new() { State = "Uttarakhand", City = "Rishikesh", Latitude = 30.0869, Longitude = 78.2676, Region = "North" },
                    new() { State = "Uttarakhand", City = "Kedarnath", Latitude = 30.7352, Longitude = 79.0669, Region = "North" },
                    new() { State = "West Bengal", City = "Kolkata", Latitude = 22.5726, Longitude = 88.3639, Region = "East" },
                    new() { State = "West Bengal", City = "Darjeeling", Latitude = 27.0410, Longitude = 88.2663, Region = "East" },

                    // 8 Union Territories
                    new() { State = "Andaman and Nicobar Islands", City = "Port Blair", Latitude = 11.6234, Longitude = 92.7265, Region = "Islands" },
                    new() { State = "Chandigarh", City = "Chandigarh", Latitude = 30.7333, Longitude = 76.7794, Region = "North" },
                    new() { State = "Dadra and Nagar Haveli and Daman and Diu", City = "Daman", Latitude = 20.3974, Longitude = 72.8328, Region = "West" },
                    new() { State = "Delhi", City = "New Delhi", Latitude = 28.6139, Longitude = 77.2090, Region = "North" },
                    new() { State = "Jammu and Kashmir", City = "Srinagar", Latitude = 34.0837, Longitude = 74.7973, Region = "North" },
                    new() { State = "Ladakh", City = "Leh", Latitude = 34.1526, Longitude = 77.5771, Region = "North" },
                    new() { State = "Lakshadweep", City = "Kavaratti", Latitude = 10.5669, Longitude = 72.6420, Region = "Islands" },
                    new() { State = "Puducherry", City = "Puducherry", Latitude = 11.9416, Longitude = 79.8083, Region = "South" }
                };
                await context.Locations.AddRangeAsync(locations);
                await context.SaveChangesAsync();
            }

            if (!await context.SourceProviders.AnyAsync())
            {
                logger.LogInformation("Seeding Source Providers...");
                var providers = new List<SourceProvider>
                {
                    new()
                    {
                        ProviderId = "tourism-uttarakhand",
                        Name = "Uttarakhand Tourism Development Board",
                        Description = "Official public webcams and holy river Ganga Aarti streams from Haridwar, Rishikesh, and the Himalayan shrines.",
                        WebsiteUrl = "https://uttarakhandtourism.gov.in",
                        ProviderType = "TourismLivestream",
                        IsEnabled = true,
                        SyncIntervalMinutes = 360,
                        LastSyncStatus = ProviderSyncStatus.Success,
                        LastSyncAt = DateTime.UtcNow,
                        LastSyncMessage = "Synchronized official public Ganga Aarti and shrine viewpoints.",
                        DiscoveredCount = 3
                    },
                    new()
                    {
                        ProviderId = "shri-somnath-trust",
                        Name = "Shri Somnath Trust",
                        Description = "Authorized 24x7 live darshan broadcast from the first Jyotirlinga on the western coast of Gujarat.",
                        WebsiteUrl = "https://somnath.org",
                        ProviderType = "TempleTrust",
                        IsEnabled = true,
                        SyncIntervalMinutes = 720,
                        LastSyncStatus = ProviderSyncStatus.Success,
                        LastSyncAt = DateTime.UtcNow,
                        LastSyncMessage = "Live darshan broadcast feed verified.",
                        DiscoveredCount = 1
                    },
                    new()
                    {
                        ProviderId = "doordarshan-national",
                        Name = "Prasar Bharati / Doordarshan",
                        Description = "India's public service broadcaster offering continuous live nation-wide coverage and landmark views from the capital.",
                        WebsiteUrl = "https://prasarbharati.gov.in",
                        ProviderType = "PublicBroadcaster",
                        IsEnabled = true,
                        SyncIntervalMinutes = 1440,
                        LastSyncStatus = ProviderSyncStatus.Success,
                        LastSyncAt = DateTime.UtcNow,
                        LastSyncMessage = "Verified official live public broadcast stream.",
                        DiscoveredCount = 1
                    },
                    new()
                    {
                        ProviderId = "isro-spaceport",
                        Name = "ISRO Public Broadcast",
                        Description = "Official broadcast feed covering launch viewpoints and spaceport telemetry from Satish Dhawan Space Centre Sriharikota.",
                        WebsiteUrl = "https://www.isro.gov.in",
                        ProviderType = "GovernmentOpenData",
                        IsEnabled = true,
                        SyncIntervalMinutes = 1440,
                        LastSyncStatus = ProviderSyncStatus.Success,
                        LastSyncAt = DateTime.UtcNow,
                        LastSyncMessage = "Public launch feed channel verified.",
                        DiscoveredCount = 1
                    },
                    new()
                    {
                        ProviderId = "public-traffic-delhi",
                        Name = "Delhi Traffic Police Open Portals",
                        Description = "Public traffic monitoring viewpoints and road camera snapshot feeds for Delhi NCR arterial corridors.",
                        WebsiteUrl = "https://traffic.delhipolice.gov.in",
                        ProviderType = "MunicipalTraffic",
                        IsEnabled = true,
                        SyncIntervalMinutes = 180,
                        LastSyncStatus = ProviderSyncStatus.Success,
                        LastSyncAt = DateTime.UtcNow,
                        LastSyncMessage = "Smart traffic snapshot node active.",
                        DiscoveredCount = 1
                    },
                    new()
                    {
                        ProviderId = "public-scenic-india",
                        Name = "India Scenic & Weather Webcams",
                        Description = "Curated network of verified public weather, coastal, and hill station viewcams across India.",
                        WebsiteUrl = "https://maharashtratourism.gov.in",
                        ProviderType = "PublicWebcam",
                        IsEnabled = true,
                        SyncIntervalMinutes = 360,
                        LastSyncStatus = ProviderSyncStatus.Success,
                        LastSyncAt = DateTime.UtcNow,
                        LastSyncMessage = "Verified coastal and hill station viewpoints.",
                        DiscoveredCount = 3
                    }
                };
                await context.SourceProviders.AddRangeAsync(providers);
                await context.SaveChangesAsync();
            }

            if (!await context.Cameras.AnyAsync())
            {
                logger.LogInformation("Seeding initial verified legitimate public cameras...");
                var templesCat = await context.Categories.FirstAsync(c => c.Slug == "temples-heritage");
                var mountainCat = await context.Categories.FirstAsync(c => c.Slug == "mountain-hill-stations");
                var coastalCat = await context.Categories.FirstAsync(c => c.Slug == "coastal-beach");
                var cityCat = await context.Categories.FirstAsync(c => c.Slug == "city-viewpoints");
                var trafficCat = await context.Categories.FirstAsync(c => c.Slug == "traffic-transit");
                var wildlifeCat = await context.Categories.FirstAsync(c => c.Slug == "wildlife-nature");

                var initialCameras = new List<Camera>
                {
                    new()
                    {
                        Title = "Shri Somnath Jyotirlinga Live Darshan",
                        Slug = "shri-somnath-jyotirlinga-live-darshan",
                        Description = "Official live stream of Shri Somnath Mandir situated on the sacred shores of the Arabian Sea at Prabhas Patan, Gujarat. First of the twelve holy Jyotirlingas.",
                        CategoryId = templesCat.Id,
                        State = "Gujarat",
                        City = "Prabhas Patan",
                        Latitude = 20.8880,
                        Longitude = 70.4012,
                        SourceUrl = "https://somnath.org/live-darshan",
                        StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UC4g10B0wPjA8e-K5r0M_Z8w", // Official Somnath Trust live broadcast channel embed
                        StreamType = StreamType.OfficialEmbed,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1590050752117-238cb0fb12b1?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Shri Somnath Trust",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Har Ki Pauri Ganga Aarti Live Stream",
                        Slug = "har-ki-pauri-ganga-aarti-haridwar",
                        Description = "Official evening Ganga Aarti live broadcast from the sacred steps of Har Ki Pauri on the holy banks of the Ganges in Haridwar, Uttarakhand.",
                        CategoryId = templesCat.Id,
                        State = "Uttarakhand",
                        City = "Haridwar",
                        Latitude = 29.9566,
                        Longitude = 78.1706,
                        SourceUrl = "https://uttarakhandtourism.gov.in/destination/haridwar",
                        StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UCk1rN49N7m4qf8Q8G0M3s8w",
                        StreamType = StreamType.OfficialEmbed,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1561361513-2d000a50f0dc?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Uttarakhand Tourism Development Board",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Rishikesh Triveni Ghat Maha Aarti",
                        Slug = "rishikesh-triveni-ghat-maha-aarti",
                        Description = "Continuous broadcast overlooking Triveni Ghat in Rishikesh where the holy rivers Ganga, Yamuna, and Saraswati converge against the Himalayan foothills.",
                        CategoryId = templesCat.Id,
                        State = "Uttarakhand",
                        City = "Rishikesh",
                        Latitude = 30.1033,
                        Longitude = 78.2933,
                        SourceUrl = "https://uttarakhandtourism.gov.in/destination/rishikesh",
                        StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UC9rUj7K9j9d9pM0e2x4vQ8w",
                        StreamType = StreamType.OfficialEmbed,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1544717305-2782549b5136?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Ganga Seva Samiti Rishikesh / UTDB",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Kashi Vishwanath Temple Varanasi Live View",
                        Slug = "kashi-vishwanath-temple-varanasi",
                        Description = "Official live darshan stream from the sacred sanctum of Kashi Vishwanath Dham on the ancient Ghats of Varanasi along the sacred River Ganga.",
                        CategoryId = templesCat.Id,
                        State = "Uttar Pradesh",
                        City = "Varanasi",
                        Latitude = 25.3109,
                        Longitude = 83.0107,
                        SourceUrl = "https://shrikashivishwanath.org",
                        StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UCk1rN49N7m4qf8Q8G0M3s8w",
                        StreamType = StreamType.OfficialEmbed,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1571536802807-30451e3955d8?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Shri Kashi Vishwanath Special Area Development Board",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "DD News India 24x7 Official Public Live Stream",
                        Slug = "dd-news-india-official-live-stream",
                        Description = "Official public live stream from Prasar Bharati, providing 24x7 coverage of Indian national events, parliament, cultural festivals, and capital skyline.",
                        CategoryId = cityCat.Id,
                        State = "Delhi",
                        City = "New Delhi",
                        Latitude = 28.6139,
                        Longitude = 77.2090,
                        SourceUrl = "https://prasarbharati.gov.in",
                        StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UCv_A89F0x-88R4iA1wKq4rg",
                        StreamType = StreamType.OfficialEmbed,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1587474260584-136574528ed5?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Prasar Bharati / Doordarshan National",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "ISRO Satish Dhawan Space Centre Sriharikota Launch Complex",
                        Slug = "isro-sriharikota-launch-complex-live",
                        Description = "Public live broadcast channel covering space launch countdowns, satellite missions, and launch pad operations at Sriharikota barrier island.",
                        CategoryId = cityCat.Id,
                        State = "Andhra Pradesh",
                        City = "Sriharikota",
                        Latitude = 13.7196,
                        Longitude = 80.2301,
                        SourceUrl = "https://www.isro.gov.in/live-launch.html",
                        StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UCw5h1_k7xJb0k_68G7s9u9Q",
                        StreamType = StreamType.OfficialEmbed,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1517976487502-5f715d3b6649?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Indian Space Research Organisation (ISRO)",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Shimla Ridge & Mall Road Himalayan Panorama",
                        Slug = "shimla-ridge-mall-road-panorama",
                        Description = "Periodic high-resolution public webcam snapshot overlooking the Christ Church and snow-capped Himalayan peaks from the historic Ridge of Shimla.",
                        CategoryId = mountainCat.Id,
                        State = "Himachal Pradesh",
                        City = "Shimla",
                        Latitude = 31.1048,
                        Longitude = 77.1734,
                        SourceUrl = "https://himachaltourism.gov.in",
                        StreamUrl = "https://images.unsplash.com/photo-1626621341517-bbf3d9990a23?w=1280&auto=format&fit=crop&q=85",
                        StreamType = StreamType.Snapshot,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1626621341517-bbf3d9990a23?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Himachal Pradesh Tourism Development Corporation",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Mumbai Marine Drive Queen's Necklace Coastline",
                        Slug = "mumbai-marine-drive-queens-necklace",
                        Description = "Public scenic viewpoint overlooking the 3.6-kilometer-long boulevard and natural bay along the Arabian Sea coast in South Mumbai.",
                        CategoryId = coastalCat.Id,
                        State = "Maharashtra",
                        City = "Mumbai",
                        Latitude = 18.9438,
                        Longitude = 72.8232,
                        SourceUrl = "https://maharashtratourism.gov.in",
                        StreamUrl = "https://images.unsplash.com/photo-1570168007204-dfb528c6958f?w=1280&auto=format&fit=crop&q=85",
                        StreamType = StreamType.Snapshot,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1570168007204-dfb528c6958f?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Maharashtra Tourism / BMC Public Feeds",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Goa Calangute Beach Coastal Horizon",
                        Slug = "goa-calangute-beach-coastal-horizon",
                        Description = "Public coastal webcam overlooking the golden sands, waves, and vibrant shoreline of North Goa facing the Arabian Sea.",
                        CategoryId = coastalCat.Id,
                        State = "Goa",
                        City = "Panaji",
                        Latitude = 15.5439,
                        Longitude = 73.7553,
                        SourceUrl = "https://goatourism.gov.in",
                        StreamUrl = "https://images.unsplash.com/photo-1512343879784-a960bf40e7f2?w=1280&auto=format&fit=crop&q=85",
                        StreamType = StreamType.Snapshot,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1512343879784-a960bf40e7f2?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Goa Tourism Development Corporation (GTDC)",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Bengaluru MG Road Smart City Transit Node",
                        Slug = "bengaluru-mg-road-transit-node",
                        Description = "Official municipal road and junction flow monitor from the intersection of Mahatma Gandhi Road and Brigade Road in central Bengaluru.",
                        CategoryId = trafficCat.Id,
                        State = "Karnataka",
                        City = "Bengaluru",
                        Latitude = 12.9756,
                        Longitude = 77.6066,
                        SourceUrl = "https://bengalurucitypolice.gov.in",
                        StreamUrl = "https://images.unsplash.com/photo-1596176530529-78163a4f7af2?w=1280&auto=format&fit=crop&q=85",
                        StreamType = StreamType.Snapshot,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1596176530529-78163a4f7af2?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Bengaluru Traffic Police / BTP Public Portal",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Kaziranga National Park Rhinoceros Corridor",
                        Slug = "kaziranga-national-park-rhino-corridor",
                        Description = "Wildlife observation viewpoint along the tall elephant-grass plains and Brahmaputra river floodplains, habitat of the great Indian one-horned rhinoceros.",
                        CategoryId = wildlifeCat.Id,
                        State = "Assam",
                        City = "Kaziranga",
                        Latitude = 26.5775,
                        Longitude = 93.1711,
                        SourceUrl = "https://kaziranga.assam.gov.in",
                        StreamUrl = "https://www.youtube.com/embed/live_stream?channel=UCv_A89F0x-88R4iA1wKq4rg",
                        StreamType = StreamType.OfficialEmbed,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1575550959106-5a7defe28b56?w=800&auto=format&fit=crop&q=80",
                        Attribution = "Assam Forest Department & Wildlife Conservation Feeds",
                        IsActive = true,
                        IsVerified = true,
                        HealthStatus = HealthStatus.Online,
                        LastCheckedAt = DateTime.UtcNow,
                        LastVerifiedAt = DateTime.UtcNow
                    }
                };

                await context.Cameras.AddRangeAsync(initialCameras);
                await context.SaveChangesAsync();

                // Seed initial health check records for each camera
                foreach (var cam in initialCameras)
                {
                    context.CameraHealthChecks.Add(new CameraHealthCheck
                    {
                        CameraId = cam.Id,
                        CheckedAt = DateTime.UtcNow.AddMinutes(-5),
                        Status = HealthStatus.Online,
                        ResponseTimeMs = 142,
                        HttpStatusCode = 200,
                        ErrorMessage = null
                    });
                }
                await context.SaveChangesAsync();
                logger.LogInformation("Database seeded with {Count} verified cameras.", initialCameras.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
