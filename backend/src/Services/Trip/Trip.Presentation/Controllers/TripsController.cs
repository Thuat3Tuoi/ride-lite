using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Contracts.Requests;
using Trip.Application.Contracts.Responses;
using Trip.Application.Trips.Commands.Arrive;
using Trip.Application.Trips.Commands.ConfirmQuote;
using Trip.Application.Trips.Commands.DriverAccept;
using Trip.Application.Trips.Commands.DriverReject;
using Trip.Application.Trips.Commands.Dropoff;
using Trip.Application.Trips.Commands.MarkPaid;
using Trip.Application.Trips.Commands.MarkPaymentFailed;
using Trip.Application.Trips.Commands.Pickup;
using Trip.Application.Trips.Commands.RejectQuote;
using Trip.Application.Trips.Commands.RequestTrip;
using Trip.Application.Trips.Queries.GetTripById;

namespace Trip.Presentation.Controllers;

[ApiController]
[Route("trips")]
public sealed class TripsController : ControllerBase
{
    private readonly ICommandHandler<RequestTripCommand, TripResponse> _requestTrip;
    private readonly IQueryHandler<GetTripByIdQuery, TripResponse> _getTripById;
    private readonly ICommandHandler<ConfirmQuoteCommand, TripResponse> _confirmQuote;
    private readonly ICommandHandler<RejectQuoteCommand, TripResponse> _rejectQuote;
    private readonly ICommandHandler<DriverAcceptCommand, TripResponse> _driverAccept;
    private readonly ICommandHandler<DriverRejectCommand, TripResponse> _driverReject;
    private readonly ICommandHandler<ArriveCommand, TripResponse> _arrive;
    private readonly ICommandHandler<PickupCommand, TripResponse> _pickup;
    private readonly ICommandHandler<DropoffCommand, TripResponse> _dropoff;
    private readonly ICommandHandler<MarkPaidCommand, TripResponse> _markPaid;
    private readonly ICommandHandler<MarkPaymentFailedCommand, TripResponse> _markPaymentFailed;

    public TripsController(
        ICommandHandler<RequestTripCommand, TripResponse> requestTrip,
        IQueryHandler<GetTripByIdQuery, TripResponse> getTripById,
        ICommandHandler<ConfirmQuoteCommand, TripResponse> confirmQuote,
        ICommandHandler<RejectQuoteCommand, TripResponse> rejectQuote,
        ICommandHandler<DriverAcceptCommand, TripResponse> driverAccept,
        ICommandHandler<DriverRejectCommand, TripResponse> driverReject,
        ICommandHandler<ArriveCommand, TripResponse> arrive,
        ICommandHandler<PickupCommand, TripResponse> pickup,
        ICommandHandler<DropoffCommand, TripResponse> dropoff,
        ICommandHandler<MarkPaidCommand, TripResponse> markPaid,
        ICommandHandler<MarkPaymentFailedCommand, TripResponse> markPaymentFailed)
    {
        _requestTrip = requestTrip;
        _getTripById = getTripById;
        _confirmQuote = confirmQuote;
        _rejectQuote = rejectQuote;
        _driverAccept = driverAccept;
        _driverReject = driverReject;
        _arrive = arrive;
        _pickup = pickup;
        _dropoff = dropoff;
        _markPaid = markPaid;
        _markPaymentFailed = markPaymentFailed;
    }

    [HttpPost]
    public async Task<ActionResult<TripResponse>> Create(
        [FromBody] RequestTripRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RequestTripCommand(
            request.RiderId,
            request.PickupAddress,
            request.PickupLatitude,
            request.PickupLongitude,
            request.DropoffAddress,
            request.DropoffLatitude,
            request.DropoffLongitude);

        var trip = await _requestTrip.HandleAsync(command, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = trip.Id }, trip);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<TripResponse>> Get(long id, CancellationToken cancellationToken) =>
        Ok(await _getTripById.HandleAsync(new GetTripByIdQuery(id), cancellationToken));

    [HttpPost("{id:long}/confirm-quote")]
    public async Task<ActionResult<TripResponse>> ConfirmQuote(long id, CancellationToken cancellationToken) =>
        Ok(await _confirmQuote.HandleAsync(new ConfirmQuoteCommand(id), cancellationToken));

    [HttpPost("{id:long}/reject-quote")]
    public async Task<ActionResult<TripResponse>> RejectQuote(
        long id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] RejectQuoteRequest? request,
        CancellationToken cancellationToken) =>
        Ok(await _rejectQuote.HandleAsync(new RejectQuoteCommand(id, request?.Reason), cancellationToken));

    [HttpPost("{id:long}/driver-accept")]
    public async Task<ActionResult<TripResponse>> DriverAccept(
        long id,
        [FromBody] DriverActionRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _driverAccept.HandleAsync(new DriverAcceptCommand(id, request.DriverId), cancellationToken));

    [HttpPost("{id:long}/driver-reject")]
    public async Task<ActionResult<TripResponse>> DriverReject(
        long id,
        [FromBody] DriverActionRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _driverReject.HandleAsync(new DriverRejectCommand(id, request.DriverId), cancellationToken));

    [HttpPost("{id:long}/arrive")]
    public async Task<ActionResult<TripResponse>> Arrive(long id, CancellationToken cancellationToken) =>
        Ok(await _arrive.HandleAsync(new ArriveCommand(id), cancellationToken));

    [HttpPost("{id:long}/pickup")]
    public async Task<ActionResult<TripResponse>> Pickup(long id, CancellationToken cancellationToken) =>
        Ok(await _pickup.HandleAsync(new PickupCommand(id), cancellationToken));

    [HttpPost("{id:long}/dropoff")]
    public async Task<ActionResult<TripResponse>> Dropoff(long id, CancellationToken cancellationToken) =>
        Ok(await _dropoff.HandleAsync(new DropoffCommand(id), cancellationToken));

    [HttpPost("{id:long}/payment-settled")]
    public async Task<ActionResult<TripResponse>> PaymentSettled(long id, CancellationToken cancellationToken) =>
        Ok(await _markPaid.HandleAsync(new MarkPaidCommand(id), cancellationToken));

    [HttpPost("{id:long}/payment-failed")]
    public async Task<ActionResult<TripResponse>> PaymentFailed(long id, CancellationToken cancellationToken) =>
        Ok(await _markPaymentFailed.HandleAsync(new MarkPaymentFailedCommand(id, null), cancellationToken));
}
