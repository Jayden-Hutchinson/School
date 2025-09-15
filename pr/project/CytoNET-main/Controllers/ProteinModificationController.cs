using System.Diagnostics;
using CytoNET.Data.ProteinModification;
using CytoNET.Models;
using CytoNET.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CytoNET.Controllers
{
    public class ProteinModification : Controller
    {
        private readonly ILogger<ProteinModification> _logger;
        private readonly IProteinModificationRepository _repository;

        public ProteinModification(
            ILogger<ProteinModification> logger,
            IProteinModificationRepository repository
        )
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult Query()
        {
            return View();
        }

        public IActionResult Result(
            string id,
            bool showStimulatory = true,
            bool showInhibitory = true,
            bool showUndefined = true,
            bool showUniProtId = true,
            bool showBlackBackground = true
        )
        {
            var UniprotId = id;
            if (string.IsNullOrEmpty(UniprotId))
            {
                return View(new ProteinModificationViewModel());
            }

            try
            {
                var data = _repository.GetProteinModificationData(UniprotId);
                if (data.Item2 == null || data.Item2.Count() == 0)
                {
                    ViewBag.ErrorMessage =
                        $"Protein modification with UniprotId {UniprotId} not found";
                    return View(new ProteinModificationViewModel());
                }

                var viewModel = new ProteinModificationViewModel
                {
                    Protein = data.Item1,
                    Sites = data.Item2,
                };
                viewModel.ShowStimulatory = showStimulatory;
                viewModel.ShowInhibitory = showInhibitory;
                viewModel.ShowUndefined = showUndefined;
                viewModel.ShowUniProtId = showUniProtId;
                viewModel.ShowBlackBackground = showBlackBackground;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error retrieving protein modification with UniprotId {UniprotId}"
                );
                ViewBag.ErrorMessage =
                    "An error occurred while retrieving the protein modification.";
                return View(new ProteinModificationViewModel());
            }
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
                var results = await _repository.SearchProteinModification(term);

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
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
