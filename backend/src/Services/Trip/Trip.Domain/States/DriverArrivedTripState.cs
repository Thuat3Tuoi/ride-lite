using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

public sealed class DriverArrivedTripState : TripState
{
    internal DriverArrivedTripState() { }

    public override TripStatus Status => TripStatus.DRIVER_ARRIVED;

    public override void StartTrip(Entities.Trip trip)
    {
        trip.SetStartedAt(DateTimeOffset.UtcNow);
        trip.TransitionTo(InProgress);
    }

    public override void Cancel(Entities.Trip trip, CancelledBy cancelledBy, string? reason)
    {
        trip.SetCancellation(cancelledBy, reason, DateTimeOffset.UtcNow);
        trip.TransitionTo(Cancelled);
    }
}

