using System.ComponentModel.DataAnnotations;

namespace IndiaCamExplorer.Core.Models;

public class CameraSuggestionRequest
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [Url]
    [StringLength(500)]
    public string StreamUrl { get; set; } = string.Empty;

    [Required]
    [Url]
    [StringLength(500)]
    public string SourceUrl { get; set; } = string.Empty;

    [StringLength(100)]
    public string? SubmitterName { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? SubmitterEmail { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
