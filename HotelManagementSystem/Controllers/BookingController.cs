using Application.Features.BookingManager.Commands;
using Application.Features.BookingManager.Queries;
using HotelManagementSystem.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : BaseApiController
    {
        public BookingController(ISender sender) : base(sender)
        {
        }

        [HttpPost]
        public async Task<ActionResult<ApiSuccessResult<CreateBookingResult>>> CreateCustomerCategoryAsync(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(request, cancellationToken);

            return Ok(new ApiSuccessResult<CreateBookingResult>
            {
                Code = StatusCodes.Status200OK,
                Message = $"Success executing {nameof(CreateCustomerCategoryAsync)}",
                Content = response
            });
        }

        // GET api/booking/FindByReference?reference={bookingReference}
        [HttpGet("FindByReference")]
        public async Task<ActionResult<ApiSuccessResult<GetBookingByReferenceResult>>> FindByReference([FromQuery] string reference, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetBookingByReferenceRequest { Reference = reference }, cancellationToken);

            return Ok(new ApiSuccessResult<GetBookingByReferenceResult>
            {
                Code = StatusCodes.Status200OK,
                Message = result.Data is null ? "Booking not found." : "Success",
                Content = result
            });
        }
    }
}
