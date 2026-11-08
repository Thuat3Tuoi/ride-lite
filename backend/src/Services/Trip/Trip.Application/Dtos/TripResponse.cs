using Trip.Domain.Entities;

namespace Trip.Application.Dtos;

public sealed record TripResponse(
    long Id,
    long RiderId,
    long? DriverId,
    string PickupAddress,
    decimal PickupLatitude,
    decimal PickupLongitude,
    string DropoffAddress,
    decimal DropoffLatitude,
    decimal DropoffLongitude,
    decimal? Cost,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? AcceptedAt,
    DateTime? DriverArrivedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime? CancelledAt,
    string? CancelledBy,
    string? CancelReason,
    int DriverRejectCount)
{
    public static TripResponse From(TripEntity trip) =>
        new(
            trip.Id,
            trip.RiderId,
            trip.DriverId,
            trip.PickupAddress,
            trip.PickupLatitude,
            trip.PickupLongitude,
            trip.DropoffAddress,
            trip.DropoffLatitude,
            trip.DropoffLongitude,
            trip.Cost,
            trip.Status.ToString(),
            trip.CreatedAt,
            trip.UpdatedAt,
            trip.AcceptedAt,
            trip.DriverArrivedAt,
            trip.StartedAt,
            trip.CompletedAt,
            trip.CancelledAt,
            trip.CancelledBy?.ToString(),
            trip.CancelReason,
            trip.DriverRejectCount);
}
