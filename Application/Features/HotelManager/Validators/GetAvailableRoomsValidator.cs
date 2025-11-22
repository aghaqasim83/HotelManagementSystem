using Application.Features.HotelManager.Queries;
using FluentValidation;

namespace Application.Features.HotelManager.Validators;

public class GetAvailableRoomsValidator : AbstractValidator<GetAvailableRoomsRequest>
{
    public GetAvailableRoomsValidator()
    {
        RuleFor(x => x.HotelId)
            .NotEmpty()
            .WithMessage("HotelId is required.");

        RuleFor(x => x.CheckIn)
            .LessThan(x => x.CheckOut)
            .WithMessage("CheckIn must be before CheckOut.");

        RuleFor(x => x.NumberOfPeople)
            .GreaterThan(0)
            .WithMessage("NumberOfPeople must be greater than zero.");
    }
}