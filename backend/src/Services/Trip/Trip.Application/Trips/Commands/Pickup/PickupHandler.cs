using Microsoft.Extensions.Logging;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Domain.Repositories;

namespace Trip.Application.Trips.Commands.Pickup;

public sealed class PickupHandler : TripCommandHandlerBase, ICommandHandler<PickupCommand, TripResponse>
{
    private readonly IValidator<PickupCommand> _validator;
    private readonly ILogger<PickupHandler> _logger;

    public PickupHandler(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IValidator<PickupCommand> validator,
        ILogger<PickupHandler> logger)
        : base(reader, writer, unitOfWork, eventBus)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<TripResponse> HandleAsync(
        PickupCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationBehavior.Validate(_validator, command);

        var trip = await LoadTripAsync(command.TripId, cancellationToken);
        trip.Pickup();
        var response = await PersistAsync(trip, cancellationToken);

        LoggingBehavior.LogHandled(_logger, nameof(PickupCommand), trip.Id);

        return response;
    }
}
