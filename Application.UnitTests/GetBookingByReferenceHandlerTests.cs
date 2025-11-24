using Application.Common.Repositories;
using Application.Features.HotelManager.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests;

public class GetBookingByReferenceHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsBookingWithRoomInfo()
    {
        // arrange
        var room = new Room { Id = "r1", Number = "101", Capacity = 2, HotelId = "h1" };
        var booking = new Booking
        {
            Id = "b1",
            BookingReference = "ref1",
            GuestName = "G",
            RoomId = room.Id,
            Room = room,
            CheckIn = DateOnly.FromDateTime(DateTime.Today),
            CheckOut = DateOnly.FromDateTime(DateTime.Today).AddDays(1),
            NumberOfGuests = 1
        };

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new TestDbContext(options);
        // Persist entities so EF Core provides an IAsyncQueryProvider and Include works
        context.Set<Room>().Add(room);
        context.Set<Booking>().Add(booking);
        context.SaveChanges();

        var bookingRepo = new Mock<ICommandRepository<Booking>>();
        bookingRepo.Setup(r => r.GetQuery()).Returns(context.Set<Booking>().AsQueryable());

        var handler = new GetBookingByReferenceHandler(bookingRepo.Object);

        // act
        var result = await handler.Handle(new GetBookingByReferenceRequest { Reference = "ref1" }, CancellationToken.None);

        // assert
        result.Data.Should().NotBeNull();
        result.Data!.BookingReference.Should().Be("ref1");
        result.Data.Room.Should().NotBeNull();
        result.Data.Room!.Number.Should().Be("101");
    }
}