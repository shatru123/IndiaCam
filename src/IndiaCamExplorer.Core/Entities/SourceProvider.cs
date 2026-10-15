using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Core.Entities;

public class SourceProvider
{
    public int Id { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string ProviderType { get; set; } = "Tourism";

    public bool IsEnabled { get; set; } = true;
    public int SyncIntervalMinutes { get; set; } = 360; // 6 hours default
    public DateTime? LastSyncAt { get; set; }
    public ProviderSyncStatus LastSyncStatus { get; set; } = ProviderSyncStatus.Idle;
    public string? LastSyncMessage { get; set; }
    public int DiscoveredCount { get; set; }
}
