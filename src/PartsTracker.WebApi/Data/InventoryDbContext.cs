using Microsoft.EntityFrameworkCore;
using PartsTracker.WebApi.Models;

namespace PartsTracker.Server.Data;

public class InventoryDbContext : DbContext
{
    public DbSet<Part> Parts { get; set; }

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=InventoryDb;Username=partsuser;Password=supersecret");
    //    //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=InventoryDb;Username=partsuser;Password=supersecret");
    //    //optionsBuilder.UseInMemoryDatabase("PartsTrackerDB");
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.HasDefaultSchema("public");

        //foreach (var entity in modelBuilder.Model.GetEntityTypes())
        //{
        //    entity.SetTableName(entity.GetTableName()!.ToLower());
        //    foreach (var property in entity.GetProperties())
        //    {
        //        property.SetColumnName(property.Name.ToLower());
        //    }
        //}

        //modelBuilder.Entity<Part>(entity =>
        //{
        //    entity.Property(p => p.PartNumber).HasMaxLength(50).IsRequired();
        //    entity.Property(p => p.Description).HasMaxLength(200);
        //    entity.Property(p => p.QuantityOnHand).IsRequired().HasDefaultValue(0);
        //    entity.Property(p => p.LocationCode).HasMaxLength(50).IsRequired();
        //    entity.Property(p => p.LastStockTake).IsRequired();
        //});

        //modelBuilder.Entity<Part>().HasData(new[]
        //{
        //    new Part
        //    {
        //        PartNumber = "A1643200725",
        //        Description = "Control arm for A Class model.",
        //        QuantityOnHand = 4,
        //        LocationCode = "A1",
        //        LastStockTake = DateTime.UtcNow,
        //    },
        //    new Part
        //    {
        //        PartNumber = "A2048200164",
        //        Description = "Headlight assembly for C Class model.",
        //        QuantityOnHand = 5,
        //        LocationCode = "B2",
        //        LastStockTake = DateTime.UtcNow,
        //    },
        //    new Part
        //    {
        //        PartNumber = "A2118201926",
        //        Description = "an electronic control unit(ECU) for a E Class model.",
        //        QuantityOnHand = 1,
        //        LocationCode = "A1",
        //        LastStockTake = DateTime.UtcNow,
        //    }
        //});

        base.OnModelCreating(modelBuilder);
    }
}
