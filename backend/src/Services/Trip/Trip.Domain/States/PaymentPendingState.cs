using Trip.Domain.Entities;
using Trip.Domain.Enums;

namespace Trip.Domain.States;

public sealed class PaymentPendingState : TripState
{
    public override TripStatus Status => TripStatus.PaymentPending;

    public override void PaymentSettled(TripEntity trip) => trip.TransitionTo(TripStatus.Paid);

    public override void PaymentFailed(TripEntity trip) => trip.Touch();
}
