using Trip.Domain.Enumerators;

namespace Trip.Domain.Entities.States;

public sealed class FailedState : TripState
{
    public override TripStatus Status => TripStatus.Failed;
}
