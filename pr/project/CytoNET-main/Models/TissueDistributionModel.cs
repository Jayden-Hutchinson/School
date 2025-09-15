using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CytoNET.Data.ProteinModification;
using Microsoft.EntityFrameworkCore;

namespace CytoNET.Data.TissueDistribution
{
    public class TissueDistributionDbContext : DbContext
    {
        public DbSet<TissueDistribution> TissueDistributions { get; set; }
        public DbSet<TissueOrgan> TissueOrgans { get; set; }
        public DbSet<TissueDistributionTissueOrgan> TissueDistributionTissueOrgans { get; set; }
        public DbSet<ProteinLevel> ProteinLevels { get; set; }

        public TissueDistributionDbContext(DbContextOptions<TissueDistributionDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TissueDistribution>(entity =>
            {
                entity.HasKey(e => e.UniprotId);

                entity
                    .HasOne(e => e.ProteinLevel)
                    .WithMany(p => p.TissueDistributions)
                    .HasForeignKey(e => e.ProteinLevelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TissueOrgan>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<TissueDistributionTissueOrgan>(entity =>
            {
                entity.HasKey(e => new { e.TissueDistributionId, e.TissueOrganId });

                entity
                    .HasOne(e => e.TissueDistribution)
                    .WithMany(e => e.TissueOrgans)
                    .HasForeignKey(e => e.TissueDistributionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(e => e.TissueOrgan)
                    .WithMany(e => e.TissueDistributions)
                    .HasForeignKey(e => e.TissueOrganId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProteinLevel>(entity =>
            {
                entity.HasKey(e => e.ProteinLevelId);
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

    public class TissueDistribution
    {
        [Key]
        public string UniprotId { get; set; } = string.Empty;

        [Required]
        public string CasNumber { get; set; } = string.Empty;

        [Required]
        public string ProteinLevelId { get; set; } = string.Empty;

        public ProteinLevel? ProteinLevel { get; set; }
        public List<TissueDistributionTissueOrgan> TissueOrgans { get; set; } =
            new List<TissueDistributionTissueOrgan>();
    }

    public class TissueOrgan
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public bool IsBold { get; set; }

        public List<TissueDistributionTissueOrgan> TissueDistributions { get; set; } =
            new List<TissueDistributionTissueOrgan>();
    }

    public class TissueDistributionTissueOrgan
    {
        [Required]
        public string TissueDistributionId { get; set; } = string.Empty;

        [Required]
        public string TissueOrganId { get; set; } = string.Empty;

        public TissueDistribution? TissueDistribution { get; set; }
        public TissueOrgan? TissueOrgan { get; set; }
    }

    public class ProteinLevel
    {
        [Key]
        public string ProteinLevelId { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public List<TissueDistribution> TissueDistributions { get; set; } = new();
    }
}
