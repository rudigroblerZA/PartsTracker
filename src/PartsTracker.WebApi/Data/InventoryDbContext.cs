using Microsoft.EntityFrameworkCore;
using PartsTracker.WebApi.Models;

namespace PartsTracker.Server.Data;

/// <summary>
/// DbContext for managing inventory parts.
/// </summary>
public class InventoryDbContext : DbContext
{
    /// <summary>
    /// Parts in the inventory.
    /// </summary>
    public DbSet<Part> Parts { get; set; }

    /// <summary>
    /// Create new instance of <see cref="InventoryDbContext"/> with specified options.
    /// </summary>
    /// <param name="options"></param>
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configure the model for the DbContext.
    /// </summary>
    /// <param name="modelBuilder">EF Model Builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Part>()
            .Property(p => p.xmin)
            .IsRowVersion()
            .HasColumnName("xmin");
    }
}
