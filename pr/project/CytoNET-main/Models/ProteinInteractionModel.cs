using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Cytonet.Data.ProteinInteraction
{
    public class ProteinInteractionDbContext : DbContext
    {
        public DbSet<ProteinLevel> ProteinLevels { get; set; }
        public DbSet<Protein> Proteins { get; set; }
        public DbSet<ProteinInteraction> ProteinInteractions { get; set; }

        public ProteinInteractionDbContext(DbContextOptions<ProteinInteractionDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ProteinLevel configuration
            modelBuilder.Entity<ProteinLevel>(entity =>
            {
                entity.ToTable("protein_level_table");
                entity.HasKey(e => e.ProteinLevelId);
                entity.Property(e => e.ProteinLevelId).HasColumnName("protein_level_id");
                entity.Property(e => e.Category).HasColumnName("category");
            });

            // Protein configuration
            modelBuilder.Entity<Protein>(entity =>
            {
                entity.ToTable("protein_table");
                entity.HasKey(e => e.UniprotIdCasNumber);
                entity.Property(e => e.UniprotIdCasNumber).HasColumnName("uniprot_id/cas_number");
                entity.Property(e => e.ShortName).HasColumnName("short_name");
                entity.Property(e => e.ProteinType).HasColumnName("protein_type");
                entity.Property(e => e.ProteinLevelId).HasColumnName("protein_level_id");

                entity
                    .HasOne(e => e.ProteinLevel)
                    .WithMany(e => e.Proteins)
                    .HasForeignKey(e => e.ProteinLevelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ProteinInteraction configuration
            modelBuilder.Entity<ProteinInteraction>(entity =>
            {
                entity.ToTable("protein_interaction_table");
                entity.HasKey(e => e.ProteinInteractionId);
                entity
                    .Property(e => e.ProteinInteractionId)
                    .HasColumnName("protein_interaction_id");
                entity.Property(e => e.InitiatingProteinId).HasColumnName("initiating_protein_id");
                entity
                    .Property(e => e.AssociatingProteinId)
                    .HasColumnName("associating_protein_id");
                entity
                    .Property(e => e.InitiatingProteinIcon)
                    .HasColumnName("initiating_protein_icon");
                entity
                    .Property(e => e.AssociatingProteinIcon)
                    .HasColumnName("associating_protein_icon");
                entity.Property(e => e.EffectOfInteraction).HasColumnName("effect_of_interaction");
                entity.Property(e => e.InteractionEdge).HasColumnName("interaction_edge");

                entity
                    .HasOne(e => e.InitiatingProtein)
                    .WithMany()
                    .HasForeignKey(e => e.InitiatingProteinId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(e => e.AssociatingProtein)
                    .WithMany()
                    .HasForeignKey(e => e.AssociatingProteinId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public void DisableForeignKeyConstraints()
        {
            var databaseType = Database.ProviderName;

            if (databaseType.Contains("SqlServer"))
            {
                Database.ExecuteSqlRaw(
                    "EXEC sp_MSforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT ALL\""
                );
            }
            else if (databaseType.Contains("Sqlite"))
            {
                Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF");
            }
            else if (databaseType.Contains("MySql") || databaseType.Contains("MariaDb"))
            {
                Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS=0");
            }
            else if (databaseType.Contains("Npgsql"))
            {
                Database.ExecuteSqlRaw("SET CONSTRAINTS ALL DEFERRED");
            }
        }

        public void EnableForeignKeyConstraints()
        {
            var databaseType = Database.ProviderName;

            if (databaseType.Contains("SqlServer"))
            {
                Database.ExecuteSqlRaw(
                    "EXEC sp_MSforeachtable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL\""
                );
            }
            else if (databaseType.Contains("Sqlite"))
            {
                Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON");
            }
            else if (databaseType.Contains("MySql") || databaseType.Contains("MariaDb"))
            {
                Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS=1");
            }
        }
    }

    public class ProteinLevel
    {
        [Key]
        public string ProteinLevelId { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public ICollection<Protein>? Proteins { get; set; }
    }

    public class Protein
    {
        [Key]
        public string UniprotIdCasNumber { get; set; } = string.Empty;

        [Required]
        public string ShortName { get; set; } = string.Empty;

        [Required]
        public string ProteinType { get; set; } = string.Empty;

        [Required]
        public string ProteinLevelId { get; set; } = string.Empty;

        public ProteinLevel? ProteinLevel { get; set; }
    }

    public class ProteinInteraction
    {
        [Key]
        public string ProteinInteractionId { get; set; } = string.Empty;

        [Required]
        public string InitiatingProteinId { get; set; } = string.Empty;

        [Required]
        public string AssociatingProteinId { get; set; } = string.Empty;

        [Required]
        public string InitiatingProteinIcon { get; set; } = string.Empty;

        [Required]
        public string AssociatingProteinIcon { get; set; } = string.Empty;

        [Required]
        public string EffectOfInteraction { get; set; } = string.Empty;

        [Required]
        public string InteractionEdge { get; set; } = string.Empty;

        public Protein? InitiatingProtein { get; set; }
        public Protein? AssociatingProtein { get; set; }
    }
}
