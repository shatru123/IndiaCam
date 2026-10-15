namespace IndiaCamExplorer.Core.Models;

public class StateSummaryDto
{
    public string State { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int TotalCameras { get; set; }
    public int OnlineCameras { get; set; }
}
