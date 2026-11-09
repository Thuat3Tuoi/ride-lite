using BuildingBlocks.SharedKernel.Exceptions;
using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Driver.Commands.CreateDriver;

public sealed class CreateDriverHandler
{
    private readonly IDriverRepository _driverRepository;

    public CreateDriverHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<CreateDriverResult> HandleAsync(
        CreateDriverCommand command,
        CancellationToken cancellationToken = default)
    {
        var normalizedPhone = command.PhoneNumber?.Trim() ?? string.Empty;

        var exists = await _driverRepository.ExistsByPhoneNumberAsync(normalizedPhone, cancellationToken);
        if (exists)
        {
            throw new AppException(MatchingPricingErrors.DriverAlreadyExists);
        }

        var driver = Domain.Entities.Driver.Create(
            command.FullName,
            normalizedPhone,
            command.DateOfBirth,
            command.Status
        );

        await _driverRepository.AddAsync(driver, cancellationToken);
        await _driverRepository.SaveChangesAsync(cancellationToken);

        return new CreateDriverResult(
            driver.Id,
            driver.FullName,
            driver.PhoneNumber,
            driver.DateOfBirth,
            driver.Status.ToString(),
            driver.CreatedAt,
            driver.UpdatedAt
        );
    }
}

