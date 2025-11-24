using Application.Common.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BookingManager.Commands;

public class CreateBookingHandler : IRequestHandler<CreateBookingRequest, CreateBookingResult>
{
    private readonly ICommandRepository<Booking> _repository;
    private readonly ICommandRepository<Room> _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingHandler(
        ICommandRepository<Booking> repository,
        ICommandRepository<Room> roomRepository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateBookingResult> Handle(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        if (!(await CheckIfBookingCanBeMade(request, cancellationToken)))
        {
            return new CreateBookingResult();
        }

        await EnsureSingleRoomForEntireStayAsync(request, cancellationToken);

        var entity = new Booking
        {
            RoomId = request.RoomId,
            GuestName = request.GuestName,
            NumberOfGuests = request.NumberOfGuests,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            BookingReference = Guid.NewGuid().ToString()
        };

        await _repository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateBookingResult
        {
            Data = entity
        };
    }

    private async Task<bool> CheckIfBookingCanBeMade(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(request.RoomId))
            throw new ArgumentException("RoomId is required.", nameof(request.RoomId));

        if (request.CheckIn >= request.CheckOut)
            throw new ArgumentException("CheckIn must be before CheckOut.");

        if (request.NumberOfGuests <= 0)
            throw new ArgumentException("NumberOfGuests must be greater than zero.", nameof(request.NumberOfGuests));

        // Validate room exists and capacity
        var room = await _roomRepository
            .GetQuery()
            .Where(r => r.Id == request.RoomId && !r.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (room is null)
            throw new InvalidOperationException("Room not found.");

        if (request.NumberOfGuests > room.Capacity)
            throw new InvalidOperationException($"Requested number of guests ({request.NumberOfGuests}) exceeds room capacity ({room.Capacity}).");

        // Check for overlapping bookings for the same room.
        // Overlap exists when NOT (existing.CheckOut <= requested.CheckIn || existing.CheckIn >= requested.CheckOut)
        var query = _repository.GetQuery()
            .Where(b => b.RoomId == request.RoomId && !b.IsDeleted);

        var overlapExists = await query
            .Where(b => !(b.CheckOut <= request.CheckIn || b.CheckIn >= request.CheckOut))
            .AnyAsync(cancellationToken)
            .ConfigureAwait(false);

        if (overlapExists)
            throw new InvalidOperationException("The room is already booked for one or more nights in the requested date range.");

        return true;
    }

    /// <summary>
    /// Ensures a single room is available for the entire stay so guests do not need to change rooms.
    /// Behavior:
    /// - If the request specifies a RoomId, confirms that this room is free for the whole date range.
    /// - If the request's RoomId is not available (or is empty), attempts to find any room in the hotel
    ///   with sufficient capacity that is free for the entire date range and assigns it to the request.
    /// Throws InvalidOperationException if no single-room solution exists.
    /// </summary>
    private async Task EnsureSingleRoomForEntireStayAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        // If a specific room is requested, check it is free for the whole stay
        if (!string.IsNullOrWhiteSpace(request.RoomId))
        {
            var overlap = await _repository.GetQuery()
                .Where(b => b.RoomId == request.RoomId && !b.IsDeleted)
                .Where(b => !(b.CheckOut <= request.CheckIn || b.CheckIn >= request.CheckOut))
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false);

            if (overlap)
            {
                // requested room is not free for entire stay — no room change allowed
                throw new InvalidOperationException("Requested room is not available for the entire stay; guests must not be required to change rooms.");
            }

            // requested room is free for the whole stay — OK
            return;
        }

        // If no specific room provided (defensive; current request requires RoomId), attempt to find any single room
        // with sufficient capacity that is free for the entire date range.
        var candidateRoom = await _roomRepository.GetQuery()
            .Where(r => !r.IsDeleted && r.Capacity >= request.NumberOfGuests)
            .Where(r => !r.Bookings.Any(b => !(b.CheckOut <= request.CheckIn || b.CheckIn >= request.CheckOut)))
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (candidateRoom is null)
        {
            throw new InvalidOperationException("No single room is available for the entire stay. Guests cannot be required to change rooms during their stay.");
        }

        // Assign the found room to the request so later processing will book that room
        request.RoomId = candidateRoom.Id;
    }
}

public class CreateBookingResult
{
    public Booking? Data { get; set; }
}

public class CreateBookingRequest : IRequest<CreateBookingResult>
{
    public required string RoomId { get; set; }
    public string GuestName { get; set; } = null!;
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int NumberOfGuests { get; set; }
}
