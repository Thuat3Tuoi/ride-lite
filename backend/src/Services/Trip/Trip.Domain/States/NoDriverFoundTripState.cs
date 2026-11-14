using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

public sealed class NoDriverFoundTripState : TripState
{
    internal NoDriverFoundTripState() { }

    public override TripStatus Status => TripStatus.NO_DRIVER_FOUND;
}

