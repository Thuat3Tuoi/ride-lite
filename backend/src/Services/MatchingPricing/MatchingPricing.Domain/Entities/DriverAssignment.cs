using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Domain.Entities;

public sealed class DriverAssignment
{
    public long Id { get; private set; }
    public long TripId { get; private set; }
    public long DriverId { get; private set; }
    public AssignmentStatus Status { get; private set; }
    public DateTimeOffset OfferedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RespondedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public Driver? Driver { get; private set; }

    private DriverAssignment() { }

    public static DriverAssignment Create(
        long tripId,
        long driverId,
        TimeSpan? offerDuration = null)
    {
        if (tripId <= 0)
            throw new ArgumentException("Trip ID must be a positive value.");

        if (driverId <= 0)
            throw new ArgumentException("Driver ID must be a positive value.");

        var duration = offerDuration ?? TimeSpan.FromSeconds(30);
        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Offer duration must be greater than zero.");

        var now = DateTimeOffset.UtcNow;

        return new DriverAssignment
        {
            TripId = tripId,
            DriverId = driverId,
            Status = AssignmentStatus.OFFERED,
            OfferedAt = now,
            ExpiresAt = now.Add(duration),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Accept()
    {
        if (Status != AssignmentStatus.OFFERED)
            throw new InvalidOperationException($"Cannot accept assignment when status is '{Status}'.");

        var now = DateTimeOffset.UtcNow;
        if (now > ExpiresAt)
        {
            Status = AssignmentStatus.EXPIRED;
            UpdatedAt = now;
            throw new InvalidOperationException("Assignment offer has expired.");
        }

        Status = AssignmentStatus.ACCEPTED;
        RespondedAt = now;
        UpdatedAt = now;
    }

    public void Reject()
    {
        if (Status != AssignmentStatus.OFFERED)
            throw new InvalidOperationException($"Cannot reject assignment when status is '{Status}'.");

        var now = DateTimeOffset.UtcNow;
        Status = AssignmentStatus.REJECTED;
        RespondedAt = now;
        UpdatedAt = now;
    }

    public void Expire()
    {
        if (Status != AssignmentStatus.OFFERED)
            throw new InvalidOperationException($"Cannot expire assignment when status is '{Status}'.");

        Status = AssignmentStatus.EXPIRED;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status != AssignmentStatus.OFFERED)
            throw new InvalidOperationException($"Cannot cancel assignment when status is '{Status}'.");

        Status = AssignmentStatus.CANCELLED;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

