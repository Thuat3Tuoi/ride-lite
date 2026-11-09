using Trip.Domain.Enumerators;

namespace Trip.Domain.Entities.States;

public sealed class CancelledState : TripState
{
    public override TripStatus Status => TripStatus.Cancelled;
}
