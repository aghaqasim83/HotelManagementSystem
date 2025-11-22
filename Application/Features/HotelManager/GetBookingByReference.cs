using Application.Common.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BookingManager.Queries;

public class GetBookingByReferenceResult
{
    public BookingDto? Data { get; set; }

    public class BookingDto
    {
        public string Id { get; set; } = null!;
        public string BookingReference { get; set; } = null!;
        public string GuestName { get; set; } = null!;
        public string RoomId { get; set; } = null!;
        public string? RoomNumber { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int NumberOfGuests { get; set; }
    }
}

public class GetBookingByReferenceRequest : IRequest<GetBookingByReferenceResult>
{
    public required string Reference { get; set; }
}

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

        var dto = new GetBookingByReferenceResult.BookingDto
        {
            Id = booking.Id,
            BookingReference = booking.BookingReference,
            GuestName = booking.GuestName,
            RoomId = booking.RoomId,
            RoomNumber = booking.Room?.Number,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            NumberOfGuests = booking.NumberOfGuests
        };

        return new GetBookingByReferenceResult { Data = dto };
    }
}