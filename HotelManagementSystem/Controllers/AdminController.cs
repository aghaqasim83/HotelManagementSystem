using Application.Features.DatabaseManager;
using HotelManagementSystem.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : BaseApiController
{
    public AdminController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Drop and recreate the database schema. Development only.
    /// POST api/admin/Clear
    /// </summary>
    [HttpPost("Clear")]
    public async Task<ActionResult<ApiSuccessResult<ClearDatabaseResult>>> Clear(CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new ClearDatabaseRequest(), cancellationToken);

        return Ok(new ApiSuccessResult<ClearDatabaseResult>
        {
            Code = StatusCodes.Status200OK,
            Message = result.Success ? result.Message ?? "Cleared" : result.Message ?? "Failed",
            Content = result
        });
    }

    /// <summary>
    /// Seed demo data into the database. Development only.
    /// POST api/admin/Seed?seedDemo=true
    /// </summary>
    [HttpPost("Seed")]
    public async Task<ActionResult<ApiSuccessResult<SeedDatabaseResult>>> Seed([FromQuery] bool seedDemo = true, CancellationToken cancellationToken = default)
    {
        var request = new SeedDatabaseRequest { SeedData = seedDemo };
        var result = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<SeedDatabaseResult>
        {
            Code = StatusCodes.Status200OK,
            Message = result.Success ? result.Message ?? "Seeded" : result.Message ?? "Failed",
            Content = result
        });
    }
}