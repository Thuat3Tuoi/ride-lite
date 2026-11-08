using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.Refund.Queries.GetRefundById;

public sealed class GetRefundByIdHandler
{
    private readonly IRefundRepository _refundRepository;

    public GetRefundByIdHandler(IRefundRepository refundRepository)
    {
        _refundRepository = refundRepository;
    }

    public async Task<GetRefundByIdResult> HandleAsync(
        GetRefundByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var refund = await _refundRepository.GetByIdAsync(
            query.Id,
            cancellationToken
        );

        if (refund is null)
        {
            throw new RefundNotFoundException(query.Id);
        }

        return new GetRefundByIdResult(
            refund.Id,
            refund.PaymentId,
            refund.ProviderRefundId,
            refund.Amount,
            refund.Reason,
            refund.Status.ToString(),
            refund.IdempotencyKey,
            refund.RefundedAt,
            refund.CreatedAt,
            refund.UpdatedAt
        );
    }
}

