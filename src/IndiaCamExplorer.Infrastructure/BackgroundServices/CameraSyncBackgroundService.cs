using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IndiaCamExplorer.Infrastructure.BackgroundServices;

public class CameraSyncBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CameraSyncBackgroundService> _logger;
    private readonly TimeSpan _syncInterval = TimeSpan.FromHours(6);

    public CameraSyncBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<CameraSyncBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Camera Discovery Sync Background Service is starting.");

        // Wait initial delay after app start
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SynchronizeProvidersAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Camera Discovery Sync Background Service");
            }

            await Task.Delay(_syncInterval, stoppingToken);
        }

        _logger.LogInformation("Camera Discovery Sync Background Service is stopping.");
    }

    public async Task SynchronizeProvidersAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var providers = scope.ServiceProvider.GetServices<ICameraSourceProvider>();
        var securityValidator = scope.ServiceProvider.GetRequiredService<ISecurityValidator>();

        var categories = await dbContext.Categories.ToDictionaryAsync(c => c.Slug, c => c.Id, cancellationToken);

        foreach (var provider in providers)
        {
            if (cancellationToken.IsCancellationRequested) break;

            var dbProvider = await dbContext.SourceProviders
                .FirstOrDefaultAsync(sp => sp.ProviderId == provider.ProviderId, cancellationToken);

            if (dbProvider != null && !dbProvider.IsEnabled)
            {
                continue;
            }

            try
            {
                _logger.LogInformation("Synchronizing provider: {Name} ({Id})", provider.ProviderName, provider.ProviderId);
                var discovered = await provider.DiscoverCamerasAsync(cancellationToken);
                var newCount = 0;

                foreach (var item in discovered)
                {
                    if (!securityValidator.IsSafePublicUrl(item.StreamUrl, out _))
                        continue;

                    var exists = await dbContext.Cameras.AnyAsync(
                        c => c.StreamUrl == item.StreamUrl || c.Title == item.Title,
                        cancellationToken);

                    if (!exists && categories.TryGetValue(item.CategorySlug, out var categoryId))
                    {
                        var slug = GenerateSlug(item.Title);
                        var uniqueSlug = slug;
                        var suffix = 1;
                        while (await dbContext.Cameras.AnyAsync(c => c.Slug == uniqueSlug, cancellationToken))
                        {
                            uniqueSlug = $"{slug}-{suffix++}";
                        }

                        var camera = new Camera
                        {
                            Title = item.Title,
                            Slug = uniqueSlug,
                            Description = item.Description,
                            CategoryId = categoryId,
                            State = item.State,
                            City = item.City,
                            Latitude = item.Latitude,
                            Longitude = item.Longitude,
                            SourceUrl = item.SourceUrl,
                            StreamUrl = item.StreamUrl,
                            StreamType = item.StreamType,
                            ThumbnailUrl = item.ThumbnailUrl,
                            Attribution = item.Attribution,
                            IsActive = true,
                            IsVerified = true,
                            HealthStatus = HealthStatus.Online,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        dbContext.Cameras.Add(camera);
                        newCount++;
                    }
                }

                if (dbProvider != null)
                {
                    dbProvider.LastSyncAt = DateTime.UtcNow;
                    dbProvider.LastSyncStatus = ProviderSyncStatus.Success;
                    dbProvider.LastSyncMessage = $"Successfully synced. Discovered {newCount} new feeds.";
                    dbProvider.DiscoveredCount += newCount;
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Provider {Id} synchronization error: {Message}", provider.ProviderId, ex.Message);
                if (dbProvider != null)
                {
                    dbProvider.LastSyncAt = DateTime.UtcNow;
                    dbProvider.LastSyncStatus = ProviderSyncStatus.Failed;
                    dbProvider.LastSyncMessage = $"Sync failed: {ex.Message}";
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    private static string GenerateSlug(string title)
    {
        var clean = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and");
        var chars = clean.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray();
        return new string(chars).Trim('-');
    }
}
