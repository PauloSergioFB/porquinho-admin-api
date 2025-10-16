using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Models;

namespace PorquinhoApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Functionality> Functionalities => Set<Functionality>();
    public DbSet<SubscriptionTier> SubscriptionTiers => Set<SubscriptionTier>();
    public DbSet<SubscriptionStatus> SubscriptionStatuses => Set<SubscriptionStatus>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SubscriptionTier>()
            .HasMany(tier => tier.Functionalities)
            .WithMany(func => func.SubscriptionTiers)
            .UsingEntity<Dictionary<string, object>>(
                "P_CATEGORY_TIER_FUNCTIONALITY",
                j => j
                    .HasOne<Functionality>()
                    .WithMany()
                    .HasForeignKey("FUNCTIONALITY_ID")
                    .HasConstraintName("FK_CTF_FUNC")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<SubscriptionTier>()
                    .WithMany()
                    .HasForeignKey("SUBSCRIPTION_TIER_ID")
                    .HasConstraintName("FK_CTF_TIER")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("SUBSCRIPTION_TIER_ID", "FUNCTIONALITY_ID")
                        .HasName("PK_CAT_TIER_FUNC");

                    j.ToTable("P_CATEGORY_TIER_FUNCTIONALITY");
                }
            );
    }
}