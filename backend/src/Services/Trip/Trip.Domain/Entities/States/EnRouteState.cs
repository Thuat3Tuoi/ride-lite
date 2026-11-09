using Trip.Domain.Entities;
using Trip.Domain.Enumerators;

namespace Trip.Domain.Entities.States;

public sealed class EnRouteState : TripState
{
    public override TripStatus Status => TripStatus.EnRoute;

    public override void Arrive(TripEntity trip)
    {
        trip.MarkDriverArrived();
        trip.TransitionTo(TripStatus.DriverArrived);
    }

    public override void Cancel(TripEntity trip, CancelledBy cancelledBy, string? reason) =>
        CancelTrip(trip, cancelledBy, reason);
}
