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
        var part = new Part { PartNumber = "ABC123", Name = "Test", Description = "Test part", QuantityOnHand = 5 };

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
            new Part { PartNumber = "A1", Name = "A1Name", QuantityOnHand = 1 },
            new Part { PartNumber = "B2", Name = "B2Name", QuantityOnHand = 2 }
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
        var part = new Part { PartNumber = "X99", Name = "ToRemove", QuantityOnHand = 10 };
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
            new Part { PartNumber = "P1", Name = "P1Name", QuantityOnHand = 10 },
            new Part { PartNumber = "P2", Name = "P2Name", QuantityOnHand = 0 }
        });
        await repository.SaveChangesAsync();

        // Act
        var available = await repository.FindAsync(p => p.QuantityOnHand > 0);

        // Assert
        Assert.Single(available);
        Assert.Equal("P1", available.First().PartNumber);
    }

    [Fact]
    public async Task AddAsync_Should_Throw_When_QuantityOnHand_Negative()
    {
        var repository = CreateRepository(out var context);
        var part = new Part { PartNumber = "NEG1", Name = "Negative", QuantityOnHand = -1 };
        await Assert.ThrowsAsync<ArgumentException>(() => repository.AddAsync(part));
    }

    [Fact]
    public async Task AddAsync_Should_Throw_When_Name_Missing()
    {
        var repository = CreateRepository(out var context);
        var part = new Part { PartNumber = "NONAME", Name = "", QuantityOnHand = 1 };
        await Assert.ThrowsAsync<ArgumentException>(() => repository.AddAsync(part));
    }

    [Fact]
    public async Task Update_Should_Throw_When_QuantityOnHand_Negative()
    {
        var repository = CreateRepository(out var context);
        var part = new Part { PartNumber = "UPDNEG", Name = "UpdateNeg", QuantityOnHand = 1 };
        await repository.AddAsync(part);
        await repository.SaveChangesAsync();
        part.QuantityOnHand = -5;
        Assert.Throws<ArgumentException>(() => repository.Update(part));
    }

    [Fact]
    public async Task AddAsync_Should_Succeed_With_Valid_Data()
    {
        var repository = CreateRepository(out var context);
        var part = new Part { PartNumber = "OK1", Name = "Valid", QuantityOnHand = 0 };
        await repository.AddAsync(part);
        await repository.SaveChangesAsync();
        var saved = await repository.GetByIdAsync("OK1");
        Assert.NotNull(saved);
        Assert.Equal("Valid", saved?.Name);
    }
}
