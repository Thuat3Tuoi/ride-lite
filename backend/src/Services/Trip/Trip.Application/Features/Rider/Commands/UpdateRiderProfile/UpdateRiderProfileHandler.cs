using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Rider.Commands.UpdateRiderProfile;

public sealed class UpdateRiderProfileHandler
{
    private readonly IRiderRepository _riderRepository;

    public UpdateRiderProfileHandler(IRiderRepository riderRepository)
    {
        _riderRepository = riderRepository;
    }

    public async Task<UpdateRiderProfileResult> HandleAsync(
        UpdateRiderProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var rider = await _riderRepository.GetByIdAsync(command.Id, cancellationToken);
        if (rider is null)
        {
            throw new AppException(TripErrors.RiderNotFound);
        }

        var normalizedPhone = command.PhoneNumber?.Trim() ?? string.Empty;

        if (!string.Equals(rider.PhoneNumber, normalizedPhone, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _riderRepository.ExistsByPhoneNumberAsync(normalizedPhone, cancellationToken);
            if (exists)
            {
                throw new AppException(TripErrors.RiderAlreadyExists);
            }
        }

        rider.UpdateProfile(command.FullName, normalizedPhone, command.DateOfBirth);

        await _riderRepository.SaveChangesAsync(cancellationToken);

        return new UpdateRiderProfileResult(
            rider.Id,
            rider.FullName,
            rider.PhoneNumber,
            rider.DateOfBirth,
            rider.UpdatedAt
        );
    }
}

