using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.Payment.Queries.GetPaymentById;

public sealed class GetPaymentByIdHandler
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentByIdHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<GetPaymentByIdResult> HandleAsync(
        GetPaymentByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(
            query.Id,
            cancellationToken
        );

        if (payment is null)
        {
            throw new PaymentNotFoundException(query.Id);
        }

        return new GetPaymentByIdResult(
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

