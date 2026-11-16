using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

public sealed class SearchingDriverTripState : TripState
{
    internal SearchingDriverTripState() { }

    public override TripStatus Status => TripStatus.SEARCHING_DRIVER;

    public override void AcceptDriver(Entities.Trip trip, long driverId)
    {
        if (driverId <= 0)
            throw new ArgumentException("Driver ID must be a positive value.");

        trip.SetDriver(driverId);
        trip.SetAcceptedAt(DateTimeOffset.UtcNow);
        trip.TransitionTo(TripStateFactory.Instance.DriverAccepted);
    }

    public override void Cancel(Entities.Trip trip, CancelledBy cancelledBy, string? reason)
    {
        trip.SetCancellation(cancelledBy, reason, DateTimeOffset.UtcNow);
        trip.TransitionTo(TripStateFactory.Instance.Cancelled);
    }

    public override void MarkNoDriverFound(Entities.Trip trip)
    {
        trip.TransitionTo(TripStateFactory.Instance.NoDriverFound);
    }
}

