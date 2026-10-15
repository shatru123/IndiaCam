using IndiaCamExplorer.Core.Entities;
using IndiaCamExplorer.Core.Interfaces;
using IndiaCamExplorer.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace IndiaCamExplorer.Web.Controllers;

public class SuggestController : Controller
{
    private readonly ICameraSuggestionRepository _suggestionRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ISecurityValidator _securityValidator;

    public SuggestController(
        ICameraSuggestionRepository suggestionRepository,
        ILocationRepository locationRepository,
        ISecurityValidator securityValidator)
    {
        _suggestionRepository = suggestionRepository;
        _locationRepository = locationRepository;
        _securityValidator = securityValidator;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.States = await _locationRepository.GetAllStatesAsync();
        return View(new CameraSuggestionRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CameraSuggestionRequest request)
    {
        ViewBag.States = await _locationRepository.GetAllStatesAsync();

        if (!ModelState.IsValid)
            return View(request);

        if (!_securityValidator.IsSafePublicUrl(request.StreamUrl, out var streamErr))
        {
            ModelState.AddModelError(nameof(request.StreamUrl), $"Stream URL is invalid: {streamErr}");
            return View(request);
        }

        if (!_securityValidator.IsSafePublicUrl(request.SourceUrl, out var sourceErr))
        {
            ModelState.AddModelError(nameof(request.SourceUrl), $"Source website URL is invalid: {sourceErr}");
            return View(request);
        }

        var suggestion = new CameraSuggestion
        {
            Title = request.Title.Trim(),
            State = request.State.Trim(),
            City = request.City.Trim(),
            StreamUrl = request.StreamUrl.Trim(),
            SourceUrl = request.SourceUrl.Trim(),
            SubmitterName = request.SubmitterName?.Trim(),
            SubmitterEmail = request.SubmitterEmail?.Trim(),
            Notes = request.Notes?.Trim()
        };

        await _suggestionRepository.AddAsync(suggestion);
        return RedirectToAction(nameof(Success));
    }

    public IActionResult Success()
    {
        return View();
    }
}
