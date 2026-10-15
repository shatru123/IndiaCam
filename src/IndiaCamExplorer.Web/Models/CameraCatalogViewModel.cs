using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Models;

namespace IndiaCamExplorer.Web.Models;

public class CameraCatalogViewModel
{
    public PagedResult<Camera> PagedCameras { get; set; } = new();
    public CameraFilterParams FilterParams { get; set; } = new();
    public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();
    public IEnumerable<string> States { get; set; } = Enumerable.Empty<string>();
    public Category? ActiveCategory { get; set; }
    public string? ActiveState { get; set; }
}
