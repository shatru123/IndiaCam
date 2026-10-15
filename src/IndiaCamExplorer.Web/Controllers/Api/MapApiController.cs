using IndiaCamExplorer.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndiaCamExplorer.Web.Controllers.Api;

[ApiController]
[Route("api/map")]
public class MapApiController : ControllerBase
{
    private readonly ICameraRepository _cameraRepository;

    public MapApiController(ICameraRepository cameraRepository)
    {
        _cameraRepository = cameraRepository;
    }

    [HttpGet("markers")]
    public async Task<IActionResult> GetMarkers()
    {
        var markers = await _cameraRepository.GetMarkersAsync();
        return Ok(markers);
    }
}
