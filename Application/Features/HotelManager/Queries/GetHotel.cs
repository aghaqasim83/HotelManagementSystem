using Application.Common.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HotelManager.Queries;

public class GetHotelByNameHandler : IRequestHandler<GetHotelByNameRequest, GetHotelByNameResult>
{
    private readonly ICommandRepository<Hotel> _hotelRepository;

    public GetHotelByNameHandler(ICommandRepository<Hotel> hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    public async Task<GetHotelByNameResult> Handle(GetHotelByNameRequest request, CancellationToken cancellationToken = default)
    {
        var nameNormalized = request.Name.Trim().ToLowerInvariant();

        var hotel = await _hotelRepository
            .GetQuery()
            .AsNoTracking()
            .Include(h => h.Rooms)
            .Where(h => !h.IsDeleted && h.Name != null && EF.Functions.Like(h.Name, request.Name))
            // EF.Functions.Like will match exact case-insensitively depending on DB collation;
            // fallback to ToLower comparison for portability:
            .FirstOrDefaultAsync(h => !h.IsDeleted && h.Name != null && h.Name.ToLower() == nameNormalized, cancellationToken)
            .ConfigureAwait(false);

        if (hotel is null)
            return new GetHotelByNameResult { Data = null };

        var dto = new GetHotelByNameResult.HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
        };

        return new GetHotelByNameResult { Data = dto };
    }
}

public class GetHotelByNameResult
{
    public HotelDto? Data { get; set; }

    public class HotelDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}

public class GetHotelByNameRequest : IRequest<GetHotelByNameResult>
{
    public required string Name { get; set; }
}