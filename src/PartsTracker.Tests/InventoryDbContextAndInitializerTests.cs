using Microsoft.EntityFrameworkCore;
using PartsTracker.Server.Data;
using PartsTracker.WebApi.Data;

namespace PartsTracker.Tests;

public class InventoryDbContextAndInitializerTests
{
    [Fact]
    public void Can_Create_InventoryDbContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDbContext")
            .Options;
        using var context = new InventoryDbContext(options);
        Assert.NotNull(context.Parts);
    }

    [Fact]
    public async Task DbInitializer_Seeds_Data()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDbInitializer")
            .Options;
        using var context = new InventoryDbContext(options);
        await DbInitializer.SeedAsync(context);
        var parts = await context.Parts.ToListAsync();
        Assert.True(parts.Count >= 3);
        Assert.Contains(parts, p => p.PartNumber == "A1643200725");
        Assert.Contains(parts, p => p.PartNumber == "A2048200164");
        Assert.Contains(parts, p => p.PartNumber == "A2118201926");
    }
}
