using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

/// <summary>
/// Factory pattern implementation for managing and instantiating TripState instances.
/// Utilizes the Flyweight pattern internally to cache stateless State singletons.
/// </summary>
public sealed class TripStateFactory : ITripStateFactory
{
    private static readonly Lazy<TripStateFactory> _lazyInstance = new(() => new TripStateFactory());

    /// <summary>
    /// Global singleton instance of the factory.
    /// </summary>
    public static TripStateFactory Instance => _lazyInstance.Value;

    private readonly SearchingDriverTripState _searchingDriver;
    private readonly DriverAcceptedTripState _driverAccepted;
    private readonly DriverArrivedTripState _driverArrived;
    private readonly InProgressTripState _inProgress;
    private readonly CompletedTripState _completed;
    private readonly CancelledTripState _cancelled;
    private readonly NoDriverFoundTripState _noDriverFound;

    public TripStateFactory()
    {
        _searchingDriver = new SearchingDriverTripState();
        _driverAccepted = new DriverAcceptedTripState();
        _driverArrived = new DriverArrivedTripState();
        _inProgress = new InProgressTripState();
        _completed = new CompletedTripState();
        _cancelled = new CancelledTripState();
        _noDriverFound = new NoDriverFoundTripState();
    }

    public TripState SearchingDriver => _searchingDriver;
    public TripState DriverAccepted => _driverAccepted;
    public TripState DriverArrived => _driverArrived;
    public TripState InProgress => _inProgress;
    public TripState Completed => _completed;
    public TripState Cancelled => _cancelled;
    public TripState NoDriverFound => _noDriverFound;

    /// <summary>
    /// Creates or resolves the corresponding TripState for the given TripStatus.
    /// </summary>
    public TripState GetState(TripStatus status) => status switch
    {
        TripStatus.SEARCHING_DRIVER => _searchingDriver,
        TripStatus.DRIVER_ACCEPTED => _driverAccepted,
        TripStatus.DRIVER_ARRIVED => _driverArrived,
        TripStatus.IN_PROGRESS => _inProgress,
        TripStatus.COMPLETED => _completed,
        TripStatus.CANCELLED => _cancelled,
        TripStatus.NO_DRIVER_FOUND => _noDriverFound,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, $"Unsupported trip status: {status}")
    };
}

