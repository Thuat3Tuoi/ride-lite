using Microsoft.AspNetCore.Mvc;
using Trip.Application.Dtos;
using Trip.Application.Services;

namespace Trip.Presentation.Controllers;

[ApiController]
[Route("trips")]
public sealed class TripsController : ControllerBase
{
    private readonly TripService _tripService;

    public TripsController(TripService tripService)
    {
        _tripService = tripService;
    }

    [HttpPost]
    public async Task<ActionResult<TripResponse>> Create(
        [FromBody] RequestTripRequest request,
        CancellationToken cancellationToken)
    {
        var trip = await _tripService.RequestTripAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = trip.Id }, trip);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<TripResponse>> Get(long id, CancellationToken cancellationToken) =>
        Ok(await _tripService.GetAsync(id, cancellationToken));

    [HttpPost("{id:long}/confirm-quote")]
    public async Task<ActionResult<TripResponse>> ConfirmQuote(long id, CancellationToken cancellationToken) =>
        Ok(await _tripService.ConfirmQuoteAsync(id, cancellationToken));

    [HttpPost("{id:long}/reject-quote")]
    public async Task<ActionResult<TripResponse>> RejectQuote(long id, CancellationToken cancellationToken) =>
        Ok(await _tripService.RejectQuoteAsync(id, cancellationToken));

    [HttpPost("{id:long}/driver-accept")]
    public async Task<ActionResult<TripResponse>> DriverAccept(
        long id,
        [FromBody] DriverActionRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _tripService.DriverAcceptAsync(id, request.DriverId, cancellationToken));

    [HttpPost("{id:long}/driver-reject")]
    public async Task<ActionResult<TripResponse>> DriverReject(
        long id,
        [FromBody] DriverActionRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _tripService.DriverRejectAsync(id, request.DriverId, cancellationToken));

    [HttpPost("{id:long}/arrive")]
    public async Task<ActionResult<TripResponse>> Arrive(long id, CancellationToken cancellationToken) =>
        Ok(await _tripService.ArriveAsync(id, cancellationToken));

    [HttpPost("{id:long}/pickup")]
    public async Task<ActionResult<TripResponse>> Pickup(long id, CancellationToken cancellationToken) =>
        Ok(await _tripService.PickupAsync(id, cancellationToken));

    [HttpPost("{id:long}/dropoff")]
    public async Task<ActionResult<TripResponse>> Dropoff(long id, CancellationToken cancellationToken) =>
        Ok(await _tripService.DropoffAsync(id, cancellationToken));

    [HttpPost("{id:long}/payment-settled")]
    public async Task<ActionResult<TripResponse>> PaymentSettled(long id, CancellationToken cancellationToken) =>
        Ok(await _tripService.OnPaymentSettledAsync(id, cancellationToken));

    [HttpPost("{id:long}/payment-failed")]
    public async Task<ActionResult<TripResponse>> PaymentFailed(long id, CancellationToken cancellationToken) =>
        Ok(await _tripService.OnPaymentFailedAsync(id, cancellationToken));
}
