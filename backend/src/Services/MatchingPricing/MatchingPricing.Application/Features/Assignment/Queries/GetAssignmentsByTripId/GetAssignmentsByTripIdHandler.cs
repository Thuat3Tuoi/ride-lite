using MatchingPricing.Application.Abstractions.Data;

namespace MatchingPricing.Application.Features.Assignment.Queries.GetAssignmentsByTripId;

public sealed class GetAssignmentsByTripIdHandler
{
    private readonly IDriverAssignmentRepository _assignmentRepository;

    public GetAssignmentsByTripIdHandler(IDriverAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<GetAssignmentsByTripIdResult> HandleAsync(
        GetAssignmentsByTripIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var assignments = await _assignmentRepository.GetByTripIdAsync(query.TripId, cancellationToken);

        var dtos = assignments.Select(a => new AssignmentDto(
            a.Id,
            a.TripId,
            a.DriverId,
            a.Status.ToString(),
            a.OfferedAt,
            a.ExpiresAt,
            a.RespondedAt,
            a.CreatedAt,
            a.UpdatedAt
        )).ToList();

        return new GetAssignmentsByTripIdResult(dtos);
    }
}

