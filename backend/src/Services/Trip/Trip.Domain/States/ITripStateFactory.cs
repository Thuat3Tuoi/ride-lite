using Trip.Domain.Enumerators;

namespace Trip.Domain.States;

/// <summary>
/// Factory interface for managing and creating TripState instances.
/// Implements the GoF Factory Pattern for Trip state lifecycle.
/// </summary>
public interface ITripStateFactory
{
    TripState GetState(TripStatus status);
    TripState SearchingDriver { get; }
    TripState DriverAccepted { get; }
    TripState DriverArrived { get; }
    TripState InProgress { get; }
    TripState Completed { get; }
    TripState Cancelled { get; }
    TripState NoDriverFound { get; }
}

