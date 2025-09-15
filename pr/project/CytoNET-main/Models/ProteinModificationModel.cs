using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CytoNET.Data.ProteinModification
{
    public class ProteinModificationDbContext : DbContext
    {
        public DbSet<ProteinModification> ProteinModifications { get; set; }
        public DbSet<PSiteAbProduct> PSiteAbProducts { get; set; }
        public DbSet<Modification> Modifications { get; set; }
        public DbSet<ProteinLevel> ProteinLevels { get; set; }

        public ProteinModificationDbContext(DbContextOptions<ProteinModificationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProteinModification>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity
                    .HasMany(e => e.Products)
                    .WithOne(e => e.ProteinModification)
                    .HasForeignKey(e => e.ProteinModificationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(e => e.Modification)
                    .WithMany(e => e.ProteinModifications)
                    .HasForeignKey(e => e.ModificationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(e => e.ProteinLevel)
                    .WithMany(e => e.ProteinModifications)
                    .HasForeignKey(e => e.ProteinLevelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PSiteAbProduct>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity
                    .HasOne(p => p.ProteinModification)
                    .WithMany(pm => pm.Products)
                    .HasForeignKey(p => p.ProteinModificationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Modification>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.Shape).HasMaxLength(50);
            });

            modelBuilder.Entity<ProteinLevel>(entity =>
            {
                entity.HasKey(e => e.ProteinLevelId);
            });
        }
    }

    public class ProteinModification
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string UniprotId { get; set; } = string.Empty;

        [Required]
        public string Site { get; set; } = string.Empty;

        [Required]
        public int NumReports { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public string ModificationDescription { get; set; } = string.Empty;

        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string ModificationId { get; set; } = string.Empty;

        [Required]
        public string ProteinLevelId { get; set; } = string.Empty;

        public List<PSiteAbProduct> Products { get; set; } = new List<PSiteAbProduct>();

        public Modification? Modification { get; set; }
        public ProteinLevel? ProteinLevel { get; set; }
    }

    public class PSiteAbProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ProductId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ProteinUniprotId { get; set; } = string.Empty;

        [Required]
        public string ProteinModificationId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Link { get; set; }
        public ProteinModification? ProteinModification { get; set; }
    }

    public class Modification
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string EffectCode { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Shape { get; set; } = string.Empty;

        public List<ProteinModification> ProteinModifications { get; set; } =
            new List<ProteinModification>();
    }

    public class ProteinLevel
    {
        [Key]
        public string ProteinLevelId { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public List<ProteinModification> ProteinModifications { get; set; } =
            new List<ProteinModification>();
    }
}
