using Microsoft.Extensions.Logging;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Domain.Repositories;

namespace Trip.Application.Trips.Commands.RejectQuote;

public sealed class RejectQuoteHandler : TripCommandHandlerBase, ICommandHandler<RejectQuoteCommand, TripResponse>
{
    private readonly IValidator<RejectQuoteCommand> _validator;
    private readonly ILogger<RejectQuoteHandler> _logger;

    public RejectQuoteHandler(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IValidator<RejectQuoteCommand> validator,
        ILogger<RejectQuoteHandler> logger)
        : base(reader, writer, unitOfWork, eventBus)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<TripResponse> HandleAsync(
        RejectQuoteCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationBehavior.Validate(_validator, command);

        var trip = await LoadTripAsync(command.TripId, cancellationToken);
        trip.RejectQuote(command.Reason);
        var response = await PersistAsync(trip, cancellationToken);

        LoggingBehavior.LogHandled(_logger, nameof(RejectQuoteCommand), trip.Id);

        return response;
    }
}
