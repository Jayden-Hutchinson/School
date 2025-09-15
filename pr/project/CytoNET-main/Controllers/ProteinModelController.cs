using System.Diagnostics;
using CytoNET.Data.Protein;
using CytoNET.Models;
using CytoNET.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CytoNET.Controllers
{
    public class ProteinModelController : Controller
    {
        private readonly ILogger<Protein> _logger;
        private readonly IProteinModelRepository _repository;

        public ProteinModelController(ILogger<Protein> logger, IProteinModelRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult Query()
        {
            return View();
        }

        public IActionResult Result(string UniprotId)
        {
            if (string.IsNullOrEmpty(UniprotId))
            {
                return View(new ProteinModelViewModel());
            }

            try
            {
                var proteinModel = _repository.GetCompleteProtein(UniprotId);

                if (proteinModel == null || string.IsNullOrEmpty(proteinModel.UniprotId))
                {
                    ViewBag.ErrorMessage = $"Protein with Uniprot Id, {UniprotId}, not found.";
                    return View(new ProteinModelViewModel());
                }

                var viewModel = new ProteinModelViewModel
                {
                    protein = proteinModel,
                    alias = proteinModel.Aliases,
                    product = proteinModel.Products.FirstOrDefault(),
                    proteinLevel = proteinModel.ProteinLevel,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving protein with Uniprot Id, {UniprotId}");
                ViewBag.ErrorMessage = "An error occurred while retrieving the protein.";
                return View(new ProteinModelViewModel());
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
