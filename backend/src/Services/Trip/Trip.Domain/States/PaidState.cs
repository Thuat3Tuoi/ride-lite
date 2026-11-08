using Trip.Domain.Enums;

namespace Trip.Domain.States;

public sealed class PaidState : TripState
{
    public override TripStatus Status => TripStatus.Paid;
}
