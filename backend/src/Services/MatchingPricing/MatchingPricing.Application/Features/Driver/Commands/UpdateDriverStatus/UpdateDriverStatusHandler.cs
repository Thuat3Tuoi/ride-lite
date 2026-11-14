using BuildingBlocks.SharedKernel.Exceptions;
using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Driver.Commands.UpdateDriverStatus;

public sealed class UpdateDriverStatusHandler
{
    private readonly IDriverRepository _driverRepository;

    public UpdateDriverStatusHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<UpdateDriverStatusResult> HandleAsync(
        UpdateDriverStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var driver = await _driverRepository.GetByIdAsync(command.DriverId, cancellationToken);
        if (driver is null)
        {
            throw new AppException(MatchingPricingErrors.DriverNotFound);
        }

        driver.SetStatus(command.Status);
        await _driverRepository.SaveChangesAsync(cancellationToken);

        return new UpdateDriverStatusResult(
            driver.Id,
            driver.Status.ToString(),
            driver.UpdatedAt
        );
    }
}

