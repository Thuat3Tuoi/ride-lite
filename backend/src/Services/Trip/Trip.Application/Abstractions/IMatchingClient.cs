using Trip.Domain.ValueObjects;

namespace Trip.Application.Abstractions;

public sealed record MatchingRequest(long TripId, GeoPoint Pickup, GeoPoint Dropoff);

public sealed record MatchingResult(decimal Fare, long DriverId, int EtaSeconds);

public interface IMatchingClient
{
    Task<MatchingResult> MatchAsync(MatchingRequest request, CancellationToken cancellationToken = default);
}
