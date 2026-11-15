using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trip.Application.Features.Rider.Commands.CreateRider;
using Trip.Application.Features.Rider.Commands.UpdateRiderProfile;
using Trip.Application.Features.Rider.Queries.GetRiderById;
using Trip.Application.Features.Rider.Queries.GetRiders;
using Trip.Presentation.Contracts.Requests;
using Trip.Presentation.Contracts.Responses;

namespace Trip.Presentation.Controllers;

[ApiController]
[Route("api/riders")]
public sealed class RidersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(RiderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RiderResponse>> Create(
        [FromBody] CreateRiderRequest request,
        [FromServices] CreateRiderHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateRiderCommand(
            request.FullName,
            request.PhoneNumber,
            request.DateOfBirth
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new RiderResponse(
            result.Id,
            result.FullName,
            result.PhoneNumber,
            result.DateOfBirth,
            result.CreatedAt,
            result.UpdatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(RiderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiderResponse>> GetById(
        long id,
        [FromServices] GetRiderByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetRiderByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = new RiderResponse(
            result.Id,
            result.FullName,
            result.PhoneNumber,
            result.DateOfBirth,
            result.CreatedAt,
            result.UpdatedAt
        );

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RiderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RiderResponse>>> GetAll(
        [FromServices] GetRidersHandler handler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRidersQuery(page, pageSize);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = result.Items.Select(r => new RiderResponse(
            r.Id,
            r.FullName,
            r.PhoneNumber,
            r.DateOfBirth,
            r.CreatedAt,
            r.UpdatedAt
        )).ToList();

        return Ok(response);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(RiderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RiderResponse>> UpdateProfile(
        long id,
        [FromBody] UpdateRiderProfileRequest request,
        [FromServices] UpdateRiderProfileHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRiderProfileCommand(
            id,
            request.FullName,
            request.PhoneNumber,
            request.DateOfBirth
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new RiderResponse(
            result.Id,
            result.FullName,
            result.PhoneNumber,
            result.DateOfBirth,
            result.UpdatedAt,
            result.UpdatedAt
        );

        return Ok(response);
    }
}
