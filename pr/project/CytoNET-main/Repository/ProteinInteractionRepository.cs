using Cytonet.Data.ProteinInteraction;
using CytoNET.Data.SmallMolecule;
using CytoNET.Models;
using CytoNET.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CytoNET.Repository
{
    public class ProteinInteractionRepository : IProteinInteractionRepository
    {
        private readonly ProteinInteractionDbContext _context;
        private readonly Data.Protein.ProteinDbContext _proteinDbContext;
        private readonly Data.SmallMolecule.SmallMoleculeDbContext _smallMoleculeDbContext;

        public ProteinInteractionRepository(
            ProteinInteractionDbContext context,
            Data.Protein.ProteinDbContext proteinDbContext,
            Data.SmallMolecule.SmallMoleculeDbContext smallMoleculeDbContext
        )
        {
            _context = context;
            _proteinDbContext = proteinDbContext;
            _smallMoleculeDbContext = smallMoleculeDbContext;
        }

        public async Task<
            Tuple<
                List<ProteinInteraction>,
                List<Data.Protein.Protein>,
                Data.SmallMolecule.SmallMolecule,
                bool
            >
        > GetProteinInteractionByProteinId(string uniprotId)
        {
            var isInitiatingProtein = false;
            var results =
                await _context
                    .ProteinInteractions.Where(p =>
                        (
                            p.InitiatingProtein != null
                            && p.InitiatingProtein.UniprotIdCasNumber == uniprotId
                        )
                        || (
                            p.AssociatingProtein != null
                            && p.AssociatingProtein.UniprotIdCasNumber == uniprotId
                        )
                    )
                    .Include(p => p.InitiatingProtein)
                    .ThenInclude(prot => prot != null ? prot.ProteinLevel : null)
                    .Include(p => p.AssociatingProtein)
                    .ThenInclude(prot => prot != null ? prot.ProteinLevel : null)
                    .ToListAsync() ?? new List<ProteinInteraction>();

            if (results.Any())
            {
                isInitiatingProtein = results[0].InitiatingProtein?.UniprotIdCasNumber == uniprotId;
            }
            var smallMolInfo =
                await _smallMoleculeDbContext
                    .SmallMolecules.Where(sM => sM.CasNo == uniprotId)
                    .Include(sM => sM.MediatorCompound)
                    .FirstOrDefaultAsync() ?? new Data.SmallMolecule.SmallMolecule();

            var initiatingProteinIds = results.Select(r => r.InitiatingProteinId).ToList();
            var associatingProteinIds = results.Select(r => r.AssociatingProteinId).ToList();

            var allProteinIds = initiatingProteinIds
                .Concat(associatingProteinIds)
                .Distinct()
                .ToList();

            if (!string.IsNullOrEmpty(smallMolInfo.MediatorCompound?.UniprotId))
                allProteinIds.Add(smallMolInfo.MediatorCompound.UniprotId);

            var additionalInfo =
                await _proteinDbContext
                    .Proteins.Where(p => allProteinIds.Contains(p.UniprotId))
                    .Include(p => p.ProteinLevel)
                    .Include(p => p.Products)
                    .ToListAsync() ?? new List<Data.Protein.Protein>();

            return new Tuple<
                List<ProteinInteraction>,
                List<Data.Protein.Protein>,
                Data.SmallMolecule.SmallMolecule,
                bool
            >(results, additionalInfo, smallMolInfo, isInitiatingProtein);
        }

        public async Task<ProteinInteraction> GetProteinInteractionByShortName(string shortName)
        {
            return await _context
                    .ProteinInteractions.Where(p =>
                        p.InitiatingProtein != null && p.InitiatingProtein.ShortName == shortName
                    )
                    .Include(p => p.InitiatingProtein)
                    .ThenInclude(prot => prot != null ? prot.ProteinLevel : null)
                    .Include(p => p.AssociatingProtein)
                    .ThenInclude(prot => prot != null ? prot.ProteinLevel : null)
                    .FirstOrDefaultAsync() ?? new ProteinInteraction();
        }

        public async Task<IEnumerable<ProteinSearchResult>> SearchProteins(string term)
        {
            term = term.ToLower();

            var initiatingResults = await _context
                .ProteinInteractions.Where(p => p.InitiatingProtein != null)
                .Where(p =>
                    EF.Functions.Like(p.InitiatingProtein.ShortName.ToLower(), $"%{term}%")
                    || EF.Functions.Like(
                        p.InitiatingProtein.UniprotIdCasNumber.ToLower(),
                        $"%{term}%"
                    )
                )
                .Select(p => new ProteinSearchResult
                {
                    Id = p.InitiatingProtein.UniprotIdCasNumber,
                    ShortName = p.InitiatingProtein.ShortName,
                    UniprotId = p.InitiatingProtein.UniprotIdCasNumber,
                })
                .Distinct()
                .Take(10)
                .ToListAsync();

            var associatingResults = await _context
                .ProteinInteractions.Where(p => p.AssociatingProtein != null)
                .Where(p =>
                    EF.Functions.Like(p.AssociatingProtein.ShortName.ToLower(), $"%{term}%")
                    || EF.Functions.Like(
                        p.AssociatingProtein.UniprotIdCasNumber.ToLower(),
                        $"%{term}%"
                    )
                )
                .Select(p => new ProteinSearchResult
                {
                    Id = p.AssociatingProtein.UniprotIdCasNumber,
                    ShortName = p.AssociatingProtein.ShortName,
                    UniprotId = p.AssociatingProtein.UniprotIdCasNumber,
                })
                .Distinct()
                .Take(10)
                .ToListAsync();
            return initiatingResults.Concat(associatingResults).DistinctBy(p => p.UniprotId);
        }
    }
}
