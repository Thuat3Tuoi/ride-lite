using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Entities;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Rider.Commands.CreateRider;

public sealed class CreateRiderHandler
{
    private readonly IRiderRepository _riderRepository;

    public CreateRiderHandler(IRiderRepository riderRepository)
    {
        _riderRepository = riderRepository;
    }

    public async Task<CreateRiderResult> HandleAsync(
        CreateRiderCommand command,
        CancellationToken cancellationToken = default)
    {
        var normalizedPhone = command.PhoneNumber?.Trim() ?? string.Empty;

        var exists = await _riderRepository.ExistsByPhoneNumberAsync(normalizedPhone, cancellationToken);
        if (exists)
        {
            throw new AppException(TripErrors.RiderAlreadyExists);
        }

        var rider = Domain.Entities.Rider.Create(
            command.FullName,
            normalizedPhone,
            command.DateOfBirth
        );

        await _riderRepository.AddAsync(rider, cancellationToken);
        await _riderRepository.SaveChangesAsync(cancellationToken);

        return new CreateRiderResult(
            rider.Id,
            rider.FullName,
            rider.PhoneNumber,
            rider.DateOfBirth,
            rider.CreatedAt,
            rider.UpdatedAt
        );
    }
}

