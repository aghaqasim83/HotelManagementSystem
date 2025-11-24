using Application.Common.Repositories;
using Application.DTO;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HotelManager.Queries;

public class GetBookingByReferenceHandler : IRequestHandler<GetBookingByReferenceRequest, GetBookingByReferenceResult>
{
    private readonly ICommandRepository<Booking> _bookingRepository;

    public GetBookingByReferenceHandler(ICommandRepository<Booking> bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<GetBookingByReferenceResult> Handle(GetBookingByReferenceRequest request, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository
            .GetQuery()
            .AsNoTracking()
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => !b.IsDeleted && b.BookingReference == request.Reference, cancellationToken)
            .ConfigureAwait(false);

        if (booking is null)
            return new GetBookingByReferenceResult { Data = null };

        var dto = new BookingDto
        {
            Id = booking.Id,
            BookingReference = booking.BookingReference,
            GuestName = booking.GuestName,
            RoomId = booking.RoomId,
            RoomNumber = booking.Room?.Number,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            NumberOfGuests = booking.NumberOfGuests,
            Room = booking.Room is null ? null : new RoomDto
            {
                Id = booking.Room.Id,
                Number = booking.Room.Number,
                Capacity = booking.Room.Capacity,
                Type = (int)booking.Room.Type,
                HotelId = booking.Room.HotelId
            },
        };

        return new GetBookingByReferenceResult { Data = dto };
    }
}

public class GetBookingByReferenceResult
{
    public BookingDto? Data { get; set; }
}

public class GetBookingByReferenceRequest : IRequest<GetBookingByReferenceResult>
{
    public required string Reference { get; set; }
}