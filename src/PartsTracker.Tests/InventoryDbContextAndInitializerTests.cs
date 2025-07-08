using Microsoft.EntityFrameworkCore;
using PartsTracker.Server.Data;
using PartsTracker.WebApi.Data;

namespace PartsTracker.Tests
{
    public class InventoryDbContextAndInitializerTests
    {
        private DbContextOptions<InventoryDbContext> CreateInMemoryOptions()
        {
            return new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task DbInitializer_SeedAsync_AddsSampleParts()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new InventoryDbContext(options);

            // Act
            await DbInitializer.SeedAsync(context);

            // Assert
            var parts = context.Parts.ToList();
            Assert.Equal(3, parts.Count);
            Assert.Contains(parts, p => p.PartNumber == "A1643200725");
            Assert.Contains(parts, p => p.PartNumber == "A2048200164");
            Assert.Contains(parts, p => p.PartNumber == "A2118201926");
        }

        [Fact]
        public async Task DbInitializer_SeedAsync_DoesNotDuplicateParts()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new InventoryDbContext(options);
            await DbInitializer.SeedAsync(context);

            // Act
            await DbInitializer.SeedAsync(context);

            // Assert
            var parts = context.Parts.ToList();
            Assert.Equal(3, parts.Count); // Should not add duplicates
        }
    }
}
