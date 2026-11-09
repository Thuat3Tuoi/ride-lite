using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trip.Application.Features.Trip.Commands.AcceptTripDriver;
using Trip.Application.Features.Trip.Commands.CancelTrip;
using Trip.Application.Features.Trip.Commands.CompleteTrip;
using Trip.Application.Features.Trip.Commands.CreateTrip;
using Trip.Application.Features.Trip.Commands.DriverArrive;
using Trip.Application.Features.Trip.Commands.MarkNoDriverFound;
using Trip.Application.Features.Trip.Commands.StartTrip;
using Trip.Application.Features.Trip.Queries.GetTripById;
using Trip.Application.Features.Trip.Queries.GetTripsByDriverId;
using Trip.Application.Features.Trip.Queries.GetTripsByRiderId;
using Trip.Application.Features.Trip.Queries.GetTripsByStatus;
using Trip.Domain.Enumerators;
using Trip.Presentation.Contracts.Requests;
using Trip.Presentation.Contracts.Responses;

namespace Trip.Presentation.Controllers;

[ApiController]
[Route("api/trips")]
public sealed class TripsController : ControllerBase
{
    private readonly CreateTripHandler _createTripHandler;
    private readonly AcceptTripDriverHandler _acceptTripDriverHandler;
    private readonly DriverArriveHandler _driverArriveHandler;
    private readonly StartTripHandler _startTripHandler;
    private readonly CompleteTripHandler _completeTripHandler;
    private readonly CancelTripHandler _cancelTripHandler;
    private readonly MarkNoDriverFoundHandler _markNoDriverFoundHandler;
    private readonly GetTripByIdHandler _getTripByIdHandler;
    private readonly GetTripsByRiderIdHandler _getTripsByRiderIdHandler;
    private readonly GetTripsByDriverIdHandler _getTripsByDriverIdHandler;
    private readonly GetTripsByStatusHandler _getTripsByStatusHandler;

    public TripsController(
        CreateTripHandler createTripHandler,
        AcceptTripDriverHandler acceptTripDriverHandler,
        DriverArriveHandler driverArriveHandler,
        StartTripHandler startTripHandler,
        CompleteTripHandler completeTripHandler,
        CancelTripHandler cancelTripHandler,
        MarkNoDriverFoundHandler markNoDriverFoundHandler,
        GetTripByIdHandler getTripByIdHandler,
        GetTripsByRiderIdHandler getTripsByRiderIdHandler,
        GetTripsByDriverIdHandler getTripsByDriverIdHandler,
        GetTripsByStatusHandler getTripsByStatusHandler)
    {
        _createTripHandler = createTripHandler;
        _acceptTripDriverHandler = acceptTripDriverHandler;
        _driverArriveHandler = driverArriveHandler;
        _startTripHandler = startTripHandler;
        _completeTripHandler = completeTripHandler;
        _cancelTripHandler = cancelTripHandler;
        _markNoDriverFoundHandler = markNoDriverFoundHandler;
        _getTripByIdHandler = getTripByIdHandler;
        _getTripsByRiderIdHandler = getTripsByRiderIdHandler;
        _getTripsByDriverIdHandler = getTripsByDriverIdHandler;
        _getTripsByStatusHandler = getTripsByStatusHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TripResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripResponse>> Create(
        [FromBody] CreateTripRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTripCommand(
            request.RiderId,
            request.PickupAddress,
            request.PickupLatitude,
            request.PickupLongitude,
            request.DropoffAddress,
            request.DropoffLatitude,
            request.DropoffLongitude,
            request.ClientFare
        );

        var result = await _createTripHandler.HandleAsync(command, cancellationToken);

        var response = new TripResponse(
            result.Id,
            result.DriverId,
            result.RiderId,
            result.PickupAddress,
            result.PickupLatitude,
            result.PickupLongitude,
            result.DropoffAddress,
            result.DropoffLatitude,
            result.DropoffLongitude,
            result.Cost,
            result.Status,
            result.CreatedAt,
            result.UpdatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(TripResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripResponse>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var query = new GetTripByIdQuery(id);
        var result = await _getTripByIdHandler.HandleAsync(query, cancellationToken);

        return Ok(MapToResponse(result));
    }

    [HttpGet("rider/{riderId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<TripResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TripResponse>>> GetByRiderId(
        long riderId,
        CancellationToken cancellationToken)
    {
        var query = new GetTripsByRiderIdQuery(riderId);
        var result = await _getTripsByRiderIdHandler.HandleAsync(query, cancellationToken);

        var response = result.Items.Select(MapToResponse).ToList();
        return Ok(response);
    }

    [HttpGet("driver/{driverId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<TripResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TripResponse>>> GetByDriverId(
        long driverId,
        CancellationToken cancellationToken)
    {
        var query = new GetTripsByDriverIdQuery(driverId);
        var result = await _getTripsByDriverIdHandler.HandleAsync(query, cancellationToken);

        var response = result.Items.Select(MapToResponse).ToList();
        return Ok(response);
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IReadOnlyList<TripResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TripResponse>>> GetByStatus(
        TripStatus status,
        CancellationToken cancellationToken)
    {
        var query = new GetTripsByStatusQuery(status);
        var result = await _getTripsByStatusHandler.HandleAsync(query, cancellationToken);

        var response = result.Items.Select(MapToResponse).ToList();
        return Ok(response);
    }

    [HttpPost("{id:long}/accept-driver")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptDriver(
        long id,
        [FromBody] AcceptTripDriverRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AcceptTripDriverCommand(id, request.DriverId);
        var result = await _acceptTripDriverHandler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:long}/driver-arrived")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DriverArrived(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new DriverArriveCommand(id);
        var result = await _driverArriveHandler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:long}/start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartTrip(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new StartTripCommand(id);
        var result = await _startTripHandler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:long}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTrip(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new CompleteTripCommand(id);
        var result = await _completeTripHandler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:long}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelTrip(
        long id,
        [FromBody] CancelTripRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CancelTripCommand(id, request.CancelledBy, request.Reason);
        var result = await _cancelTripHandler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:long}/no-driver")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkNoDriver(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new MarkNoDriverFoundCommand(id);
        var result = await _markNoDriverFoundHandler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    private static TripResponse MapToResponse(GetTripByIdResult result)
    {
        return new TripResponse(
            result.Id,
            result.DriverId,
            result.RiderId,
            result.PickupAddress,
            result.PickupLatitude,
            result.PickupLongitude,
            result.DropoffAddress,
            result.DropoffLatitude,
            result.DropoffLongitude,
            result.Cost,
            result.Status,
            result.CreatedAt,
            result.UpdatedAt,
            result.AcceptedAt,
            result.DriverArrivedAt,
            result.StartedAt,
            result.CompletedAt,
            result.CancelledAt,
            result.CancelledBy,
            result.CancelReason
        );
    }
}

