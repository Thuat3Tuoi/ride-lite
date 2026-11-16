using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

public sealed class DriverAcceptedTripState : TripState
{
    internal DriverAcceptedTripState() { }

    public override TripStatus Status => TripStatus.DRIVER_ACCEPTED;

    public override void DriverArrive(Entities.Trip trip)
    {
        trip.SetDriverArrivedAt(DateTimeOffset.UtcNow);
        trip.TransitionTo(TripStateFactory.Instance.DriverArrived);
    }

    public override void Cancel(Entities.Trip trip, CancelledBy cancelledBy, string? reason)
    {
        trip.SetCancellation(cancelledBy, reason, DateTimeOffset.UtcNow);
        trip.TransitionTo(TripStateFactory.Instance.Cancelled);
    }
}
