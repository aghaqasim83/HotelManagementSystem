using Application.Common.Repositories;
using Application.DTO;
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
        var pattern = $"%{nameNormalized}%";

        var hotels = await _hotelRepository
            .GetQuery()
            .AsNoTracking()
            .Include(h => h.Rooms)
            .Where(h => !h.IsDeleted && h.Name != null)
            .Where(h =>
                EF.Functions.Like(h.Name.ToLower(), pattern) // substring (partial) match, case-insensitive
                || h.Name.ToLower() == nameNormalized)       // exact match fallback
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var dtos = hotels.Select(h => new HotelDto
        {
            Id = h.Id,
            Name = h.Name,
        }).ToList();

        return new GetHotelByNameResult { Data = dtos };
    }
}

public class GetHotelByNameResult
{
    public List<HotelDto> Data { get; set; } = new();
}

public class GetHotelByNameRequest : IRequest<GetHotelByNameResult>
{
    public required string Name { get; set; }
}