using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CytoNET.Data.SmallMolecule
{
    public class SmallMoleculeDbContext : DbContext
    {
        public DbSet<SmallMolecule> SmallMolecules { get; set; }
        public DbSet<MediatorCompound> MediatorCompounds { get; set; }

        public SmallMoleculeDbContext(DbContextOptions<SmallMoleculeDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SmallMolecule>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<MediatorCompound>(entity =>
            {
                entity.HasKey(e => e.UniprotId);

                entity
                    .HasOne(e => e.SmallMolecule)
                    .WithOne(e => e.MediatorCompound)
                    .HasForeignKey<MediatorCompound>(e => e.SmallMoleculeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

    public class SmallMolecule
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string CasNo { get; set; } = string.Empty;

        [Required]
        public string MediatorShortName { get; set; } = string.Empty;

        [Required]
        public string MediatorLongName { get; set; } = string.Empty;

        [Required]
        public string MediatorAlias { get; set; } = string.Empty;

        [Required]
        public string MediatorType { get; set; } = string.Empty;

        [Required]
        public string PrecursorMediatorProteinMass { get; set; }

        [Required]
        public string MatMedProtAaPositions { get; set; } = string.Empty;

        [Required]
        public string HormoneStatus { get; set; } = string.Empty;

        [Required]
        public string CytokineStatus { get; set; } = string.Empty;

        [Required]
        public string NeurotransmitterStatus { get; set; } = string.Empty;

        [Required]
        public string MediatorId { get; set; } = string.Empty;

        [Required]
        public string ProteaseInfo { get; set; } = string.Empty;

        public string CasLink { get; set; } = string.Empty;
        public string PubChemLink { get; set; } = string.Empty;
        public string ChemSpiderLink { get; set; } = string.Empty;
        public string ChemBlLink { get; set; } = string.Empty;

        public MediatorCompound? MediatorCompound { get; set; }
    }

    public class MediatorCompound
    {
        public string UniprotId { get; set; } = string.Empty;

        [Required]
        public string SmallMoleculeId { get; set; } = string.Empty;

        [Required]
        public double Mass { get; set; }

        [Required]
        public string PubchemNum { get; set; } = string.Empty;

        [Required]
        public string ChemspiderNum { get; set; } = string.Empty;

        [Required]
        public string ChemblNum { get; set; } = string.Empty;

        [Required]
        public string BiosynthNum { get; set; } = string.Empty;

        [Required]
        public string LongName { get; set; } = string.Empty;

        [Required]
        public string ShortName { get; set; } = string.Empty;

        [Required]
        public string Alias { get; set; } = string.Empty;

        [Required]
        public string PhosphositeplusId { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public SmallMolecule? SmallMolecule { get; set; }
    }
}
