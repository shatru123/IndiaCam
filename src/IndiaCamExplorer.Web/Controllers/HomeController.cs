using System.Diagnostics;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace IndiaCamExplorer.Web.Controllers;

public class HomeController : Controller
{
    private readonly ICameraRepository _cameraRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;

    public HomeController(
        ICameraRepository cameraRepository,
        ICategoryRepository categoryRepository,
        ILocationRepository locationRepository)
    {
        _cameraRepository = cameraRepository;
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<IActionResult> Index()
    {
        var featuredTask = _cameraRepository.GetFeaturedCamerasAsync(6);
        var recentTask = _cameraRepository.GetRecentlyAddedCamerasAsync(6);
        var categoriesTask = _categoryRepository.GetCategoriesWithCountsAsync();
        var stateSummariesTask = _cameraRepository.GetStateSummariesAsync();
        var totalCountTask = _cameraRepository.GetTotalCountAsync();
        var onlineCountTask = _cameraRepository.GetOnlineCountAsync();
        var allStatesTask = _locationRepository.GetAllStatesAsync();

        await Task.WhenAll(featuredTask, recentTask, categoriesTask, stateSummariesTask, totalCountTask, onlineCountTask, allStatesTask);

        var vm = new HomeViewModel
        {
            FeaturedCameras = await featuredTask,
            RecentlyAdded = await recentTask,
            Categories = await categoriesTask,
            StateSummaries = await stateSummariesTask,
            TotalCameras = await totalCountTask,
            OnlineCameras = await onlineCountTask,
            TotalStates = (await allStatesTask).Count()
        };

        return View(vm);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult AcceptableUse()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
