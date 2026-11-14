using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

public sealed class CancelledTripState : TripState
{
    internal CancelledTripState() { }

    public override TripStatus Status => TripStatus.CANCELLED;
}

