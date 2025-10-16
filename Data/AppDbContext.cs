using Microsoft.EntityFrameworkCore;
using PorquinhoApi.Models;

namespace PorquinhoApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    // public DbSet<Wallet> Wallets => Set<wallet>();
}