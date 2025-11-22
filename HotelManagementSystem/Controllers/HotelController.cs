using Application.Features.HotelManager.Queries;
using HotelManagementSystem.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelController : BaseApiController
{
    public HotelController(ISender sender) : base(sender)
    {
    }

    // GET api/hotel/FindByName?name=Premier%20Inn
    [HttpGet("FindByName")]
    public async Task<ActionResult<ApiSuccessResult<GetHotelByNameResult>>> FindByName([FromQuery] string name, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetHotelByNameRequest { Name = name }, cancellationToken);

        return Ok(new ApiSuccessResult<GetHotelByNameResult>
        {
            Code = StatusCodes.Status200OK,
            Message = result.Data is null ? "Hotel not found." : "Success",
            Content = result
        });
    }

    // GET api/hotel/AvailableRooms? hotelId = { hotelId }&checkIn=2025-12-01&checkOut=2025-12-03&people=2
    [HttpGet("AvailableRooms")]
    public async Task<ActionResult<ApiSuccessResult<GetAvailableRoomsResult>>> AvailableRooms(
        [FromQuery] string hotelId,
        [FromQuery] DateOnly checkIn,
        [FromQuery] DateOnly checkOut,
        [FromQuery] int people,
        CancellationToken cancellationToken = default)
    {
        var request = new GetAvailableRoomsRequest
        {
            HotelId = hotelId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            NumberOfPeople = people
        };

        var result = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetAvailableRoomsResult>
        {
            Code = StatusCodes.Status200OK,
            Message = result.Data.Any() ? "Success" : "No available rooms",
            Content = result
        });
    }
}