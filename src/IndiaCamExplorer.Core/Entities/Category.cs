namespace IndiaCamExplorer.Core.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconName { get; set; } = "camera";
    public int DisplayOrder { get; set; }

    public ICollection<Camera> Cameras { get; set; } = new List<Camera>();
}
