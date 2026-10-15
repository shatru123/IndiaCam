using System.ComponentModel.DataAnnotations;
using IndiaCamExplorer.Core.Enums;

namespace IndiaCamExplorer.Web.Models;

public class AdminCameraEditModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(220)]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Required]
    [Range(-180, 180)]
    public double Longitude { get; set; }

    [Required]
    [Url]
    [StringLength(500)]
    [Display(Name = "Source URL (Official Website)")]
    public string SourceUrl { get; set; } = string.Empty;

    [Required]
    [Url]
    [StringLength(500)]
    [Display(Name = "Stream / Embed / Snapshot URL")]
    public string StreamUrl { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Stream Type")]
    public StreamType StreamType { get; set; } = StreamType.Hls;

    [Url]
    [StringLength(500)]
    [Display(Name = "Thumbnail URL")]
    public string? ThumbnailUrl { get; set; }

    [Required]
    [StringLength(200)]
    public string Attribution { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Verified Official")]
    public bool IsVerified { get; set; } = true;

    [Display(Name = "Health Status")]
    public HealthStatus HealthStatus { get; set; } = HealthStatus.Online;
}
