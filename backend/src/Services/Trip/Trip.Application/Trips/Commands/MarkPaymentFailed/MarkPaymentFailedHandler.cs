using Microsoft.Extensions.Logging;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Services;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Domain.Constants;
using Trip.Domain.Repositories;

namespace Trip.Application.Trips.Commands.MarkPaymentFailed;

public sealed class MarkPaymentFailedHandler : TripCommandHandlerBase, ICommandHandler<MarkPaymentFailedCommand, TripResponse>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly IValidator<MarkPaymentFailedCommand> _validator;
    private readonly ILogger<MarkPaymentFailedHandler> _logger;

    public MarkPaymentFailedHandler(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IPaymentGateway paymentGateway,
        IValidator<MarkPaymentFailedCommand> validator,
        ILogger<MarkPaymentFailedHandler> logger)
        : base(reader, writer, unitOfWork, eventBus)
    {
        _paymentGateway = paymentGateway;
        _validator = validator;
        _logger = logger;
    }

    public async Task<TripResponse> HandleAsync(
        MarkPaymentFailedCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationBehavior.Validate(_validator, command);

        var trip = await LoadTripAsync(command.TripId, cancellationToken);
        trip.PaymentFailed();
        var response = await PersistAsync(trip, cancellationToken);

        if (trip.Cost is { } amount)
        {
            await _paymentGateway.RequestPaymentAsync(
                new PaymentRequest(
                    trip.Id,
                    trip.RiderId,
                    amount,
                    TripRules.PaymentIdempotencyKey(trip.Id)),
                cancellationToken);
        }

        LoggingBehavior.LogHandled(_logger, nameof(MarkPaymentFailedCommand), trip.Id);

        return response;
    }
}
