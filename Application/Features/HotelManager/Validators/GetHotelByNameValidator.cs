using Application.Features.HotelManager.Queries;
using FluentValidation;
using static Domain.Common.Constants;

namespace Application.Features.HotelManager.Validators;

public class GetHotelByNameValidator : AbstractValidator<GetHotelByNameRequest>
{
    public GetHotelByNameValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Hotel name is required.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("Hotel name must not be whitespace.")
            .MaximumLength(NameConsts.MaxLength).WithMessage($"Hotel name must be at most {NameConsts.MaxLength} characters.");
    }
}