using BuildingBlocks.SharedKernel.Exceptions;
using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Driver.Commands.UpdateDriverLocation;

public sealed class UpdateDriverLocationHandler
{
    private readonly IDriverRepository _driverRepository;

    public UpdateDriverLocationHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<UpdateDriverLocationResult> HandleAsync(
        UpdateDriverLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var driver = await _driverRepository.GetByIdAsync(command.DriverId, cancellationToken);
        if (driver is null)
        {
            throw new AppException(MatchingPricingErrors.DriverNotFound);
        }

        try
        {
            driver.UpdateLocation(command.Latitude, command.Longitude);
        }
        catch (ArgumentException)
        {
            throw new AppException(MatchingPricingErrors.InvalidCoordinates);
        }

        await _driverRepository.SaveChangesAsync(cancellationToken);

        return new UpdateDriverLocationResult(
            driver.Id,
            driver.Latitude!.Value,
            driver.Longitude!.Value,
            driver.Location!,
            driver.LocationUpdatedAt!.Value
        );
    }
}

