using Trip.Domain.Enumerators;
using Trip.Domain.States;

namespace Trip.Domain.Entities;

public sealed class Trip
{
    private TripState? _state;

    public long Id { get; private set; }
    public long? DriverId { get; private set; }
    public long RiderId { get; private set; }
    public string PickupAddress { get; private set; } = string.Empty;
    public decimal PickupLatitude { get; private set; }
    public decimal PickupLongitude { get; private set; }
    public string DropoffAddress { get; private set; } = string.Empty;
    public decimal DropoffLatitude { get; private set; }
    public decimal DropoffLongitude { get; private set; }
    public decimal Cost { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? AcceptedAt { get; private set; }
    public DateTimeOffset? DriverArrivedAt { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public CancelledBy? CancelledBy { get; private set; }
    public string? CancelReason { get; private set; }
    public TripStatus Status { get; private set; }

    public TripState State => _state ??= TripStateFactory.Instance.GetState(Status);

    public Rider? Rider { get; private set; }

    private Trip() { }

    public static Trip Create(
        long riderId,
        string pickupAddress,
        decimal pickupLatitude,
        decimal pickupLongitude,
        string dropoffAddress,
        decimal dropoffLatitude,
        decimal dropoffLongitude,
        decimal cost)
    {
        if (riderId <= 0)
            throw new ArgumentException("Rider ID must be a positive value.");

        ValidateAddress(pickupAddress, "Pickup");
        ValidateAddress(dropoffAddress, "Dropoff");
        ValidateCoordinates(pickupLatitude, pickupLongitude, "Pickup");
        ValidateCoordinates(dropoffLatitude, dropoffLongitude, "Dropoff");

        if (cost < 0)
            throw new ArgumentException("Trip cost must be non-negative.");

        var now = DateTimeOffset.UtcNow;

        return new Trip
        {
            RiderId = riderId,
            PickupAddress = pickupAddress.Trim(),
            PickupLatitude = pickupLatitude,
            PickupLongitude = pickupLongitude,
            DropoffAddress = dropoffAddress.Trim(),
            DropoffLatitude = dropoffLatitude,
            DropoffLongitude = dropoffLongitude,
            Cost = cost,
            Status = TripStatus.SEARCHING_DRIVER,
            _state = TripStateFactory.Instance.SearchingDriver,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void AcceptDriver(long driverId)
    {
        State.AcceptDriver(this, driverId);
    }

    public void DriverArrive()
    {
        State.DriverArrive(this);
    }

    public void StartTrip()
    {
        State.StartTrip(this);
    }

    public void CompleteTrip()
    {
        State.CompleteTrip(this);
    }

    public void Cancel(CancelledBy cancelledBy, string? reason = null)
    {
        State.Cancel(this, cancelledBy, reason);
    }

    public void MarkNoDriverFound()
    {
        State.MarkNoDriverFound(this);
    }

    internal void SetDriver(long driverId)
    {
        DriverId = driverId;
    }

    internal void SetAcceptedAt(DateTimeOffset acceptedAt)
    {
        AcceptedAt = acceptedAt;
    }

    internal void SetDriverArrivedAt(DateTimeOffset driverArrivedAt)
    {
        DriverArrivedAt = driverArrivedAt;
    }

    internal void SetStartedAt(DateTimeOffset startedAt)
    {
        StartedAt = startedAt;
    }

    internal void SetCompletedAt(DateTimeOffset completedAt)
    {
        CompletedAt = completedAt;
    }

    internal void SetCancellation(CancelledBy cancelledBy, string? reason, DateTimeOffset cancelledAt)
    {
        CancelledBy = cancelledBy;
        CancelReason = reason?.Trim();
        CancelledAt = cancelledAt;
    }

    internal void TransitionTo(TripState newState)
    {
        _state = newState;
        Status = newState.Status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void ValidateAddress(string address, string type)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException($"{type} address cannot be empty.");

        if (address.Trim().Length > 500)
            throw new ArgumentException($"{type} address cannot exceed 500 characters.");
    }

    private static void ValidateCoordinates(decimal latitude, decimal longitude, string type)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentException($"{type} latitude must be between -90 and 90.");

        if (longitude is < -180 or > 180)
            throw new ArgumentException($"{type} longitude must be between -180 and 180.");
    }
}
