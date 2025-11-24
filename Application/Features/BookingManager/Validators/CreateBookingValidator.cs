using Application.Features.BookingManager.Commands;
using FluentValidation;

namespace Application.Features.BookingManager.Validators;

public class CreateBookingValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty()
            .WithMessage("RoomId is required.");

        RuleFor(x => x.GuestName)
            .NotEmpty()
            .WithMessage("Guest name is required.")
            .MaximumLength(200)
            .WithMessage("Guest name must not exceed 200 characters.");

        RuleFor(x => x.CheckIn)
            .Must(ci => ci != default)
            .WithMessage("Check-in date is required.")
            .Must(ci => ci >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Check-in cannot be in the past.");

        RuleFor(x => x.CheckOut)
            .Must(co => co != default)
            .WithMessage("Check-out date is required.");

        RuleFor(x => x.NumberOfGuests)
            .GreaterThan(0)
            .WithMessage("Number of guests must be at least 1.");

        RuleFor(x => x)
            .Must(req => req.CheckOut > req.CheckIn)
            .WithMessage("Check-out must be after check-in.");
    }
}
