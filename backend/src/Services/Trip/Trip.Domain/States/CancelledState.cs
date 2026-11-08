using Trip.Domain.Enums;

namespace Trip.Domain.States;

public sealed class CancelledState : TripState
{
    public override TripStatus Status => TripStatus.Cancelled;
}
