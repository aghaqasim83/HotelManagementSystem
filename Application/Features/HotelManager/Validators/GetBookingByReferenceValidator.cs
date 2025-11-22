using Application.Features.BookingManager.Queries;
using FluentValidation;

namespace Application.Features.BookingManager.Validators;

public class GetBookingByReferenceValidator : AbstractValidator<GetBookingByReferenceRequest>
{
    public GetBookingByReferenceValidator()
    {
        RuleFor(x => x.Reference)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Booking reference is required.")
            .MaximumLength(200).WithMessage("Booking reference is too long.");
    }
}