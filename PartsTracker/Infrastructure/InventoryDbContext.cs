using Microsoft.EntityFrameworkCore;
using PartsTracker.Models;

namespace PartsTracker.Infrastructure;

/// <summary>
/// The Entity Framework Core database context for the inventory system.
/// </summary>
public class InventoryDbContext : DbContext
{
    /// <summary>
    /// Constructor for the InventoryDbContext.
    /// </summary>
    /// <param name="options"></param>
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Parts table.
    /// </summary>
    public DbSet<Part> Parts { get; set; } = null!;

    /// <summary>
    /// Configures the model for the database context.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()!.ToLower());
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToLower());
            }
        }

        //modelBuilder.Entity<Part>(entity =>
        //{
        //    entity.Property(p => p.PartNumber).HasMaxLength(50).IsRequired();
        //    entity.Property(p => p.Description).HasMaxLength(200);
        //    entity.Property(p => p.LocationCode).HasMaxLength(50);
        //    entity.Property(p => p.CreatedBy).HasMaxLength(100);
        //    entity.Property(p => p.ModifiedBy).HasMaxLength(100);
        //});

        base.OnModelCreating(modelBuilder);
    }
}
