using Trip.Domain.Enumerators;
using Trip.Domain.Shared;
using Trip.Domain.Entities.States;
using Trip.Domain.ValueObjects;

namespace Trip.Domain.Entities;

public class TripEntity : AggregateRoot<long>
{
    private TripEntity()
    {
        PickupAddress = string.Empty;
        DropoffAddress = string.Empty;
    }

    public TripEntity(
        long riderId,
        string pickupAddress,
        GeoPoint pickup,
        string dropoffAddress,
        GeoPoint dropoff)
    {
        if (riderId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(riderId), "Rider id must be positive.");
        }

        if (string.IsNullOrWhiteSpace(pickupAddress))
        {
            throw new ArgumentException("Pickup address is required.", nameof(pickupAddress));
        }

        if (string.IsNullOrWhiteSpace(dropoffAddress))
        {
            throw new ArgumentException("Dropoff address is required.", nameof(dropoffAddress));
        }

        RiderId = riderId;
        PickupAddress = pickupAddress;
        PickupLatitude = pickup.Latitude;
        PickupLongitude = pickup.Longitude;
        DropoffAddress = dropoffAddress;
        DropoffLatitude = dropoff.Latitude;
        DropoffLongitude = dropoff.Longitude;
        Status = TripStatus.Requested;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public long RiderId { get; private set; }

    public long? DriverId { get; private set; }

    public string PickupAddress { get; private set; }

    public decimal PickupLatitude { get; private set; }

    public decimal PickupLongitude { get; private set; }

    public string DropoffAddress { get; private set; }

    public decimal DropoffLatitude { get; private set; }

    public decimal DropoffLongitude { get; private set; }

    public decimal? Cost { get; private set; }

    public TripStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public DateTime? AcceptedAt { get; private set; }

    public DateTime? DriverArrivedAt { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public CancelledBy? CancelledBy { get; private set; }

    public string? CancelReason { get; private set; }

    public int DriverRejectCount { get; private set; }

    public GeoPoint PickupLocation => new(PickupLatitude, PickupLongitude);

    public GeoPoint DropoffLocation => new(DropoffLatitude, DropoffLongitude);

    private TripState State => TripState.For(Status);

    public void Quote(Money cost, long driverId)
    {
        if (driverId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(driverId), "Driver id must be positive.");
        }

        State.Quote(this, cost, driverId);
    }

    public void ConfirmQuote() => State.ConfirmQuote(this);

    public void RejectQuote(string? reason = null) => State.RejectQuote(this, reason);

    public void DriverAccept(long driverId)
    {
        if (driverId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(driverId), "Driver id must be positive.");
        }

        State.DriverAccept(this, driverId);
    }

    public void DriverReject(long driverId) => State.DriverReject(this, driverId);

    public void Arrive() => State.Arrive(this);

    public void Pickup() => State.Pickup(this);

    public void Dropoff() => State.Dropoff(this);

    public void PaymentSettled() => State.PaymentSettled(this);

    public void PaymentFailed() => State.PaymentFailed(this);

    public void Cancel(CancelledBy cancelledBy, string? reason = null) => State.Cancel(this, cancelledBy, reason);

    internal void TransitionTo(TripStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void Touch() => UpdatedAt = DateTime.UtcNow;

    internal void SetCost(Money cost) => Cost = cost.Amount;

    internal void SetDriver(long? driverId) => DriverId = driverId;

    internal void MarkAccepted() => AcceptedAt = DateTime.UtcNow;

    internal void MarkDriverArrived() => DriverArrivedAt = DateTime.UtcNow;

    internal void MarkStarted() => StartedAt = DateTime.UtcNow;

    internal void MarkCompleted() => CompletedAt = DateTime.UtcNow;

    internal void MarkCancelled(CancelledBy cancelledBy, string? reason)
    {
        CancelledAt = DateTime.UtcNow;
        CancelledBy = cancelledBy;
        CancelReason = reason;
    }

    internal void IncrementDriverRejectCount() => DriverRejectCount++;
}
