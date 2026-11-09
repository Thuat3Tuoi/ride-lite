using Microsoft.Extensions.Logging;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Domain.Repositories;

namespace Trip.Application.Trips.Commands.ConfirmQuote;

public sealed class ConfirmQuoteHandler : TripCommandHandlerBase, ICommandHandler<ConfirmQuoteCommand, TripResponse>
{
    private readonly IValidator<ConfirmQuoteCommand> _validator;
    private readonly ILogger<ConfirmQuoteHandler> _logger;

    public ConfirmQuoteHandler(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IValidator<ConfirmQuoteCommand> validator,
        ILogger<ConfirmQuoteHandler> logger)
        : base(reader, writer, unitOfWork, eventBus)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<TripResponse> HandleAsync(
        ConfirmQuoteCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationBehavior.Validate(_validator, command);

        var trip = await LoadTripAsync(command.TripId, cancellationToken);
        trip.ConfirmQuote();
        var response = await PersistAsync(trip, cancellationToken);

        LoggingBehavior.LogHandled(_logger, nameof(ConfirmQuoteCommand), trip.Id);

        return response;
    }
}
