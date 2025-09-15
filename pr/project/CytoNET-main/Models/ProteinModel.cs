using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CytoNET.Data.Protein
{
    public class ProteinDbContext : DbContext
    {
        public DbSet<Protein> Proteins { get; set; }
        public DbSet<Alias> Aliases { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProteinLevel> ProteinLevels { get; set; }

        public ProteinDbContext(DbContextOptions<ProteinDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Protein>(entity =>
            {
                entity.HasKey(e => e.UniprotId);

                entity
                    .HasOne(e => e.ProteinLevel)
                    .WithMany(e => e.Proteins)
                    .HasForeignKey(e => e.ProteinLevelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Aliases);

                entity
                    .HasMany(e => e.Products)
                    .WithOne(e => e.Protein)
                    .HasForeignKey(e => e.ProteinUniprotId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Alias>(entity =>
            {
                entity.HasKey(e => e.AliasId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
            });

            modelBuilder.Entity<ProteinLevel>(entity =>
            {
                entity.HasKey(e => e.ProteinLevelId);
            });
        }
    }

    public class Protein
    {
        [Key]
        public string UniprotId { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string ShortName { get; set; } = string.Empty;

        [Required]
        public string LongName { get; set; } = string.Empty;

        [Required]
        public string Processing { get; set; } = string.Empty;

        [Required]
        public string PrecursorProteinMass { get; set; } = string.Empty;

        [Required]
        public string FunctionDescription { get; set; } = string.Empty;

        [Required]
        public string CstPhosphositePlusEntryId { get; set; } = string.Empty;

        [Required]
        public string ProteinLevelId { get; set; } = string.Empty;

        [Required]
        public string IconType { get; set; } = string.Empty;

        [Required]
        public string Aliases { get; set; } = string.Empty;

        public string StringDbLink { get; set; } = string.Empty;
        public string KinaseNetLink { get; set; } = string.Empty;
        public string UniprotLink { get; set; } = string.Empty;
        public string PhosphoNetLink { get; set; } = string.Empty;
        public string TranscriptoNetLink { get; set; } = string.Empty;
        public string OncoNetLink { get; set; } = string.Empty;
        public string DrugProNetLink { get; set; } = string.Empty;
        public string PhosphoSitePlusLink { get; set; } = string.Empty;

        public ProteinLevel? ProteinLevel { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }

    public class Alias
    {
        [Key]
        public string AliasId { get; set; } = string.Empty;

        [Required]
        public string ProteinUniprotId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public Protein? Protein { get; set; }
    }

    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        public string ProteinUniprotId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Link { get; set; }

        public Protein? Protein { get; set; }
    }

    public class ProteinLevel
    {
        [Key]
        public string ProteinLevelId { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public List<Protein> Proteins { get; set; } = new List<Protein>();
    }
}
