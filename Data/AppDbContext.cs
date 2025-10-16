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
}