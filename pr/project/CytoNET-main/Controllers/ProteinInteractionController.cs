using System.Diagnostics;
using System.Linq;
using CytoNET.Models;
using CytoNET.Repository;
using CytoNET.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CytoNET.Data.ProteinInteraction;

public class ProteinInteractionController : Controller
{
    private readonly ILogger<ProteinInteractionController> _logger;
    private readonly IProteinInteractionRepository _repository;

    public ProteinInteractionController(
        ILogger<ProteinInteractionController> logger,
        IProteinInteractionRepository repository
    )
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<IActionResult> Result(
        string id,
        bool showStimulatory = true,
        bool showInhibitory = true,
        bool showUndefined = true,
        bool showUniProtId = true,
        bool showBlackBackground = true
    )
    {
        try
        {
            _logger.LogInformation("Attempting to retrieve protein interaction for ID: {Id}", id);

            var results = await _repository.GetProteinInteractionByProteinId(id);

            if (results.Item1.Count <= 0)
            {
                _logger.LogWarning("No protein interactions found for ID: {Id}", id);
                TempData["Message"] =
                    $"No protein interactions found for '{id}'. Please try a different identifier.";
                return RedirectToAction(nameof(Query));
            }

            var viewModel = new ProteinInteractionViewModel(
                results.Item1,
                results.Item2,
                results.Item3,
                results.Item4
            );
            viewModel.ShowStimulatory = showStimulatory;
            viewModel.ShowInhibitory = showInhibitory;
            viewModel.ShowUndefined = showUndefined;
            viewModel.ShowUniProtId = showUniProtId;
            viewModel.ShowBlackBackground = showBlackBackground;
            viewModel.UniprotId = id;
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving protein interaction with uniprot or cas number of {Id}. Exception: {ExMessage}",
                id,
                ex.Message
            );
            TempData["ErrorMessage"] = $"Error retrieving data for '{id}': {ex.Message}";
            return RedirectToAction(nameof(Query), "ProteinInteraction");
        }
    }

    public IActionResult Query()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SearchProteins(string term)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
        {
            return BadRequest("Search term must be at least 3 characters long");
        }

        try
        {
            var results = await _repository.SearchProteins(term);

            _logger.LogInformation(
                "Found {count} protein results for term: {term}",
                results?.Count() ?? 0,
                term
            );

            return Json(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for proteins with term: {Term}", term);
            return StatusCode(500, "An error occurred while searching for proteins");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            "Error",
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
