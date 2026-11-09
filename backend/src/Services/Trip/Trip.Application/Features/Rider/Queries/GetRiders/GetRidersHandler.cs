using Trip.Application.Abstractions.Data;
using Trip.Application.Features.Rider.Queries.GetRiderById;

namespace Trip.Application.Features.Rider.Queries.GetRiders;

public sealed class GetRidersHandler
{
    private readonly IRiderRepository _riderRepository;

    public GetRidersHandler(IRiderRepository riderRepository)
    {
        _riderRepository = riderRepository;
    }

    public async Task<GetRidersResult> HandleAsync(
        GetRidersQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;

        var riders = await _riderRepository.GetAllAsync(page, pageSize, cancellationToken);

        var items = riders.Select(r => new GetRiderByIdResult(
            r.Id,
            r.FullName,
            r.PhoneNumber,
            r.DateOfBirth,
            r.CreatedAt,
            r.UpdatedAt
        )).ToList();

        return new GetRidersResult(items, page, pageSize);
    }
}

