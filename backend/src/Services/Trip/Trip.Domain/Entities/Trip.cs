using Trip.Domain.Enumerators;

namespace Trip.Domain.Entities;

public sealed class Trip
{
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
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void AcceptDriver(long driverId)
    {
        if (Status != TripStatus.SEARCHING_DRIVER)
            throw new InvalidOperationException($"Cannot accept driver when trip status is '{Status}'.");

        if (driverId <= 0)
            throw new ArgumentException("Driver ID must be a positive value.");

        DriverId = driverId;
        Status = TripStatus.DRIVER_ACCEPTED;
        AcceptedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void DriverArrive()
    {
        if (Status != TripStatus.DRIVER_ACCEPTED)
            throw new InvalidOperationException($"Cannot mark driver arrived when trip status is '{Status}'.");

        Status = TripStatus.DRIVER_ARRIVED;
        DriverArrivedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void StartTrip()
    {
        if (Status != TripStatus.DRIVER_ARRIVED)
            throw new InvalidOperationException($"Cannot start trip when trip status is '{Status}'.");

        Status = TripStatus.IN_PROGRESS;
        StartedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void CompleteTrip()
    {
        if (Status != TripStatus.IN_PROGRESS)
            throw new InvalidOperationException($"Cannot complete trip when trip status is '{Status}'.");

        Status = TripStatus.COMPLETED;
        CompletedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel(CancelledBy cancelledBy, string? reason = null)
    {
        if (Status is TripStatus.COMPLETED or TripStatus.CANCELLED or TripStatus.NO_DRIVER_FOUND)
            throw new InvalidOperationException($"Cannot cancel a trip when status is '{Status}'.");

        Status = TripStatus.CANCELLED;
        CancelledBy = cancelledBy;
        CancelReason = reason?.Trim();
        CancelledAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkNoDriverFound()
    {
        if (Status != TripStatus.SEARCHING_DRIVER)
            throw new InvalidOperationException($"Cannot mark no driver found when trip status is '{Status}'.");

        Status = TripStatus.NO_DRIVER_FOUND;
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

