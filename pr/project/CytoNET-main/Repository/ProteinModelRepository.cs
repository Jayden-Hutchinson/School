using System.Text;
using CytoNET.Data.Protein;
using CytoNET.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CytoNET.Repository
{
    public class ProteinModelRepository : IProteinModelRepository
    {
        private readonly ProteinDbContext _context;

        public ProteinModelRepository(ProteinDbContext context)
        {
            _context = context;
        }

        public Protein GetProteinById(string uniprotId)
        {
            try
            {
                var protein = _context
                    .Proteins.Where(p => p.UniprotId == uniprotId)
                    .FirstOrDefault();

                if (protein != null)
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == uniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }

                return protein;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProteinById: {ex.Message}");
                return null;
            }
        }

        public List<Protein> GetProteinsByType(string type)
        {
            try
            {
                var proteins = _context
                    .Proteins.Where(p => EF.Functions.Like(p.Type.ToLower(), $"%{type.ToLower()}%"))
                    .ToList();

                foreach (var protein in proteins)
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == protein.UniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }

                return proteins;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProteinsByType: {ex.Message}");
                return new List<Protein>();
            }
        }

        public Protein GetProteinByType(string type)
        {
            try
            {
                var protein = _context.Proteins.Where(p => p.Type == type).FirstOrDefault();

                if (protein != null)
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == protein.UniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }

                return protein;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProteinByType: {ex.Message}");
                return null;
            }
        }

        public Protein GetProteinByShortName(string shortName)
        {
            try
            {
                var protein = _context
                    .Proteins.Where(p => p.ShortName == shortName)
                    .FirstOrDefault();

                if (protein != null)
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == protein.UniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }

                return protein;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProteinByShortName: {ex.Message}");
                return null;
            }
        }

        public Protein GetProteinByLongName(string longName)
        {
            try
            {
                var protein = _context.Proteins.Where(p => p.LongName == longName).FirstOrDefault();

                if (protein != null)
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == protein.UniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }

                return protein;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProteinByLongName: {ex.Message}");
                return null;
            }
        }

        public Protein GetProteinByProcessing(string processing)
        {
            try
            {
                var protein = _context
                    .Proteins.Where(p => p.Processing == processing)
                    .FirstOrDefault();

                if (protein != null)
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == protein.UniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }

                return protein;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProteinByProcessing: {ex.Message}");
                return null;
            }
        }

        public Protein GetProteinByPrecursorProteinMass(string precursorProteinMass)
        {
            try
            {
                var protein = _context
                    .Proteins.Where(p => p.PrecursorProteinMass == precursorProteinMass)
                    .FirstOrDefault();

                if (protein != null)
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == protein.UniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }

                return protein;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProteinByPrecursorProteinMass: {ex.Message}");
                return null;
            }
        }

        public Protein GetProteinByFunctionDescription(string functionDescription)
        {
            try
            {
                var protein = _context
                    .Proteins.Where(p => p.FunctionDescription.Contains(functionDescription))
                    .FirstOrDefault();

                if (protein != null)
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == protein.UniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }

                return protein;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProteinByFunctionDescription: {ex.Message}");
                return null;
            }
        }

        public Protein GetProteinCstPhosphositePlusEntryId(string cstPhosphositePlusEntryId)
        {
            try
            {
                var protein = _context
                    .Proteins.Where(p => p.CstPhosphositePlusEntryId == cstPhosphositePlusEntryId)
                    .FirstOrDefault();

                if (protein != null)
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == protein.UniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }

                return protein;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProteinCstPhosphositePlusEntryId: {ex.Message}");
                return null;
            }
        }

        public Protein GetProteinLevelId(string proteinLevelId)
        {
            try
            {
                var protein = _context
                    .Proteins.Where(p => p.ProteinLevelId == proteinLevelId)
                    .FirstOrDefault();

                if (protein != null)
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == protein.UniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }

                return protein;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProteinLevelId: {ex.Message}");
                return null;
            }
        }

        public Protein GetCompleteProtein(string uniprotId)
        {
            try
            {
                var protein = _context
                    .Proteins.Where(p => p.UniprotId == uniprotId)
                    .FirstOrDefault();

                if (protein == null)
                    return null;

                try
                {
                    var aliases = _context
                        .Aliases.Where(a => a.ProteinUniprotId == uniprotId)
                        .Select(a => a.Name)
                        .ToList();

                    if (aliases != null && aliases.Any())
                    {
                        protein.Aliases = string.Join("; ", aliases);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading Aliases: {ex.Message}");
                }

                try
                {
                    protein.Products = _context
                        .Products.Where(p => p.ProteinUniprotId == uniprotId)
                        .ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading Products: {ex.Message}");
                }

                try
                {
                    protein.ProteinLevel = _context.ProteinLevels.FirstOrDefault(pl =>
                        pl.ProteinLevelId == protein.ProteinLevelId
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading ProteinLevel: {ex.Message}");
                }

                return protein;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCompleteProtein: {ex.Message}");
                return null;
            }
        }
    }
}
