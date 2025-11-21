using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.BookingManager.Commands;

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

public class CreateBookingHandler : IRequestHandler<CreateBookingRequest, CreateBookingResult>
{
    private readonly ICommandRepository<Booking> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingHandler(
        ICommandRepository<Booking> repository,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateBookingResult> Handle(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Booking
        {
            RoomId = request.RoomId
        };
        
        entity.GuestName = request.GuestName;
        entity.NumberOfGuests = request.NumberOfGuests;
        entity.CheckIn = request.CheckIn;
        entity.CheckOut = request.CheckOut;
        entity.BookingReference = Guid.NewGuid().ToString();

        await _repository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateBookingResult
        {
            Data = entity
        };
    }
}
