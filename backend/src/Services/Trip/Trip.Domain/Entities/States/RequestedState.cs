using Trip.Domain.Entities;
using Trip.Domain.Enumerators;
using Trip.Domain.ValueObjects;

namespace Trip.Domain.Entities.States;

public sealed class RequestedState : TripState
{
    public override TripStatus Status => TripStatus.Requested;

    public override void Quote(TripEntity trip, Money cost, long driverId)
    {
        trip.SetCost(cost);
        trip.SetDriver(driverId);
        trip.TransitionTo(TripStatus.Quoted);
    }

    public override void Cancel(TripEntity trip, CancelledBy cancelledBy, string? reason)
    {
        if (cancelledBy == CancelledBy.Driver)
        {
            throw Invalid(trip, "be cancelled by a driver");
        }

        CancelTrip(trip, cancelledBy, reason);
    }
}
