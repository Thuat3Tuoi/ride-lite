using Payment.Application.Abstractions.Data;

namespace Payment.Application.Features.Refund.Queries.GetRefundsByPaymentId;

public sealed class GetRefundsByPaymentIdHandler
{
    private readonly IRefundRepository _refundRepository;

    public GetRefundsByPaymentIdHandler(IRefundRepository refundRepository)
    {
        _refundRepository = refundRepository;
    }

    public async Task<GetRefundsByPaymentIdResult> HandleAsync(
        GetRefundsByPaymentIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var refunds = await _refundRepository.GetByPaymentIdAsync(
            query.PaymentId,
            cancellationToken
        );

        var items = refunds.Select(r => new RefundItemResult(
            r.Id,
            r.PaymentId,
            r.ProviderRefundId,
            r.Amount,
            r.Reason,
            r.Status.ToString(),
            r.IdempotencyKey,
            r.RefundedAt,
            r.CreatedAt
        )).ToList();

        return new GetRefundsByPaymentIdResult(items);
    }
}

