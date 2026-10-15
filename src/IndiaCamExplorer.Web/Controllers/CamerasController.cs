using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;
using IndiaCamExplorer.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace IndiaCamExplorer.Web.Controllers;

public class CamerasController : Controller
{
    private readonly ICameraRepository _cameraRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;

    public CamerasController(
        ICameraRepository cameraRepository,
        ICategoryRepository categoryRepository,
        ILocationRepository locationRepository)
    {
        _cameraRepository = cameraRepository;
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<IActionResult> Index([FromQuery] CameraFilterParams filterParams)
    {
        var pagedResult = await _cameraRepository.GetCamerasAsync(filterParams);
        var categories = await _categoryRepository.GetAllAsync();
        var states = await _locationRepository.GetAllStatesAsync();

        Category? activeCategory = null;
        if (!string.IsNullOrWhiteSpace(filterParams.CategorySlug))
        {
            activeCategory = await _categoryRepository.GetBySlugAsync(filterParams.CategorySlug);
        }

        var vm = new CameraCatalogViewModel
        {
            PagedCameras = pagedResult,
            FilterParams = filterParams,
            Categories = categories,
            States = states,
            ActiveCategory = activeCategory,
            ActiveState = filterParams.State
        };

        return View("Index", vm);
    }

    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var camera = await _cameraRepository.GetBySlugAsync(slug);
        if (camera == null)
            return NotFound();

        var recentChecks = await _cameraRepository.GetRecentHealthChecksAsync(camera.Id, 10);
        var related = await _cameraRepository.GetCamerasAsync(new CameraFilterParams
        {
            CategorySlug = camera.Category.Slug,
            PageSize = 4
        });

        var vm = new CameraDetailViewModel
        {
            Camera = camera,
            RecentHealthChecks = recentChecks,
            RelatedCameras = related.Items.Where(c => c.Id != camera.Id).Take(3)
        };

        return View(vm);
    }

    public async Task<IActionResult> State(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return RedirectToAction(nameof(Index));

        var filterParams = new CameraFilterParams
        {
            State = state,
            Page = 1,
            PageSize = 12
        };

        return await Index(filterParams);
    }

    public async Task<IActionResult> Category(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return RedirectToAction(nameof(Index));

        var filterParams = new CameraFilterParams
        {
            CategorySlug = slug,
            Page = 1,
            PageSize = 12
        };

        return await Index(filterParams);
    }
}
