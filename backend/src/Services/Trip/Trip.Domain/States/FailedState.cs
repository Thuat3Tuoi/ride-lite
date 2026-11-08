using Trip.Domain.Enums;

namespace Trip.Domain.States;

public sealed class FailedState : TripState
{
    public override TripStatus Status => TripStatus.Failed;
}
