using Microsoft.Extensions.Logging;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Domain.Repositories;

namespace Trip.Application.Trips.Commands.Arrive;

public sealed class ArriveHandler : TripCommandHandlerBase, ICommandHandler<ArriveCommand, TripResponse>
{
    private readonly IValidator<ArriveCommand> _validator;
    private readonly ILogger<ArriveHandler> _logger;

    public ArriveHandler(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IValidator<ArriveCommand> validator,
        ILogger<ArriveHandler> logger)
        : base(reader, writer, unitOfWork, eventBus)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<TripResponse> HandleAsync(
        ArriveCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationBehavior.Validate(_validator, command);

        var trip = await LoadTripAsync(command.TripId, cancellationToken);
        trip.Arrive();
        var response = await PersistAsync(trip, cancellationToken);

        LoggingBehavior.LogHandled(_logger, nameof(ArriveCommand), trip.Id);

        return response;
    }
}
