using BuildingBlocks.SharedKernel.Exceptions;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Errors;

namespace Trip.Application.Features.Rider.Queries.GetRiderById;

public sealed class GetRiderByIdHandler
{
    private readonly IRiderRepository _riderRepository;

    public GetRiderByIdHandler(IRiderRepository riderRepository)
    {
        _riderRepository = riderRepository;
    }

    public async Task<GetRiderByIdResult> HandleAsync(
        GetRiderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var rider = await _riderRepository.GetByIdAsync(query.Id, cancellationToken);
        if (rider is null)
        {
            throw new AppException(TripErrors.RiderNotFound);
        }

        return new GetRiderByIdResult(
            rider.Id,
            rider.FullName,
            rider.PhoneNumber,
            rider.DateOfBirth,
            rider.CreatedAt,
            rider.UpdatedAt
        );
    }
}

