using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

public sealed class InProgressTripState : TripState
{
    internal InProgressTripState() { }

    public override TripStatus Status => TripStatus.IN_PROGRESS;

    public override void CompleteTrip(Entities.Trip trip)
    {
        trip.SetCompletedAt(DateTimeOffset.UtcNow);
        trip.TransitionTo(TripStateFactory.Instance.Completed);
    }

    public override void Cancel(Entities.Trip trip, CancelledBy cancelledBy, string? reason)
    {
        trip.SetCancellation(cancelledBy, reason, DateTimeOffset.UtcNow);
        trip.TransitionTo(TripStateFactory.Instance.Cancelled);
    }
}

