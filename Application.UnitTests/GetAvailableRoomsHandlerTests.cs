using Application.Common.Repositories;
using Application.Features.HotelManager.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests;

public class GetAvailableRoomsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAvailableRooms_ExcludesBookedRooms()
    {
        // arrange
        var room1 = new Room { Id = "r1", Number = "101", Capacity = 2, HotelId = "h1", Bookings = new List<Booking>() };
        var room2 = new Room
        {
            Id = "r2",
            Number = "102",
            Capacity = 2,
            HotelId = "h1",
            Bookings = new List<Booking>
            {
                new Booking
                {
                    Id = "b1",
                    RoomId = "r2",
                    CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(1),
                    CheckOut = DateOnly.FromDateTime(DateTime.Today).AddDays(3),
                    BookingReference = "ref-b1",   // required by model
                    GuestName = "Test Guest"       // required by model
                }
            }
        };

        // Use EF Core in-memory DbContext so async EF operators used by handlers work in tests
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new TestDbContext(options);
        context.Set<Room>().AddRange(room1, room2);
        context.SaveChanges();

        var roomRepo = new Mock<ICommandRepository<Room>>();
        roomRepo.Setup(r => r.GetQuery()).Returns(context.Set<Room>().AsQueryable());

        var handler = new GetAvailableRoomsHandler(roomRepo.Object);

        var request = new GetAvailableRoomsRequest
        {
            HotelId = "h1",
            CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(1),
            CheckOut = DateOnly.FromDateTime(DateTime.Today).AddDays(2),
            NumberOfPeople = 2
        };

        // act
        var result = await handler.Handle(request, CancellationToken.None);

        // assert
        result.Data.Should().ContainSingle(r => r.Id == "r1");
        result.Data.Should().NotContain(r => r.Id == "r2");
    }

    [Fact]
    public async Task Handle_ThrowsWhenInvalidRequest()
    {
        var roomRepo = new Mock<ICommandRepository<Room>>();
        var handler = new GetAvailableRoomsHandler(roomRepo.Object);

        var request = new GetAvailableRoomsRequest
        {
            HotelId = "",
            CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(3),
            CheckOut = DateOnly.FromDateTime(DateTime.Today).AddDays(2),
            NumberOfPeople = 0
        };

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(request, CancellationToken.None));
    }
}