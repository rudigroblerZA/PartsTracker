using PartsTracker.Server.Data;
using PartsTracker.WebApi.Models;

namespace PartsTracker.WebApi.Data;

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
                    PartNumber = "A1643200725",
                    Description = "Control arm for A Class model.",
                    QuantityOnHand = 4,
                    LocationCode = "A1",
                    LastStockTake = DateTime.UtcNow,
                },
                new Part
                {
                    PartNumber = "A2048200164",
                    Description = "Headlight assembly for C Class model.",
                    QuantityOnHand = 5,
                    LocationCode = "B2",
                    LastStockTake = DateTime.UtcNow,
                },
                new Part
                {
                    PartNumber = "A2118201926",
                    Description = "an electronic control unit(ECU) for a E Class model.",
                    QuantityOnHand = 1,
                    LocationCode = "A1",
                    LastStockTake = DateTime.UtcNow,
                }
            });

            await context.SaveChangesAsync();
        }
    }
}
