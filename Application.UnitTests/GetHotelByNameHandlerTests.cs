using Application.Common.Repositories;
using Application.Features.HotelManager.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests;

public class GetHotelByNameHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsHotel_WhenNameMatches()
    {
        // arrange
        var hotel = new Hotel
        {
            Id = "h1",
            Name = "Premier Inn",
            Rooms = new List<Room>
            {
                new Room { Id = "r1", Number = "101", Capacity = 2, HotelId = "h1" }
            }
        };

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new TestDbContext(options);
        context.Set<Hotel>().Add(hotel);
        context.SaveChanges();

        var hotelRepo = new Mock<ICommandRepository<Hotel>>();
        hotelRepo.Setup(r => r.GetQuery()).Returns(context.Set<Hotel>().AsQueryable());

        var handler = new GetHotelByNameHandler(hotelRepo.Object);

        var request = new GetHotelByNameRequest { Name = "Premier Inn" };

        // act
        var result = await handler.Handle(request, CancellationToken.None);

        // assert
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
        result.Data![0].Name.Should().Be("Premier Inn");
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenNotFound()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new TestDbContext(options);
        // no hotels added

        var hotelRepo = new Mock<ICommandRepository<Hotel>>();
        hotelRepo.Setup(r => r.GetQuery()).Returns(context.Set<Hotel>().AsQueryable());

        var handler = new GetHotelByNameHandler(hotelRepo.Object);
        var result = await handler.Handle(new GetHotelByNameRequest { Name = "Nope" }, CancellationToken.None);

        result.Data.Should().BeNull();
    }
}