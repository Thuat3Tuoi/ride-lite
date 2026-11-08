using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.Payment.Queries.GetPaymentByTripId;

public sealed class GetPaymentByTripIdHandler
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentByTripIdHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<GetPaymentByTripIdResult> HandleAsync(
        GetPaymentByTripIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByTripIdAsync(
            query.TripId,
            cancellationToken
        );

        if (payment is null)
        {
            throw new PaymentNotFoundException(query.TripId);
        }

        return new GetPaymentByTripIdResult(
            payment.Id,
            payment.TripId,
            payment.PaymentOptionId,
            payment.ProviderId,
            payment.ProviderTransactionId,
            payment.GatewayToken,
            payment.Amount,
            payment.IdempotencyKey,
            payment.Status.ToString(),
            payment.PaidAt,
            payment.CreatedAt,
            payment.UpdatedAt
        );
    }
}

