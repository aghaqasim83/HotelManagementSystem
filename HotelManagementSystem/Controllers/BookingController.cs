using Application.Features.BookingManager.Commands;
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
    }
}
