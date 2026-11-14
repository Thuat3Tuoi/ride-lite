using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

public abstract class TripState
{
    public static readonly TripState SearchingDriver = new SearchingDriverTripState();
    public static readonly TripState DriverAccepted = new DriverAcceptedTripState();
    public static readonly TripState DriverArrived = new DriverArrivedTripState();
    public static readonly TripState InProgress = new InProgressTripState();
    public static readonly TripState Completed = new CompletedTripState();
    public static readonly TripState Cancelled = new CancelledTripState();
    public static readonly TripState NoDriverFound = new NoDriverFoundTripState();

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

    public static TripState FromStatus(TripStatus status) => status switch
    {
        TripStatus.SEARCHING_DRIVER => SearchingDriver,
        TripStatus.DRIVER_ACCEPTED => DriverAccepted,
        TripStatus.DRIVER_ARRIVED => DriverArrived,
        TripStatus.IN_PROGRESS => InProgress,
        TripStatus.COMPLETED => Completed,
        TripStatus.CANCELLED => Cancelled,
        TripStatus.NO_DRIVER_FOUND => NoDriverFound,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, $"Unsupported trip status: {status}")
    };
}

