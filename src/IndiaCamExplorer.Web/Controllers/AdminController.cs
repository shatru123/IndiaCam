using System.Security.Claims;
using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Enums;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;
using IndiaCamExplorer.Infrastructure.BackgroundServices;
using IndiaCamExplorer.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndiaCamExplorer.Web.Controllers;

public class AdminController : Controller
{
    private readonly ICameraRepository _cameraRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ISourceProviderRepository _providerRepository;
    private readonly IBrokenStreamReportRepository _reportRepository;
    private readonly ICameraSuggestionRepository _suggestionRepository;
    private readonly IHealthCheckService _healthCheckService;
    private readonly ISecurityValidator _securityValidator;
    private readonly IConfiguration _configuration;
    private readonly CameraSyncBackgroundService _syncService;

    public AdminController(
        ICameraRepository cameraRepository,
        ICategoryRepository categoryRepository,
        ILocationRepository locationRepository,
        ISourceProviderRepository providerRepository,
        IBrokenStreamReportRepository reportRepository,
        ICameraSuggestionRepository suggestionRepository,
        IHealthCheckService healthCheckService,
        ISecurityValidator securityValidator,
        IConfiguration configuration,
        CameraSyncBackgroundService syncService)
    {
        _cameraRepository = cameraRepository;
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
        _providerRepository = providerRepository;
        _reportRepository = reportRepository;
        _suggestionRepository = suggestionRepository;
        _healthCheckService = healthCheckService;
        _securityValidator = securityValidator;
        _configuration = configuration;
        _syncService = syncService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
        {
            return RedirectToAction(nameof(Index));
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var configuredUser = _configuration["AdminSettings:Username"] ?? "admin";
        var configuredPass = _configuration["AdminSettings:Password"] ?? "IndiaCamAdmin2026!";

        if (model.Username == configuredUser && model.Password == configuredPass)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, model.Username),
                new(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Invalid administrator credentials.");
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var total = await _cameraRepository.GetTotalCountAsync();
        var online = await _cameraRepository.GetOnlineCountAsync();
        var providers = await _providerRepository.GetAllAsync();
        var reports = await _reportRepository.GetAllAsync();
        var suggestions = await _suggestionRepository.GetAllAsync();
        var recentCameras = await _cameraRepository.GetRecentlyAddedCamerasAsync(10);

        var vm = new AdminDashboardViewModel
        {
            TotalCameras = total,
            OnlineCameras = online,
            DegradedCameras = 0,
            OfflineCameras = total - online,
            PendingReports = reports.Count(r => r.Status == ReportStatus.Pending),
            PendingSuggestions = suggestions.Count(s => s.Status == SuggestionStatus.Pending),
            Providers = providers,
            RecentReports = reports.Take(5),
            RecentCameras = recentCameras
        };

        return View(vm);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cameras([FromQuery] CameraFilterParams filterParams)
    {
        filterParams.PageSize = 25;
        var result = await _cameraRepository.GetCamerasAsync(filterParams);
        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.States = await _locationRepository.GetAllStatesAsync();
        return View(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCamera()
    {
        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.States = await _locationRepository.GetAllStatesAsync();
        return View(new AdminCameraEditModel
        {
            IsActive = true,
            IsVerified = true,
            HealthStatus = HealthStatus.Online
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCamera(AdminCameraEditModel model)
    {
        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.States = await _locationRepository.GetAllStatesAsync();

        if (!ModelState.IsValid)
            return View(model);

        if (!_securityValidator.IsSafePublicUrl(model.StreamUrl, out var streamErr))
        {
            ModelState.AddModelError(nameof(model.StreamUrl), $"Stream URL rejected: {streamErr}");
            return View(model);
        }

        if (!_securityValidator.IsSafePublicUrl(model.SourceUrl, out var sourceErr))
        {
            ModelState.AddModelError(nameof(model.SourceUrl), $"Source URL rejected: {sourceErr}");
            return View(model);
        }

        var camera = new Camera
        {
            Title = model.Title.Trim(),
            Slug = model.Slug.Trim().ToLowerInvariant(),
            Description = model.Description?.Trim(),
            CategoryId = model.CategoryId,
            State = model.State.Trim(),
            City = model.City.Trim(),
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            SourceUrl = model.SourceUrl.Trim(),
            StreamUrl = model.StreamUrl.Trim(),
            StreamType = model.StreamType,
            ThumbnailUrl = model.ThumbnailUrl?.Trim(),
            Attribution = model.Attribution.Trim(),
            IsActive = model.IsActive,
            IsVerified = model.IsVerified,
            HealthStatus = model.HealthStatus,
            LastVerifiedAt = DateTime.UtcNow
        };

        await _cameraRepository.AddAsync(camera);
        TempData["SuccessMessage"] = $"Camera '{camera.Title}' created successfully.";
        return RedirectToAction(nameof(Cameras));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditCamera(int id)
    {
        var camera = await _cameraRepository.GetByIdAsync(id);
        if (camera == null) return NotFound();

        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.States = await _locationRepository.GetAllStatesAsync();

        var model = new AdminCameraEditModel
        {
            Id = camera.Id,
            Title = camera.Title,
            Slug = camera.Slug,
            Description = camera.Description,
            CategoryId = camera.CategoryId,
            State = camera.State,
            City = camera.City,
            Latitude = camera.Latitude,
            Longitude = camera.Longitude,
            SourceUrl = camera.SourceUrl,
            StreamUrl = camera.StreamUrl,
            StreamType = camera.StreamType,
            ThumbnailUrl = camera.ThumbnailUrl,
            Attribution = camera.Attribution,
            IsActive = camera.IsActive,
            IsVerified = camera.IsVerified,
            HealthStatus = camera.HealthStatus
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCamera(int id, AdminCameraEditModel model)
    {
        if (id != model.Id) return BadRequest();

        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.States = await _locationRepository.GetAllStatesAsync();

        if (!ModelState.IsValid)
            return View(model);

        var camera = await _cameraRepository.GetByIdAsync(id);
        if (camera == null) return NotFound();

        if (!_securityValidator.IsSafePublicUrl(model.StreamUrl, out var streamErr))
        {
            ModelState.AddModelError(nameof(model.StreamUrl), $"Stream URL rejected: {streamErr}");
            return View(model);
        }

        camera.Title = model.Title.Trim();
        camera.Slug = model.Slug.Trim().ToLowerInvariant();
        camera.Description = model.Description?.Trim();
        camera.CategoryId = model.CategoryId;
        camera.State = model.State.Trim();
        camera.City = model.City.Trim();
        camera.Latitude = model.Latitude;
        camera.Longitude = model.Longitude;
        camera.SourceUrl = model.SourceUrl.Trim();
        camera.StreamUrl = model.StreamUrl.Trim();
        camera.StreamType = model.StreamType;
        camera.ThumbnailUrl = model.ThumbnailUrl?.Trim();
        camera.Attribution = model.Attribution.Trim();
        camera.IsActive = model.IsActive;
        camera.IsVerified = model.IsVerified;
        camera.HealthStatus = model.HealthStatus;

        await _cameraRepository.UpdateAsync(camera);
        TempData["SuccessMessage"] = $"Camera '{camera.Title}' updated successfully.";
        return RedirectToAction(nameof(Cameras));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCamera(int id)
    {
        await _cameraRepository.DeleteAsync(id);
        TempData["SuccessMessage"] = "Camera deleted successfully.";
        return RedirectToAction(nameof(Cameras));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCameraStatus(int id)
    {
        var camera = await _cameraRepository.GetByIdAsync(id);
        if (camera != null)
        {
            camera.IsActive = !camera.IsActive;
            await _cameraRepository.UpdateAsync(camera);
            TempData["SuccessMessage"] = $"Camera is now {(camera.IsActive ? "active" : "inactive")}.";
        }
        return RedirectToAction(nameof(Cameras));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RunHealthCheck(int id)
    {
        var camera = await _cameraRepository.GetByIdAsync(id);
        if (camera == null) return NotFound();

        var check = await _healthCheckService.CheckCameraHealthAsync(camera);
        camera.HealthStatus = check.Status;
        camera.LastCheckedAt = DateTime.UtcNow;
        await _cameraRepository.UpdateAsync(camera);

        TempData["SuccessMessage"] = $"Health check completed: Status is {check.Status} ({check.ResponseTimeMs}ms).";
        return RedirectToAction(nameof(Cameras));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Providers()
    {
        var providers = await _providerRepository.GetAllAsync();
        return View(providers);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TriggerSync()
    {
        await _syncService.SynchronizeProvidersAsync(CancellationToken.None);
        TempData["SuccessMessage"] = "Provider synchronization completed successfully.";
        return RedirectToAction(nameof(Providers));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reports([FromQuery] ReportStatus? status)
    {
        var reports = await _reportRepository.GetAllAsync(status);
        ViewBag.ActiveFilter = status;
        return View(reports);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateReportStatus(int id, ReportStatus status, string? notes)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report != null)
        {
            report.Status = status;
            report.AdminResolutionNotes = notes;
            if (status == ReportStatus.Resolved || status == ReportStatus.Dismissed)
            {
                report.ResolvedAt = DateTime.UtcNow;
            }
            await _reportRepository.UpdateAsync(report);
            TempData["SuccessMessage"] = $"Report #{id} status changed to {status}.";
        }
        return RedirectToAction(nameof(Reports));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Suggestions([FromQuery] SuggestionStatus? status)
    {
        var suggestions = await _suggestionRepository.GetAllAsync(status);
        ViewBag.ActiveFilter = status;
        return View(suggestions);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSuggestionStatus(int id, SuggestionStatus status)
    {
        var suggestion = await _suggestionRepository.GetByIdAsync(id);
        if (suggestion != null)
        {
            suggestion.Status = status;
            await _suggestionRepository.UpdateAsync(suggestion);
            TempData["SuccessMessage"] = $"Suggestion #{id} marked as {status}.";
        }
        return RedirectToAction(nameof(Suggestions));
    }
}
