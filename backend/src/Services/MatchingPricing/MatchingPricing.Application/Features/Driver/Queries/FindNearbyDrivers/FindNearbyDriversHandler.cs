using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Application.Features.Driver.Queries.GetDriverById;

namespace MatchingPricing.Application.Features.Driver.Queries.FindNearbyDrivers;

public sealed class FindNearbyDriversHandler
{
    private readonly IDriverRepository _driverRepository;

    public FindNearbyDriversHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<FindNearbyDriversResult> HandleAsync(
        FindNearbyDriversQuery query,
        CancellationToken cancellationToken = default)
    {
        var radius = query.RadiusKm <= 0 ? 10.0 : query.RadiusKm;
        var limit = query.Limit is < 1 or > 50 ? 10 : query.Limit;

        var availableDrivers = await _driverRepository.GetAvailableNearbyDriversAsync(
            query.PickupLatitude,
            query.PickupLongitude,
            radius,
            limit,
            cancellationToken
        );

        var items = availableDrivers.Select(d =>
        {
            var dist = d.Latitude.HasValue && d.Longitude.HasValue
                ? CalculateDistance(
                    (double)query.PickupLatitude, (double)query.PickupLongitude,
                    (double)d.Latitude.Value, (double)d.Longitude.Value)
                : 0.0;

            var dto = new GetDriverByIdResult(
                d.Id,
                d.FullName,
                d.PhoneNumber,
                d.DateOfBirth,
                d.Latitude,
                d.Longitude,
                d.Location,
                d.Status.ToString(),
                d.LocationUpdatedAt,
                d.CreatedAt,
                d.UpdatedAt
            );

            return new NearbyDriverItem(dto, Math.Round(dist, 2));
        }).OrderBy(x => x.DistanceKm).ToList();

        return new FindNearbyDriversResult(items);
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}

