using Microsoft.AspNetCore.Mvc;

namespace IndiaCamExplorer.Web.Controllers;

public class FavoritesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
