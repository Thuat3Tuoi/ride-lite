using Trip.Domain.Entities;
using Trip.Domain.Enumerators;
using Trip.Domain.DomainEvents;
using Trip.Domain.Exceptions;
using Trip.Domain.ValueObjects;

namespace Trip.Domain.Entities.States;

public abstract class TripState
{
    public abstract TripStatus Status { get; }

    public virtual void Quote(TripEntity trip, Money cost, long driverId) =>
        throw Invalid(trip, "quote");

    public virtual void ConfirmQuote(TripEntity trip) =>
        throw Invalid(trip, "confirm the quote of");

    public virtual void RejectQuote(TripEntity trip, string? reason) =>
        throw Invalid(trip, "reject the quote of");

    public virtual void DriverAccept(TripEntity trip, long driverId) =>
        throw Invalid(trip, "accept");

    public virtual void DriverReject(TripEntity trip, long driverId) =>
        throw Invalid(trip, "reject");

    public virtual void Arrive(TripEntity trip) =>
        throw Invalid(trip, "mark arrival on");

    public virtual void Pickup(TripEntity trip) =>
        throw Invalid(trip, "pick up");

    public virtual void Dropoff(TripEntity trip) =>
        throw Invalid(trip, "drop off");

    public virtual void PaymentSettled(TripEntity trip) =>
        throw Invalid(trip, "settle payment for");

    public virtual void PaymentFailed(TripEntity trip) =>
        throw Invalid(trip, "fail payment for");

    public virtual void Cancel(TripEntity trip, CancelledBy cancelledBy, string? reason) =>
        throw Invalid(trip, "cancel");

    protected InvalidTripTransitionException Invalid(TripEntity trip, string action) =>
        new($"Cannot {action} trip {trip.Id} while it is in status '{Status}'.");

    protected static void CancelTrip(TripEntity trip, CancelledBy cancelledBy, string? reason)
    {
        trip.MarkCancelled(cancelledBy, reason);
        trip.TransitionTo(TripStatus.Cancelled);
        trip.Raise(new TripCancelled(trip.Id, cancelledBy, reason, trip.CancelledAt!.Value));
    }

    public static TripState For(TripStatus status) => status switch
    {
        TripStatus.Requested => new RequestedState(),
        TripStatus.Quoted => new QuotedState(),
        TripStatus.AwaitingDriver => new AwaitingDriverState(),
        TripStatus.EnRoute => new EnRouteState(),
        TripStatus.DriverArrived => new DriverArrivedState(),
        TripStatus.PickedUp => new PickedUpState(),
        TripStatus.DroppedOff => new DroppedOffState(),
        TripStatus.PaymentPending => new PaymentPendingState(),
        TripStatus.Paid => new PaidState(),
        TripStatus.Cancelled => new CancelledState(),
        TripStatus.Failed => new FailedState(),
        _ => throw new DomainException($"Unsupported trip status '{status}'.")
    };
}
