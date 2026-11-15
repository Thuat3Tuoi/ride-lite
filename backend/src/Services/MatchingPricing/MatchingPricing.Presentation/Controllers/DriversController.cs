using MatchingPricing.Application.Abstractions.Messaging;
using MatchingPricing.Application.Features.Driver.Commands.CreateDriver;
using MatchingPricing.Application.Features.Driver.Commands.UpdateDriverLocation;
using MatchingPricing.Application.Features.Driver.Commands.UpdateDriverStatus;
using MatchingPricing.Application.Features.Driver.Queries.FindNearbyDrivers;
using MatchingPricing.Application.Features.Driver.Queries.GetDriverById;
using MatchingPricing.Presentation.Contracts.Requests;
using MatchingPricing.Presentation.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing.Presentation.Controllers;

[ApiController]
[Route("api/drivers")]
public sealed class DriversController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(DriverResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DriverResponse>> Create(
        [FromBody] CreateDriverRequest request,
        [FromServices] ICommandHandler<CreateDriverCommand, CreateDriverResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateDriverCommand(
            request.FullName,
            request.PhoneNumber,
            request.DateOfBirth,
            request.Status
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new DriverResponse(
            result.Id,
            result.FullName,
            result.PhoneNumber,
            result.DateOfBirth,
            null,
            null,
            null,
            result.Status,
            null,
            result.CreatedAt,
            result.UpdatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DriverResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverResponse>> GetById(
        long id,
        [FromServices] IQueryHandler<GetDriverByIdQuery, GetDriverByIdResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetDriverByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = new DriverResponse(
            result.Id,
            result.FullName,
            result.PhoneNumber,
            result.DateOfBirth,
            result.Latitude,
            result.Longitude,
            result.Location,
            result.Status,
            result.LocationUpdatedAt,
            result.CreatedAt,
            result.UpdatedAt
        );

        return Ok(response);
    }

    [HttpPost("{id:long}/location")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocation(
        long id,
        [FromBody] UpdateDriverLocationRequest request,
        [FromServices] ICommandHandler<UpdateDriverLocationCommand, UpdateDriverLocationResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDriverLocationCommand(id, request.Latitude, request.Longitude);
        var result = await handler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:long}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        long id,
        [FromBody] UpdateDriverStatusRequest request,
        [FromServices] ICommandHandler<UpdateDriverStatusCommand, UpdateDriverStatusResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDriverStatusCommand(id, request.Status);
        var result = await handler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpGet("nearby")]
    [ProducesResponseType(typeof(IReadOnlyList<NearbyDriverResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<NearbyDriverResponse>>> FindNearby(
        [FromQuery] decimal pickupLatitude,
        [FromQuery] decimal pickupLongitude,
        [FromServices] IQueryHandler<FindNearbyDriversQuery, FindNearbyDriversResult> handler,
        [FromQuery] double radiusKm = 10.0,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new FindNearbyDriversQuery(pickupLatitude, pickupLongitude, radiusKm, limit);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = result.Drivers.Select(d => new NearbyDriverResponse(
            new DriverResponse(
                d.Driver.Id,
                d.Driver.FullName,
                d.Driver.PhoneNumber,
                d.Driver.DateOfBirth,
                d.Driver.Latitude,
                d.Driver.Longitude,
                d.Driver.Location,
                d.Driver.Status,
                d.Driver.LocationUpdatedAt,
                d.Driver.CreatedAt,
                d.Driver.UpdatedAt
            ),
            d.DistanceKm
        )).ToList();

        return Ok(response);
    }
}
