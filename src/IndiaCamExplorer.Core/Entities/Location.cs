namespace IndiaCamExplorer.Core.Entities;

public class Location
{
    public int Id { get; set; }
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Region { get; set; } = "North"; // North, South, East, West, Central, NorthEast, Islands
}
