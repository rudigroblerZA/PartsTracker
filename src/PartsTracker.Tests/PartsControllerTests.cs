using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PartsTracker.WebApi.Controllers;
using PartsTracker.WebApi.Infrastricture;
using PartsTracker.WebApi.Models;

namespace PartsTracker.Tests;

public class PartsControllerTests
{
    private static PartsController CreateController(Mock<IPartsRepository> repoMock)
    {
        repoMock ??= new Mock<IPartsRepository>();
        var loggerMock = new Mock<ILogger<PartsController>>();
        return new PartsController(repoMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithParts()
    {
        // Arrange
        var repoMock = new Mock<IPartsRepository>();
        repoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(new List<Part> { new Part { PartNumber = "P1" } });
        var controller = CreateController(repoMock);

        // Act
        var result = await controller.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var parts = Assert.IsAssignableFrom<IEnumerable<Part>>(ok.Value);
        Assert.Single(parts);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenMissing()
    {
        var repoMock = new Mock<IPartsRepository>();
        repoMock.Setup(r => r.GetByIdAsync("X")).ReturnsAsync(null as Part);
        var controller = CreateController(repoMock);
        var result = await controller.Get("X");
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Get_ReturnsOk_WhenFound()
    {
        var repoMock = new Mock<IPartsRepository>();
        repoMock.Setup(r => r.GetByIdAsync("P2")).ReturnsAsync(new Part { PartNumber = "P2" });
        var controller = CreateController(repoMock);
        var result = await controller.Get("P2");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var part = Assert.IsType<Part>(ok.Value);
        Assert.Equal("P2", part.PartNumber);
    }

    [Fact]
    public async Task Create_ReturnsConflict_IfExists()
    {
        var repoMock = new Mock<IPartsRepository>();
        repoMock.Setup(r => r.GetByIdAsync("P3")).ReturnsAsync(new Part { PartNumber = "P3" });
        var controller = CreateController(repoMock);
        var result = await controller.Create(new Part { PartNumber = "P3" });
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenSuccessful()
    {
        var repoMock = new Mock<IPartsRepository>();
        var part = new Part { PartNumber = "P4", QuantityOnHand = 10 };
        repoMock.Setup(r => r.GetByIdAsync("P4")).ReturnsAsync(part);
        var controller = CreateController(repoMock);
        var result = await controller.Update("P4", part);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMissing()
    {
        var repoMock = new Mock<IPartsRepository>();
        repoMock.Setup(r => r.GetByIdAsync("P5")).ReturnsAsync((Part?)null);
        var controller = CreateController(repoMock);
        var result = await controller.Update("P5", new Part { PartNumber = "P5" });
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_ForNegativeQuantity()
    {
        var repoMock = new Mock<IPartsRepository>();
        var part = new Part { PartNumber = "P9", QuantityOnHand = -5 };
        repoMock.Setup(r => r.GetByIdAsync("P9")).ReturnsAsync(part);
        var controller = CreateController(repoMock);
        var result = await controller.Update("P9", part);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        var repoMock = new Mock<IPartsRepository>();
        var part = new Part { PartNumber = "P6" };
        repoMock.Setup(r => r.GetByIdAsync("P6")).ReturnsAsync(part);
        var controller = CreateController(repoMock);
        var result = await controller.Delete("P6");
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        var repoMock = new Mock<IPartsRepository>();
        repoMock.Setup(r => r.GetByIdAsync("P7")).ReturnsAsync((Part?)null);
        var controller = CreateController(repoMock);
        var result = await controller.Delete("P7");
        Assert.IsType<NotFoundResult>(result);
    }
}
