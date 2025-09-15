using CytoNET.Data.Protein;
using CytoNET.Data.SmallMolecule;
using CytoNET.Data.TissueDistribution;
using CytoNET.Models;
using CytoNET.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CytoNET.Repository
{
    public class TissueDistributionRepository : ITissueDistributionRepository
    {
        private readonly TissueDistributionDbContext _tissueContext;
        private readonly ProteinDbContext _proteinContext;
        private readonly SmallMoleculeDbContext _smallMoleculeContext;

        public TissueDistributionRepository(
            TissueDistributionDbContext tissueContext,
            ProteinDbContext proteinContext,
            SmallMoleculeDbContext smallMoleculeContext
        )
        {
            _tissueContext = tissueContext;
            _proteinContext = proteinContext;
            _smallMoleculeContext = smallMoleculeContext;
        }

        public async Task<TissueDistribution> GetTissueDistributionById(string uniprotId)
        {
            try
            {
                var distribution = await _tissueContext
                    .TissueDistributions.Include(t => t.ProteinLevel)
                    .Include(t => t.TissueOrgans)
                    .ThenInclude(to => to.TissueOrgan)
                    .Where(t => t.UniprotId == uniprotId)
                    .FirstOrDefaultAsync();

                if (distribution != null)
                {
                    return distribution;
                }

                var mediatorCompound = await _smallMoleculeContext
                    .MediatorCompounds.Include(mc => mc.SmallMolecule)
                    .Where(mc => mc.UniprotId == uniprotId)
                    .FirstOrDefaultAsync();

                if (mediatorCompound != null && mediatorCompound.SmallMolecule != null)
                {
                    return await _tissueContext
                            .TissueDistributions.Include(t => t.ProteinLevel)
                            .Include(t => t.TissueOrgans)
                            .ThenInclude(to => to.TissueOrgan)
                            .Where(t => t.CasNumber == mediatorCompound.SmallMolecule.CasNo)
                            .FirstOrDefaultAsync() ?? new TissueDistribution();
                }

                return new TissueDistribution();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception retrieving tissue distribution by ID: {ex.Message}");
                return new TissueDistribution();
            }
        }

        public async Task<TissueDistribution> GetTissueDistributionByCasNumber(string casNumber)
        {
            try
            {
                var distribution = await _tissueContext
                    .TissueDistributions.Include(t => t.ProteinLevel)
                    .Include(t => t.TissueOrgans)
                    .ThenInclude(to => to.TissueOrgan)
                    .Where(t => t.CasNumber == casNumber)
                    .FirstOrDefaultAsync();

                if (distribution != null)
                {
                    return distribution;
                }

                var smallMolecule = await _smallMoleculeContext
                    .SmallMolecules.Include(sm => sm.MediatorCompound)
                    .Where(sm => sm.CasNo == casNumber)
                    .FirstOrDefaultAsync();

                if (smallMolecule?.MediatorCompound != null)
                {
                    return await _tissueContext
                            .TissueDistributions.Include(t => t.ProteinLevel)
                            .Include(t => t.TissueOrgans)
                            .ThenInclude(to => to.TissueOrgan)
                            .Where(t => t.UniprotId == smallMolecule.MediatorCompound.UniprotId)
                            .FirstOrDefaultAsync() ?? new TissueDistribution();
                }

                return new TissueDistribution();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception retrieving tissue distribution by CAS: {ex.Message}");
                return new TissueDistribution();
            }
        }

        public async Task<List<TissueDistribution>> GetTissueDistributionByTissueOrOrganId(
            string tissueOrganId
        )
        {
            try
            {
                return await _tissueContext
                    .TissueDistributions.Include(t => t.ProteinLevel)
                    .Include(t => t.TissueOrgans)
                    .ThenInclude(to => to.TissueOrgan)
                    .Where(t => t.TissueOrgans.Any(tO => tO.TissueOrganId == tissueOrganId))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Exception retrieving tissue distribution by organ ID: {ex.Message}"
                );
                return new List<TissueDistribution>();
            }
        }

        public async Task<List<TissueDistribution>> GetTissueDistributionByTissueOrOrganName(
            string tissueOrganName
        )
        {
            try
            {
                return await _tissueContext
                    .TissueDistributions.Include(t => t.ProteinLevel)
                    .Include(t => t.TissueOrgans)
                    .ThenInclude(to => to.TissueOrgan)
                    .Where(t =>
                        t.TissueOrgans.Any(tO =>
                            tO.TissueOrgan != null && tO.TissueOrgan.Name == tissueOrganName
                        )
                    )
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Exception retrieving tissue distribution by organ name: {ex.Message}"
                );
                return new List<TissueDistribution>();
            }
        }

        public async Task<string> GetRelatedCasNumberForUniprotId(string uniprotId)
        {
            try
            {
                var mediatorCompound = await _smallMoleculeContext
                    .MediatorCompounds.Include(mc => mc.SmallMolecule)
                    .Where(mc => mc.UniprotId == uniprotId)
                    .FirstOrDefaultAsync();

                if (mediatorCompound?.SmallMolecule != null)
                {
                    return mediatorCompound.SmallMolecule.CasNo;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception retrieving related CAS number: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<List<TissueDistributionSearchResult>> SearchTissueDistributions(
            string searchTerm
        )
        {
            try
            {
                var term = searchTerm.ToLower();

                var tissueUniprotIds = await _tissueContext
                    .TissueDistributions.Select(p => p.UniprotId)
                    .Distinct()
                    .ToHashSetAsync();
                var tissueCasNums = await _tissueContext
                    .TissueDistributions.Select(p => p.CasNumber)
                    .Distinct()
                    .ToHashSetAsync();

                var protResults = await _proteinContext
                    .Proteins.Where(p =>
                        EF.Functions.Like(p.UniprotId.ToLower(), $"%{term}%")
                        || EF.Functions.Like(p.LongName.ToLower(), $"%{term}%")
                        || EF.Functions.Like(p.ShortName.ToLower(), $"%{term}%")
                        || EF.Functions.Like(p.Aliases.ToLower(), $"%{term}%")
                    )
                    .Select(p => new TissueDistributionSearchResult
                    {
                        UniprotIdorCas = p.UniprotId,
                        ShortName = p.ShortName,
                        LongName = p.LongName ?? string.Empty,
                        Alias = p.Aliases,
                    })
                    .Take(10)
                    .ToListAsync();

                var smallMolResults = await _smallMoleculeContext
                    .SmallMolecules.Where(p =>
                        EF.Functions.Like(p.CasNo.ToLower(), $"%{term}%")
                        || EF.Functions.Like(p.MediatorLongName.ToLower(), $"%{term}%")
                        || EF.Functions.Like(p.MediatorShortName.ToLower(), $"%{term}%")
                        || EF.Functions.Like(p.MediatorAlias.ToLower(), $"%{term}%")
                    )
                    .Select(p => new TissueDistributionSearchResult
                    {
                        UniprotIdorCas = p.CasNo,
                        ShortName = p.MediatorShortName,
                        LongName = p.MediatorLongName ?? string.Empty,
                        Alias = p.MediatorAlias,
                        MediatorCompoundShortName =
                            p.MediatorCompound != null ? p.MediatorCompound.ShortName : "",
                        MediatorCompoundUniprotId =
                            p.MediatorCompound != null ? p.MediatorCompound.UniprotId : "",
                    })
                    .Take(10)
                    .ToListAsync();

                var results = smallMolResults.Concat(protResults);

                return results
                    .Where(p =>
                        tissueUniprotIds.Contains(p.UniprotIdorCas)
                        || tissueCasNums.Contains(p.UniprotIdorCas)
                    )
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception searching tissue distributions: {ex.Message}");
                return new List<TissueDistributionSearchResult>();
            }
        }
    }
}
