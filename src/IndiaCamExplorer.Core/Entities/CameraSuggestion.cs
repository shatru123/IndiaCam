using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Core.Entities;

public class CameraSuggestion
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string StreamUrl { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string? SubmitterName { get; set; }
    public string? SubmitterEmail { get; set; }
    public string? Notes { get; set; }

    public SuggestionStatus Status { get; set; } = SuggestionStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
