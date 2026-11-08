using Trip.Domain.Entities;
using Trip.Domain.Enums;
using Trip.Domain.Events;

namespace Trip.Domain.States;

public sealed class AwaitingDriverState : TripState
{
    public override TripStatus Status => TripStatus.AwaitingDriver;

    public override void DriverAccept(TripEntity trip, long driverId)
    {
        trip.SetDriver(driverId);
        trip.MarkAccepted();
        trip.TransitionTo(TripStatus.EnRoute);
        trip.Raise(new TripAccepted(trip.Id, driverId, trip.AcceptedAt!.Value));
    }

    public override void DriverReject(TripEntity trip, long driverId)
    {
        trip.IncrementDriverRejectCount();

        if (trip.DriverRejectCount > TripEntity.MaxDriverRejectCount)
        {
            const string reason = "No driver accepted the trip.";
            trip.MarkCancelled(CancelledBy.System, reason);
            trip.TransitionTo(TripStatus.Failed);
            trip.Raise(new TripFailed(trip.Id, reason, trip.CancelledAt!.Value));
            return;
        }

        trip.SetDriver(null);
        trip.Touch();
    }

    public override void Cancel(TripEntity trip, CancelledBy cancelledBy, string? reason) =>
        CancelTrip(trip, cancelledBy, reason);
}
