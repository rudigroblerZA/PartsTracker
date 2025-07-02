using PartsTracker.Models;

namespace PartsTracker.Infrastructure;

/// <summary>
/// Initializes the database with sample data.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Seed at least three sample parts. 
    /// </summary>
    public static async Task SeedAsync(InventoryDbContext context)
    {
        if (!context.Parts.Any())
        {
            context.Parts.AddRange(new[]
            {
            new Part
            {
                PartNumber = "P001",
                Description = "Widget A",
                QuantityOnHand = 100,
                LocationCode = "A1",
                LastStockTake = DateTime.UtcNow,
            },
            new Part
            {
                PartNumber = "P002",
                Description = "Widget B",
                QuantityOnHand = 50,
                LocationCode = "B2",
                LastStockTake = DateTime.UtcNow,
            },
            new Part
            {
                PartNumber = "P003",
                Description = "Widget C",
                QuantityOnHand = 75,
                LocationCode = "C3",
                LastStockTake = DateTime.UtcNow,
            }
        });

            await context.SaveChangesAsync();
        }
    }
}
