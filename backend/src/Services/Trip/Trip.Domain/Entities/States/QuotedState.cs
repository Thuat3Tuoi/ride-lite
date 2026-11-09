using Trip.Domain.Entities;
using Trip.Domain.Enumerators;

namespace Trip.Domain.Entities.States;

public sealed class QuotedState : TripState
{
    public override TripStatus Status => TripStatus.Quoted;

    public override void ConfirmQuote(TripEntity trip) => trip.TransitionTo(TripStatus.AwaitingDriver);

    public override void RejectQuote(TripEntity trip, string? reason) =>
        CancelTrip(trip, CancelledBy.Rider, reason ?? "Quote rejected by rider.");

    public override void Cancel(TripEntity trip, CancelledBy cancelledBy, string? reason) =>
        CancelTrip(trip, cancelledBy, reason);
}
