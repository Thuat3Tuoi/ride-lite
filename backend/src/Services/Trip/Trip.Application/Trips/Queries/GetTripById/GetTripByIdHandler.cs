using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Application.Exceptions;
using Trip.Domain.Repositories;

namespace Trip.Application.Trips.Queries.GetTripById;

public sealed class GetTripByIdHandler : IQueryHandler<GetTripByIdQuery, TripResponse>
{
    private readonly ITripReader _reader;
    private readonly IValidator<GetTripByIdQuery> _validator;

    public GetTripByIdHandler(ITripReader reader, IValidator<GetTripByIdQuery> validator)
    {
        _reader = reader;
        _validator = validator;
    }

    public async Task<TripResponse> HandleAsync(
        GetTripByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ValidationBehavior.Validate(_validator, query);

        var trip = await _reader.GetByIdAsync(query.TripId, cancellationToken)
            ?? throw new TripNotFoundException(query.TripId);

        return TripResponse.From(trip);
    }
}
