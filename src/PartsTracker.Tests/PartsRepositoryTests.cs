using Microsoft.EntityFrameworkCore;
using Moq;
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

    [Fact]
    public async Task AddAsync_Should_Throw_When_QuantityOnHand_Negative()
    {
        var repository = CreateRepository(out var context);
        var part = new Part { PartNumber = "NEG1", QuantityOnHand = -1 };
        await Assert.ThrowsAsync<ArgumentException>(() => repository.AddAsync(part));
    }

    [Fact]
    public async Task Update_Should_Throw_When_QuantityOnHand_Negative()
    {
        var repository = CreateRepository(out var context);
        var part = new Part { PartNumber = "UPDNEG", QuantityOnHand = 1 };
        await repository.AddAsync(part);
        await repository.SaveChangesAsync();
        part.QuantityOnHand = -5;
        Assert.Throws<ArgumentException>(() => repository.Update(part));
    }

    [Fact]
    public void RemoveRange_RemovesPartsFromContext()
    {
        // Arrange
        var parts = new List<Part>
            {
                new Part { PartNumber = "P1", Description = "Part1", QuantityOnHand = 10 },
                new Part { PartNumber = "P2", Description = "Part2", QuantityOnHand = 5 }
            };

        var mockSet = new Mock<DbSet<Part>>();
        var mockContext = new Mock<InventoryDbContext>();
        mockContext.Setup(m => m.Parts).Returns(mockSet.Object);

        var repository = new PartsRepository(mockContext.Object);

        // Act
        repository.RemoveRange(parts);

        // Assert
        mockSet.Verify(m => m.RemoveRange(parts), Times.Once);
    }

    [Fact]
    public void Query_WithTracking_ReturnsTrackedQueryable()
    {
        // Arrange
        var mockSet = new Mock<DbSet<Part>>();
        var mockContext = new Mock<InventoryDbContext>();
        mockContext.Setup(m => m.Parts).Returns(mockSet.Object);

        var repository = new PartsRepository(mockContext.Object);

        // Act
        var result = repository.Query(tracking: true);

        // Assert
        Assert.Same(mockSet.Object, result);
    }

    [Fact]
    public void Query_WithoutTracking_ReturnsNoTrackingQueryable()
    {
        // Arrange
        var mockSet = new Mock<DbSet<Part>>();
        var mockContext = new Mock<InventoryDbContext>();
        mockContext.Setup(m => m.Parts).Returns(mockSet.Object);

        // Setup AsNoTracking to return a different IQueryable for test
        var noTrackingQueryable = new List<Part>().AsQueryable();
        mockSet.As<IQueryable<Part>>().Setup(m => m.Provider).Returns(noTrackingQueryable.Provider);
        mockSet.As<IQueryable<Part>>().Setup(m => m.Expression).Returns(noTrackingQueryable.Expression);
        mockSet.As<IQueryable<Part>>().Setup(m => m.ElementType).Returns(noTrackingQueryable.ElementType);
        mockSet.As<IQueryable<Part>>().Setup(m => m.GetEnumerator()).Returns(noTrackingQueryable.GetEnumerator());
        mockSet.Setup(m => m.AsNoTracking()).Returns(noTrackingQueryable);

        var repository = new PartsRepository(mockContext.Object);

        // Act
        var result = repository.Query(tracking: false);

        // Assert
        Assert.Same(noTrackingQueryable, result);
    }
}
