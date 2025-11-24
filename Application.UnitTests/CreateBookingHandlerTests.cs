using Application.Common.Repositories;
using Application.Features.BookingManager.Commands;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests;

public class CreateBookingHandlerTests
{
    [Fact]
    public async Task Handle_WhenRoomAvailableAndCapacity_ShouldCreateAndSaveBooking()
    {
        // arrange
        var existingBookings = new List<Booking>(); // no bookings

        var bookingsOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var bookingsContext = new TestDbContext(bookingsOptions);
        bookingsContext.Set<Booking>().AddRange(existingBookings);
        bookingsContext.SaveChanges();

        var bookingRepo = new Mock<ICommandRepository<Booking>>();
        // Return EF Core-backed IQueryable so EF async extensions work
        bookingRepo.Setup(r => r.GetQuery()).Returns(bookingsContext.Set<Booking>().AsQueryable());
        bookingRepo.Setup(r => r.CreateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

        var room = new Room { Id = "room1", Capacity = 2, HotelId = "hotel1", Number = "101" };

        var roomsOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var roomsContext = new TestDbContext(roomsOptions);
        roomsContext.Set<Room>().Add(room);
        roomsContext.SaveChanges();

        var roomRepo = new Mock<ICommandRepository<Room>>();
        roomRepo.Setup(r => r.GetQuery()).Returns(roomsContext.Set<Room>().AsQueryable());

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new CreateBookingHandler(bookingRepo.Object, roomRepo.Object, unitOfWork.Object);

        var request = new CreateBookingRequest
        {
            RoomId = "room1",
            GuestName = "Alice",
            CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(1),
            CheckOut = DateOnly.FromDateTime(DateTime.Today).AddDays(3),
            NumberOfGuests = 2
        };

        // act
        var result = await handler.Handle(request, CancellationToken.None);

        // assert
        result.Data.Should().NotBeNull();
        bookingRepo.Verify(r => r.CreateAsync(It.Is<Booking>(b => b.RoomId == "room1" && b.GuestName == "Alice"), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOverlapExists_ShouldThrowInvalidOperationException()
    {
        // arrange
        var existingBooking = new Booking
        {
            Id = "b1",
            RoomId = "room1",
            CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(2),
            CheckOut = DateOnly.FromDateTime(DateTime.Today).AddDays(5)
        };
        var bookings = new List<Booking> { existingBooking };
        var bookingRepo = new Mock<ICommandRepository<Booking>>();
        bookingRepo.Setup(r => r.GetQuery()).Returns(bookings.AsQueryable());

        var room = new Room { Id = "room1", Capacity = 2, HotelId = "hotel1" };
        var roomRepo = new Mock<ICommandRepository<Room>>();
        roomRepo.Setup(r => r.GetQuery()).Returns(new List<Room> { room }.AsQueryable());

        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateBookingHandler(bookingRepo.Object, roomRepo.Object, unitOfWork.Object);

        var request = new CreateBookingRequest
        {
            RoomId = "room1",
            GuestName = "Bob",
            CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(3),
            CheckOut = DateOnly.FromDateTime(DateTime.Today).AddDays(4),
            NumberOfGuests = 1
        };

        // act / assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenNumberOfGuestsExceedsCapacity_ShouldThrowInvalidOperationException()
    {
        // arrange
        var bookingRepo = new Mock<ICommandRepository<Booking>>();
        bookingRepo.Setup(r => r.GetQuery()).Returns(new List<Booking>().AsQueryable());

        var room = new Room { Id = "room1", Capacity = 1, HotelId = "hotel1" };
        var roomRepo = new Mock<ICommandRepository<Room>>();
        roomRepo.Setup(r => r.GetQuery()).Returns(new List<Room> { room }.AsQueryable());

        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateBookingHandler(bookingRepo.Object, roomRepo.Object, unitOfWork.Object);

        var request = new CreateBookingRequest
        {
            RoomId = "room1",
            GuestName = "Charlie",
            CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(1),
            CheckOut = DateOnly.FromDateTime(DateTime.Today).AddDays(2),
            NumberOfGuests = 2
        };

        // act / assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(request, CancellationToken.None));
    }
}
