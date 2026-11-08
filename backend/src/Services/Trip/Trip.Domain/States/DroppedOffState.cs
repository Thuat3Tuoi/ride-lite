using Trip.Domain.Entities;
using Trip.Domain.Enums;

namespace Trip.Domain.States;

public sealed class DroppedOffState : TripState
{
    public override TripStatus Status => TripStatus.DroppedOff;

    public override void PaymentSettled(TripEntity trip) => trip.TransitionTo(TripStatus.Paid);

    public override void PaymentFailed(TripEntity trip) => trip.Touch();
}
