using BuildingBlocks.SharedKernel.Exceptions;
using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Driver.Queries.GetDriverById;

public sealed class GetDriverByIdHandler
{
    private readonly IDriverRepository _driverRepository;

    public GetDriverByIdHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<GetDriverByIdResult> HandleAsync(
        GetDriverByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var driver = await _driverRepository.GetByIdAsync(query.Id, cancellationToken);
        if (driver is null)
        {
            throw new AppException(MatchingPricingErrors.DriverNotFound);
        }

        return new GetDriverByIdResult(
            driver.Id,
            driver.FullName,
            driver.PhoneNumber,
            driver.DateOfBirth,
            driver.Latitude,
            driver.Longitude,
            driver.Location,
            driver.Status.ToString(),
            driver.LocationUpdatedAt,
            driver.CreatedAt,
            driver.UpdatedAt
        );
    }
}

