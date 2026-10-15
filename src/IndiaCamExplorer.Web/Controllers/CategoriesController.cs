using IndiaCamExplorer.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndiaCamExplorer.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryRepository.GetCategoriesWithCountsAsync();
        return View(categories);
    }
}
