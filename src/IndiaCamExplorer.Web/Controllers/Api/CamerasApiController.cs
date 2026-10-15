using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace IndiaCamExplorer.Web.Controllers.Api;

[ApiController]
[Route("api/cameras")]
public class CamerasApiController : ControllerBase
{
    private readonly ICameraRepository _cameraRepository;
    private readonly IBrokenStreamReportRepository _reportRepository;

    public CamerasApiController(
        ICameraRepository cameraRepository,
        IBrokenStreamReportRepository reportRepository)
    {
        _cameraRepository = cameraRepository;
        _reportRepository = reportRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetCameras([FromQuery] CameraFilterParams filterParams)
    {
        var result = await _cameraRepository.GetCamerasAsync(filterParams);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var camera = await _cameraRepository.GetByIdAsync(id);
        if (camera == null) return NotFound();
        return Ok(camera);
    }

    [HttpGet("{id:int}/health")]
    public async Task<IActionResult> GetHealthHistory(int id)
    {
        var history = await _cameraRepository.GetRecentHealthChecksAsync(id, 15);
        return Ok(history);
    }

    [HttpPost("{id:int}/report")]
    public async Task<IActionResult> ReportBrokenStream(int id, [FromBody] BrokenStreamReportRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var camera = await _cameraRepository.GetByIdAsync(id);
        if (camera == null)
            return NotFound(new { message = "Camera not found." });

        var report = new BrokenStreamReport
        {
            CameraId = id,
            Reason = request.Reason.Trim(),
            Notes = request.Notes?.Trim(),
            ReporterIpHash = HttpContext?.Connection?.RemoteIpAddress?.ToString()
        };

        await _reportRepository.AddAsync(report);

        return Ok(new { success = true, message = "Thank you. Your report has been submitted to administrators for review." });
    }

    [HttpPost("favorites")]
    public async Task<IActionResult> GetFavoritesByIds([FromBody] List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return Ok(new List<Camera>());

        var result = new List<Camera>();
        foreach (var id in ids.Take(50))
        {
            var camera = await _cameraRepository.GetByIdAsync(id);
            if (camera != null && camera.IsActive)
            {
                result.Add(camera);
            }
        }

        return Ok(result);
    }
}
