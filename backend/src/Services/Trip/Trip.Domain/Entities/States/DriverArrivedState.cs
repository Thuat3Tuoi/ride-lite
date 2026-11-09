using Trip.Domain.Entities;
using Trip.Domain.Enumerators;

namespace Trip.Domain.Entities.States;

public sealed class DriverArrivedState : TripState
{
    public override TripStatus Status => TripStatus.DriverArrived;

    public override void Pickup(TripEntity trip)
    {
        trip.MarkStarted();
        trip.TransitionTo(TripStatus.PickedUp);
    }

    public override void Cancel(TripEntity trip, CancelledBy cancelledBy, string? reason) =>
        CancelTrip(trip, cancelledBy, reason);
}
