using Microsoft.Extensions.Logging;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Domain.Repositories;

namespace Trip.Application.Trips.Commands.DriverAccept;

public sealed class DriverAcceptHandler : TripCommandHandlerBase, ICommandHandler<DriverAcceptCommand, TripResponse>
{
    private readonly IValidator<DriverAcceptCommand> _validator;
    private readonly ILogger<DriverAcceptHandler> _logger;

    public DriverAcceptHandler(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IValidator<DriverAcceptCommand> validator,
        ILogger<DriverAcceptHandler> logger)
        : base(reader, writer, unitOfWork, eventBus)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<TripResponse> HandleAsync(
        DriverAcceptCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationBehavior.Validate(_validator, command);

        var trip = await LoadTripAsync(command.TripId, cancellationToken);
        trip.DriverAccept(command.DriverId);
        var response = await PersistAsync(trip, cancellationToken);

        LoggingBehavior.LogHandled(_logger, nameof(DriverAcceptCommand), trip.Id);

        return response;
    }
}
