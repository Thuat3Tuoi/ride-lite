using Trip.Domain.Enumerators;

namespace Trip.Domain.Entities.States;

public sealed class PaidState : TripState
{
    public override TripStatus Status => TripStatus.Paid;
}
