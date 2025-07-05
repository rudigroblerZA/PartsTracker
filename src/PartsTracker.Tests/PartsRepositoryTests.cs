using Microsoft.EntityFrameworkCore;
using PartsTracker.Server.Data;
using PartsTracker.WebApi.Infrastricture;
using PartsTracker.WebApi.Models;

namespace PartsTracker.Tests;

public class PartsRepositoryTests
{
    private static PartsRepository CreateRepository(out InventoryDbContext context)
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new InventoryDbContext(options);
        context.Database.EnsureCreated();

        return new PartsRepository(context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Part()
    {
        // Arrange
        var repository = CreateRepository(out var context);
        var part = new Part { PartNumber = "ABC123", Description = "Test part", QuantityOnHand = 5 };

        // Act
        await repository.AddAsync(part);
        await repository.SaveChangesAsync();

        // Assert
        var saved = await repository.GetByIdAsync("ABC123");
        Assert.NotNull(saved);
        Assert.Equal("Test part", saved?.Description);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Inserted_Parts()
    {
        // Arrange
        var repository = CreateRepository(out var context);
        var parts = new List<Part>
        {
            new Part { PartNumber = "A1", QuantityOnHand = 1 },
            new Part { PartNumber = "B2", QuantityOnHand = 2 }
        };
        await repository.AddRangeAsync(parts);
        await repository.SaveChangesAsync();

        // Act
        var all = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task Remove_Should_Delete_Part()
    {
        // Arrange
        var repository = CreateRepository(out var context);
        var part = new Part { PartNumber = "X99", QuantityOnHand = 10 };
        await repository.AddAsync(part);
        await repository.SaveChangesAsync();

        // Act
        var toRemove = await repository.GetByIdAsync("X99");
        Assert.NotNull(toRemove);

        repository.Remove(toRemove!);
        await repository.SaveChangesAsync();

        // Assert
        var deleted = await repository.GetByIdAsync("X99");
        Assert.Null(deleted);
    }

    [Fact]
    public async Task FindAsync_Should_Filter_Correctly()
    {
        // Arrange
        var repository = CreateRepository(out var context);
        await repository.AddRangeAsync(new[]
        {
            new Part { PartNumber = "P1", QuantityOnHand = 10 },
            new Part { PartNumber = "P2", QuantityOnHand = 0 }
        });
        await repository.SaveChangesAsync();

        // Act
        var available = await repository.FindAsync(p => p.QuantityOnHand > 0);

        // Assert
        Assert.Single(available);
        Assert.Equal("P1", available.First().PartNumber);
    }
}
