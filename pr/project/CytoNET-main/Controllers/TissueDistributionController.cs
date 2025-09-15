using System.Diagnostics;
using System.Linq;
using CytoNET.Data.Protein;
using CytoNET.Data.SmallMolecule;
using CytoNET.Data.TissueDistribution;
using CytoNET.Models;
using CytoNET.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CytoNET.Controllers
{
    public class TissueDistributionController : Controller
    {
        private readonly ILogger<TissueDistributionController> _logger;
        private readonly ITissueDistributionRepository _tissueRepository;
        private readonly IProteinModelRepository _proteinRepository;
        private readonly SmallMoleculeDbContext _smallMoleculeContext;

        public TissueDistributionController(
            ILogger<TissueDistributionController> logger,
            ITissueDistributionRepository tissueRepository,
            IProteinModelRepository proteinRepository,
            SmallMoleculeDbContext smallMoleculeContext
        )
        {
            _logger = logger;
            _tissueRepository = tissueRepository;
            _proteinRepository = proteinRepository;
            _smallMoleculeContext = smallMoleculeContext;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Query));
        }

        public IActionResult Query()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchProteins(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 3)
            {
                return Json(new List<object>());
            }

            try
            {
                var results = await _tissueRepository.SearchTissueDistributions(term);

                return Json(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for proteins with term: {Term}", term);
                return Json(new List<object>());
            }
        }

        public async Task<IActionResult> Result(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(Query));
            }

            try
            {
                var viewModel = new TissueDistributionResultViewModel();

                var distribution = await _tissueRepository.GetTissueDistributionById(id);

                if (distribution == null || string.IsNullOrEmpty(distribution.UniprotId))
                {
                    distribution = await _tissueRepository.GetTissueDistributionByCasNumber(id);
                    if (distribution == null || string.IsNullOrEmpty(distribution.UniprotId))
                    {
                        return NotFound();
                    }
                }

                viewModel.UniprotId = distribution.UniprotId;
                viewModel.TissueDistribution = distribution;

                try
                {
                    var protein = _proteinRepository.GetCompleteProtein(distribution.UniprotId);
                    Console.WriteLine(protein.Type);
                    if (protein != null)
                    {
                        viewModel.Type = protein.Type ?? "Unknown";
                        viewModel.ShortName = protein.ShortName ?? "";
                        viewModel.FullName = protein.LongName ?? "";
                        viewModel.Alias = protein.Aliases ?? "";
                        viewModel.FunctionDescription = protein.FunctionDescription ?? "";
                        viewModel.PhosphoSiteId = protein.CstPhosphositePlusEntryId ?? "";

                        DetermineEntityType(viewModel, protein.Type);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error retrieving protein data for: {Id}",
                        distribution.UniprotId
                    );
                }

                await GetSmallMoleculeData(viewModel);

                if (
                    viewModel.SmallMolecule != null
                    && !viewModel.IsMediator
                    && !viewModel.IsReceptor
                    && !viewModel.IsMediatorEnzyme
                    && !viewModel.IsInteractingProtein
                )
                {
                    viewModel.IsMediator = true;
                }

                if (
                    !viewModel.IsMediator
                    && !viewModel.IsReceptor
                    && !viewModel.IsMediatorEnzyme
                    && !viewModel.IsInteractingProtein
                )
                {
                    viewModel.IsInteractingProtein = true;
                }

                await GetRelationshipData(viewModel);
                Console.WriteLine(viewModel.Type);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tissue distribution for id: {Id}", id);
                return RedirectToAction(nameof(Error));
            }
        }

        private void DetermineEntityType(TissueDistributionResultViewModel viewModel, string type)
        {
            if (string.IsNullOrEmpty(type))
                return;

            var typeLower = type.ToLower();

            viewModel.IsMediatorEnzyme = typeLower.Contains("enzyme");
            viewModel.IsMediator = typeLower.Contains("mediator");
            viewModel.IsReceptor = typeLower.Contains("receptor");
            viewModel.IsInteractingProtein =
                !viewModel.IsMediatorEnzyme && !viewModel.IsMediator && !viewModel.IsReceptor;
        }

        private async Task GetSmallMoleculeData(TissueDistributionResultViewModel viewModel)
        {
            try
            {
                var mediatorCompound = await _smallMoleculeContext
                    .MediatorCompounds.Include(mc => mc.SmallMolecule)
                    .FirstOrDefaultAsync(mc => mc.UniprotId == viewModel.UniprotId);

                if (mediatorCompound?.SmallMolecule != null)
                {
                    viewModel.SmallMolecule = mediatorCompound.SmallMolecule;

                    if (string.IsNullOrEmpty(viewModel.ShortName))
                    {
                        viewModel.ShortName = mediatorCompound.SmallMolecule.MediatorShortName;
                    }

                    if (string.IsNullOrEmpty(viewModel.FullName))
                    {
                        viewModel.FullName = mediatorCompound.SmallMolecule.MediatorLongName;
                    }

                    if (string.IsNullOrEmpty(viewModel.Alias))
                    {
                        viewModel.Alias = mediatorCompound.SmallMolecule.MediatorAlias;
                    }

                    return;
                }

                if (!string.IsNullOrEmpty(viewModel.TissueDistribution.CasNumber))
                {
                    var smallMolecule =
                        await _smallMoleculeContext.SmallMolecules.FirstOrDefaultAsync(sm =>
                            sm.CasNo == viewModel.TissueDistribution.CasNumber
                        );

                    if (smallMolecule != null)
                    {
                        viewModel.SmallMolecule = smallMolecule;

                        if (string.IsNullOrEmpty(viewModel.ShortName))
                        {
                            viewModel.ShortName = smallMolecule.MediatorShortName;
                        }

                        if (string.IsNullOrEmpty(viewModel.FullName))
                        {
                            viewModel.FullName = smallMolecule.MediatorLongName;
                        }

                        if (string.IsNullOrEmpty(viewModel.Alias))
                        {
                            viewModel.Alias = smallMolecule.MediatorAlias;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error retrieving small molecule data for: {Id}",
                    viewModel.UniprotId
                );
            }
        }

        private async Task GetRelationshipData(TissueDistributionResultViewModel viewModel)
        {
            try
            {
                if (viewModel.IsReceptor)
                {
                    string receptorName = viewModel.ShortName?.ToLower() ?? "";

                    var smallMolecules = await _smallMoleculeContext.SmallMolecules.ToListAsync();

                    foreach (var molecule in smallMolecules)
                    {
                        string mediatorName = molecule.MediatorShortName.ToLower();

                        if (
                            receptorName.Contains(mediatorName)
                            || (
                                viewModel.FunctionDescription?.ToLower().Contains(mediatorName)
                                == true
                            )
                        )
                        {
                            viewModel.ReceptorName = molecule.MediatorShortName;
                            viewModel.ReceptorUniprotId = molecule.CasNo;
                            viewModel.ReceptorType = molecule.MediatorType ?? "Unknown";

                            var mediatorCompound =
                                await _smallMoleculeContext.MediatorCompounds.FirstOrDefaultAsync(
                                    mc => mc.SmallMoleculeId == molecule.Id
                                );

                            if (mediatorCompound != null)
                            {
                                var enzymeDistribution =
                                    await _tissueRepository.GetTissueDistributionById(
                                        mediatorCompound.UniprotId
                                    );
                                if (enzymeDistribution?.TissueOrgans != null)
                                {
                                    foreach (var tissueOrgan in enzymeDistribution.TissueOrgans)
                                    {
                                        if (tissueOrgan.TissueOrgan != null)
                                        {
                                            viewModel.ReceptorOrgans.Add(
                                                new OrganViewModel
                                                {
                                                    OrganId = tissueOrgan.TissueOrganId,
                                                    OrganName = tissueOrgan.TissueOrgan.Name,
                                                    IsBold = tissueOrgan.TissueOrgan.IsBold,
                                                }
                                            );
                                        }
                                    }
                                }

                                if (viewModel.ReceptorOrgans.Any())
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
                else if (viewModel.IsMediator || viewModel.IsMediatorEnzyme)
                {
                    string mediatorName = viewModel.ShortName?.ToLower() ?? "";

                    var receptors = _proteinRepository.GetProteinsByType("receptor");

                    foreach (var receptor in receptors)
                    {
                        bool potentialMatch = false;

                        if (
                            receptor.ShortName?.ToLower()?.Contains(mediatorName) == true
                            || receptor.FunctionDescription?.ToLower()?.Contains(mediatorName)
                                == true
                        )
                        {
                            potentialMatch = true;
                        }
                        else if (viewModel.SmallMolecule != null)
                        {
                            string smallMolName =
                                viewModel.SmallMolecule.MediatorShortName.ToLower();
                            if (
                                receptor.ShortName?.ToLower()?.Contains(smallMolName) == true
                                || receptor.FunctionDescription?.ToLower()?.Contains(smallMolName)
                                    == true
                            )
                            {
                                potentialMatch = true;
                            }
                        }

                        if (potentialMatch)
                        {
                            viewModel.ReceptorName = receptor.ShortName;
                            viewModel.ReceptorUniprotId = receptor.UniprotId;

                            var receptorDistribution =
                                await _tissueRepository.GetTissueDistributionById(
                                    receptor.UniprotId
                                );
                            if (receptorDistribution?.TissueOrgans != null)
                            {
                                foreach (var tissueOrgan in receptorDistribution.TissueOrgans)
                                {
                                    if (tissueOrgan.TissueOrgan != null)
                                    {
                                        viewModel.ReceptorOrgans.Add(
                                            new OrganViewModel
                                            {
                                                OrganId = tissueOrgan.TissueOrganId,
                                                OrganName = tissueOrgan.TissueOrgan.Name,
                                                IsBold = tissueOrgan.TissueOrgan.IsBold,
                                            }
                                        );
                                    }
                                }
                            }

                            if (viewModel.ReceptorOrgans.Any())
                            {
                                return;
                            }
                        }
                    }
                }

                if (viewModel.ReceptorOrgans == null || !viewModel.ReceptorOrgans.Any())
                {
                    if (viewModel.IsMediator || viewModel.IsMediatorEnzyme)
                    {
                        viewModel.ReceptorName = "Related Receptors";
                        viewModel.ReceptorUniprotId = "";

                        viewModel.ReceptorOrgans.Add(
                            new OrganViewModel
                            {
                                OrganId = "placeholder",
                                OrganName =
                                    "For information about related receptors, please use the search function.",
                                IsBold = false,
                            }
                        );
                    }
                    else if (viewModel.IsReceptor)
                    {
                        viewModel.ReceptorName = "Related Mediators";
                        viewModel.ReceptorUniprotId = "";

                        viewModel.ReceptorOrgans.Add(
                            new OrganViewModel
                            {
                                OrganId = "placeholder",
                                OrganName =
                                    "For information about related mediators, please use the search function.",
                                IsBold = false,
                            }
                        );
                    }
                    else
                    {
                        viewModel.ReceptorName = "Related Proteins";
                        viewModel.ReceptorUniprotId = "";

                        viewModel.ReceptorOrgans.Add(
                            new OrganViewModel
                            {
                                OrganId = "placeholder",
                                OrganName =
                                    "For information about related proteins, please use the search function.",
                                IsBold = false,
                            }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRelationshipData for: {Id}", viewModel.UniprotId);
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
