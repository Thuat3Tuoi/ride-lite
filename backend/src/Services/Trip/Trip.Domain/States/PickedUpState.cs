using Trip.Domain.Entities;
using Trip.Domain.Enums;
using Trip.Domain.Events;
using Trip.Domain.Exceptions;

namespace Trip.Domain.States;

public sealed class PickedUpState : TripState
{
    public override TripStatus Status => TripStatus.PickedUp;

    public override void Dropoff(TripEntity trip)
    {
        var amount = trip.Cost
            ?? throw new DomainException($"Cannot complete trip {trip.Id} because it has no cost.");

        trip.MarkCompleted();
        trip.TransitionTo(TripStatus.DroppedOff);
        trip.Raise(new RideCompleted(
            trip.Id,
            trip.RiderId,
            amount,
            $"trip-{trip.Id}-payment",
            trip.CompletedAt!.Value));
        trip.TransitionTo(TripStatus.PaymentPending);
    }
}
