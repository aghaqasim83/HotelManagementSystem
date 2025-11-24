using Application.Features.HotelManager.Queries;
using Application.Features.HotelManager.Validators;
using FluentValidation.TestHelper;
using Xunit;
namespace Application.UnitTests;

public class ValidatorsTests
{
    [Fact]
    public void GetAvailableRoomsValidator_FailsOnInvalid()
    {
        var validator = new GetAvailableRoomsValidator();
        var request = new GetAvailableRoomsRequest
        {
            HotelId = "",
            CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(3),
            CheckOut = DateOnly.FromDateTime(DateTime.Today).AddDays(2),
            NumberOfPeople = 0
        };

        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.HotelId);
        result.ShouldHaveValidationErrorFor(x => x.NumberOfPeople);
    }

    [Fact]
    public void GetHotelByNameValidator_FailsOnEmpty()
    {
        var validator = new GetHotelByNameValidator();
        var request = new GetHotelByNameRequest { Name = "" };
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}