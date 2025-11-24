using Application.Features.DatabaseManager.Commands;
using Application.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests;

public class SeedAndMaintenanceHandlersTests
{
    [Fact]
    public async Task SeedDatabaseHandler_CallsSeeder_WhenSeedRequested()
    {
        var seederMock = new Mock<IDbSeeder>();
        seederMock.Setup(s => s.SeedDemoDataAsync()).Returns(Task.CompletedTask);

        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<SeedDatabaseHandler>>();
        var handler = new SeedDatabaseHandler(seederMock.Object, loggerMock.Object);
        var result = await handler.Handle(new SeedDatabaseRequest { SeedData = true }, CancellationToken.None);

        result.Success.Should().BeTrue();
        seederMock.Verify(s => s.SeedDemoDataAsync(), Times.Once);
    }

    [Fact]
    public async Task ClearDatabaseHandler_CallsMaintenance()
    {
        var maintenanceMock = new Mock<IDbMaintenance>();
        maintenanceMock.Setup(m => m.ClearDatabaseAsync()).Returns(Task.CompletedTask);

        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<ClearDatabaseHandler>>();
        var handler = new ClearDatabaseHandler(maintenanceMock.Object, loggerMock.Object);

        var result = await handler.Handle(new ClearDatabaseRequest(), CancellationToken.None);

        result.Success.Should().BeTrue();
        maintenanceMock.Verify(m => m.ClearDatabaseAsync(), Times.Once);
    }
}
