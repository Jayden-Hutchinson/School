using CytoNET.Data.Protein;
using Cytonet.Data.ProteinInteraction;
using CytoNET.Data.ProteinInteraction;
using CytoNET.Data.ProteinModification;
using CytoNET.Models;
using CytoNET.Repository.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SQLitePCL;

namespace CytoNET.Repository
{
    public class ProteinModificationRepository : IProteinModificationRepository
    {
        private readonly ProteinModificationDbContext _context;
        private readonly Data.Protein.ProteinDbContext _proteinDbContext;
        private readonly ProteinInteractionDbContext _proteinInteractionDbContext;

        public ProteinModificationRepository(
            ProteinModificationDbContext context,
            Data.Protein.ProteinDbContext proteinDbContext,
            ProteinInteractionDbContext proteinInteractionDbContext
        )
        {
            _context = context;
            _proteinDbContext = proteinDbContext;
        }

        // public ProteinModification GetProteinModificationById(string uniprotId)
        // {
        //     return _context
        //             .ProteinModifications.Include(pm => pm.Product)
        //             .Include(pm => pm.Modification)
        //             .Include(pm => pm.ProteinLevel)
        //             .FirstOrDefault(pm => pm.UniprotId == uniprotId.ToString())
        //         ?? new ProteinModification();
        // }

        // public ProteinModification GetProteinModificationWithAllLinks(string uniprotId)
        // {
        //     return _context
        //             .ProteinModifications.Include(pm => pm.Product)
        //             .Include(pm => pm.Modification)
        //             .Include(pm => pm.ProteinLevel)
        //             .FirstOrDefault(pm => pm.UniprotId == uniprotId.ToString())
        //         ?? new ProteinModification();
        // }
        public (Data.Protein.Protein, List<ProteinModification>) GetProteinModificationData(
            string UniprotId
        )
        {
            var ProteinInfo = _proteinDbContext
                .Proteins.Where(p => p.UniprotId == UniprotId)
                .First();
            var Count = _proteinDbContext.Proteins.ToList().Count();
            Console.WriteLine(Count);

            var proteinModifications = _context
                .ProteinModifications.Where(p => p.UniprotId == UniprotId)
                .ToList()
                .Select(p => new ProteinModification
                {
                    Id = p.Id,
                    Modification = _context
                        .Modifications.Where(m => m.Id == p.ModificationId)
                        .Select(m => new Modification
                        {
                            EffectCode = m.EffectCode,
                            Description = m.Description,
                            Id = m.Id,
                            Type = m.Type,
                            Color = m.Color,
                            Shape = m.Shape,
                        })
                        .FirstOrDefault(),
                    ModificationId = p.ModificationId,
                    ModificationDescription = p.ModificationDescription,
                    ProductId = p.ProductId,
                    Products = _context
                        .PSiteAbProducts.Where(product => product.ProteinModificationId == p.Id)
                        .Select(product => new PSiteAbProduct
                        {
                            ProductId = product.ProductId,
                            ProteinUniprotId = product.ProteinUniprotId,
                            Name = product.Name,
                            Link = product.Link,
                            ProteinModificationId = p.Id,
                        })
                        .ToList(),
                    ProteinLevelId = p.ProteinLevelId,
                    ProteinLevel = _context
                        .ProteinLevels.Where(pl => pl.ProteinLevelId == p.ProteinLevelId)
                        .Select(pl => new Data.ProteinModification.ProteinLevel
                        {
                            Category = pl.Category,
                            ProteinLevelId = pl.ProteinLevelId,
                            ProteinModifications = new List<ProteinModification>(),
                        })
                        .FirstOrDefault(),
                    Site = p.Site,
                    UniprotId = p.UniprotId,
                    Description = p.Description,
                })
                .ToList();

            return (ProteinInfo, proteinModifications);
        }

        public async Task<IEnumerable<ProteinModificationSearchResult>> SearchProteinModification(
            string term
        )
        {
            term = term.ToLower();
            var modificationUniprotIds = await _context
                .ProteinModifications.Select(p => p.UniprotId)
                .Distinct()
                .ToHashSetAsync();

            var results = await _proteinDbContext
                .Proteins.Where(p =>
                    EF.Functions.Like(p.UniprotId.ToLower(), $"%{term}%")
                    || EF.Functions.Like(p.LongName.ToLower(), $"%{term}%")
                    || EF.Functions.Like(p.ShortName.ToLower(), $"%{term}%")
                    || EF.Functions.Like(p.Aliases.ToLower(), $"%{term}%")
                )
                .Select(p => new ProteinModificationSearchResult
                {
                    UniprotId = p.UniprotId,
                    ShortName = p.ShortName,
                    LongName = p.LongName ?? string.Empty,
                    Alias = p.Aliases,
                })
                .Take(10)
                .ToListAsync();

            return results.Where(p => modificationUniprotIds.Contains(p.UniprotId)).ToList();
        }
    }
}
