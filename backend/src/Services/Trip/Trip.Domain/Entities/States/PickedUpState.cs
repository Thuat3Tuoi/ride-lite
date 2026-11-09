using Trip.Domain.Constants;
using Trip.Domain.Entities;
using Trip.Domain.Enumerators;
using Trip.Domain.DomainEvents;
using Trip.Domain.Exceptions;

namespace Trip.Domain.Entities.States;

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
            TripRules.PaymentIdempotencyKey(trip.Id),
            trip.CompletedAt!.Value));
        trip.TransitionTo(TripStatus.PaymentPending);
    }
}
