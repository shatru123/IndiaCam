using IndiaCamExplorer.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndiaCamExplorer.Web.Controllers.Api;

[ApiController]
[Route("api/health")]
public class HealthApiController : ControllerBase
{
    private readonly ICameraRepository _cameraRepository;

    public HealthApiController(ICameraRepository cameraRepository)
    {
        _cameraRepository = cameraRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        var total = await _cameraRepository.GetTotalCountAsync();
        var online = await _cameraRepository.GetOnlineCountAsync();

        return Ok(new
        {
            status = "healthy",
            product = "IndiaCam Explorer",
            tagline = "Explore India, Live.",
            timestampUtc = DateTime.UtcNow,
            cameraStats = new
            {
                totalCameras = total,
                onlineCameras = online,
                offlineCameras = total - online
            }
        });
    }
}
