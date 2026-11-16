using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

public abstract class TripState
{
    public static TripState SearchingDriver => TripStateFactory.Instance.SearchingDriver;
    public static TripState DriverAccepted => TripStateFactory.Instance.DriverAccepted;
    public static TripState DriverArrived => TripStateFactory.Instance.DriverArrived;
    public static TripState InProgress => TripStateFactory.Instance.InProgress;
    public static TripState Completed => TripStateFactory.Instance.Completed;
    public static TripState Cancelled => TripStateFactory.Instance.Cancelled;
    public static TripState NoDriverFound => TripStateFactory.Instance.NoDriverFound;

    public abstract TripStatus Status { get; }

    public virtual void AcceptDriver(Entities.Trip trip, long driverId)
    {
        throw new InvalidOperationException($"Cannot accept driver when trip status is '{trip.Status}'.");
    }

    public virtual void DriverArrive(Entities.Trip trip)
    {
        throw new InvalidOperationException($"Cannot mark driver arrived when trip status is '{trip.Status}'.");
    }

    public virtual void StartTrip(Entities.Trip trip)
    {
        throw new InvalidOperationException($"Cannot start trip when trip status is '{trip.Status}'.");
    }

    public virtual void CompleteTrip(Entities.Trip trip)
    {
        throw new InvalidOperationException($"Cannot complete trip when trip status is '{trip.Status}'.");
    }

    public virtual void Cancel(Entities.Trip trip, CancelledBy cancelledBy, string? reason)
    {
        throw new InvalidOperationException($"Cannot cancel a trip when status is '{trip.Status}'.");
    }

    public virtual void MarkNoDriverFound(Entities.Trip trip)
    {
        throw new InvalidOperationException($"Cannot mark no driver found when trip status is '{trip.Status}'.");
    }

    public static TripState FromStatus(TripStatus status) => TripStateFactory.Instance.GetState(status);
}

