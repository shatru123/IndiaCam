using IndiaCamExplorer.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndiaCamExplorer.Web.Controllers;

public class MapController : Controller
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;

    public MapController(
        ICategoryRepository categoryRepository,
        ILocationRepository locationRepository)
    {
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.States = await _locationRepository.GetAllStatesAsync();
        return View();
    }
}
