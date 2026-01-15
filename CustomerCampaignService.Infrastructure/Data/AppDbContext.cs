using CustomerCampaignService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerCampaignService.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

    
        public DbSet<Campaign> Campaigns => Set<Campaign>();
        public DbSet<Agent> Agents => Set<Agent>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<RewardEntry> RewardEntries => Set<RewardEntry>();
        public DbSet<PurchaseImport> PurchaseImports => Set<PurchaseImport>();
        public DbSet<PurchaseTransaction> PurchaseTransactions => Set<PurchaseTransaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.HasKey(x => x.CampaignId);
                entity.Property(x => x.CampaignCode).IsRequired();
                entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
                entity.Property(x => x.StartDate).HasColumnType("date");
                entity.Property(x => x.EndDate).HasColumnType("date");
                entity.Property(x => x.CreatedAt).HasColumnType("datetime2");

                entity.HasIndex(x => x.CampaignCode).IsUnique();
            });

            modelBuilder.Entity<Agent>(entity =>
            {
                entity.HasKey(x => x.AgentId);
                entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
                entity.Property(x => x.CreatedAt).HasColumnType("datetime2");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(x => x.CustomerId);
                entity.Property(x => x.ExternalCustomerRef).HasMaxLength(100).IsRequired();
                entity.HasIndex(x => x.ExternalCustomerRef).IsUnique();
                entity.Property(x => x.FirstName).HasMaxLength(100);
                entity.Property(x => x.LastName).HasMaxLength(100);
                entity.Property(x => x.CreatedAt).HasColumnType("datetime2");
            });

            modelBuilder.Entity<RewardEntry>(entity =>
            {
                entity.HasKey(x => x.RewardEntryId);
                entity.Property(x => x.RewardCode).ValueGeneratedOnAdd();
                entity.Property(x => x.RewardDate).HasColumnType("date");
                entity.Property(x => x.Notes).HasMaxLength(1000);
                entity.Property(x => x.CreatedAt).HasColumnType("datetime2");

                entity.HasOne(x => x.Campaign)
                    .WithMany(x => x.RewardEntries)
                    .HasForeignKey(x => x.CampaignId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Agent)
                    .WithMany(x => x.RewardEntries)
                    .HasForeignKey(x => x.AgentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Customer)
                    .WithMany(x => x.RewardEntries)
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.CorrectionOfRewardEntry)
                    .WithMany(x => x.Corrections)
                    .HasForeignKey(x => x.CorrectionOfRewardEntryId)
                    .OnDelete(DeleteBehavior.Restrict).IsRequired(false);

                entity.HasIndex(x => x.RewardCode).IsUnique();
                entity.HasIndex(x => new { x.CampaignId, x.AgentId, x.RewardDate, x.Status });
            });

            modelBuilder.Entity<PurchaseImport>(entity =>
            {
                entity.HasKey(x => x.ImportBatchId);
                entity.Property(x => x.FileName).HasMaxLength(260).IsRequired();
                entity.Property(x => x.FileHash).HasMaxLength(128).IsRequired();
                entity.Property(x => x.ErrorMessage).HasMaxLength(2000);
                entity.Property(x => x.ImportedAt).HasColumnType("datetime2");

                entity.HasOne(x => x.Campaign)
                    .WithMany(x => x.PurchaseImportBatches)
                    .HasForeignKey(x => x.CampaignId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchaseTransaction>(entity =>
            {
                entity.HasKey(x => x.PurchaseTransactionId);
                entity.Property(x => x.TransactionId).HasMaxLength(100).IsRequired();
                entity.Property(x => x.PurchaseDate).HasColumnType("datetime2");
                entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                entity.Property(x => x.CreatedAt).HasColumnType("datetime2");

                entity.HasOne(x => x.ImportBatch)
                    .WithMany()
                    .HasForeignKey(x => x.ImportBatchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Campaign)
                    .WithMany()
                    .HasForeignKey(x => x.CampaignId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Customer)
                    .WithMany()
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.CampaignId, x.CustomerId });
                entity.HasIndex(x => x.TransactionId);
            });

        }

    }
}
