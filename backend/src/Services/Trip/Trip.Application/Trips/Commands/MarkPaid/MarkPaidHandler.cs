using Microsoft.Extensions.Logging;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Domain.Repositories;

namespace Trip.Application.Trips.Commands.MarkPaid;

public sealed class MarkPaidHandler : TripCommandHandlerBase, ICommandHandler<MarkPaidCommand, TripResponse>
{
    private readonly IValidator<MarkPaidCommand> _validator;
    private readonly ILogger<MarkPaidHandler> _logger;

    public MarkPaidHandler(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IValidator<MarkPaidCommand> validator,
        ILogger<MarkPaidHandler> logger)
        : base(reader, writer, unitOfWork, eventBus)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<TripResponse> HandleAsync(
        MarkPaidCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationBehavior.Validate(_validator, command);

        var trip = await LoadTripAsync(command.TripId, cancellationToken);
        trip.PaymentSettled();
        var response = await PersistAsync(trip, cancellationToken);

        LoggingBehavior.LogHandled(_logger, nameof(MarkPaidCommand), trip.Id);

        return response;
    }
}
