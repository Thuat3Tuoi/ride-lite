using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

public sealed class CompletedTripState : TripState
{
    internal CompletedTripState() { }

    public override TripStatus Status => TripStatus.COMPLETED;
}

