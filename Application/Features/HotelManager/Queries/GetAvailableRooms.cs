using Application.Common.Repositories;
using Application.DTO;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HotelManager.Queries;

public class GetAvailableRoomsHandler : IRequestHandler<GetAvailableRoomsRequest, GetAvailableRoomsResult>
{
    private readonly ICommandRepository<Room> _roomRepository;

    public GetAvailableRoomsHandler(ICommandRepository<Room> roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<GetAvailableRoomsResult> Handle(GetAvailableRoomsRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.HotelId))
            throw new ArgumentException("HotelId is required.", nameof(request.HotelId));

        if (request.CheckIn >= request.CheckOut)
            throw new ArgumentException("CheckIn must be before CheckOut.");

        if (request.NumberOfPeople <= 0)
            throw new ArgumentException("NumberOfPeople must be greater than zero.");

        // Rooms that belong to the hotel, not deleted, and with sufficient capacity
        var query = _roomRepository
            .GetQuery()
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.HotelId == request.HotelId && r.Capacity >= request.NumberOfPeople);

        // Filter out rooms that have any overlapping bookings
        // Overlap condition: NOT (existing.CheckOut <= requested.CheckIn || existing.CheckIn >= requested.CheckOut)
        var availableRooms = await query
            .Where(r => !r.Bookings.Any(b => !(b.CheckOut <= request.CheckIn || b.CheckIn >= request.CheckOut)))
            .Select(r => new RoomDto
            {
                Id = r.Id,
                Number = r.Number,
                Capacity = r.Capacity,
                Type = r.Type.ToString(),
                HotelId = r.HotelId
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new GetAvailableRoomsResult { Data = availableRooms };
    }
}

public class GetAvailableRoomsResult
{
    public List<RoomDto> Data { get; set; } = new();
}

public class GetAvailableRoomsRequest : IRequest<GetAvailableRoomsResult>
{
    public required string HotelId { get; set; }
    public required DateOnly CheckIn { get; set; }
    public required DateOnly CheckOut { get; set; }
    public required int NumberOfPeople { get; set; }
}