using Microsoft.Extensions.Logging;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Domain.Repositories;

namespace Trip.Application.Trips.Commands.DriverReject;

public sealed class DriverRejectHandler : TripCommandHandlerBase, ICommandHandler<DriverRejectCommand, TripResponse>
{
    private readonly IValidator<DriverRejectCommand> _validator;
    private readonly ILogger<DriverRejectHandler> _logger;

    public DriverRejectHandler(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IValidator<DriverRejectCommand> validator,
        ILogger<DriverRejectHandler> logger)
        : base(reader, writer, unitOfWork, eventBus)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<TripResponse> HandleAsync(
        DriverRejectCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationBehavior.Validate(_validator, command);

        var trip = await LoadTripAsync(command.TripId, cancellationToken);
        trip.DriverReject(command.DriverId);
        var response = await PersistAsync(trip, cancellationToken);

        LoggingBehavior.LogHandled(_logger, nameof(DriverRejectCommand), trip.Id);

        return response;
    }
}
