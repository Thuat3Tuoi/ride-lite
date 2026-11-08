using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.Refund.Commands.CompleteRefund;

public sealed class CompleteRefundHandler
{
    private readonly IRefundRepository _refundRepository;

    public CompleteRefundHandler(IRefundRepository refundRepository)
    {
        _refundRepository = refundRepository;
    }

    public async Task<CompleteRefundResult> HandleAsync(
        CompleteRefundCommand command,
        CancellationToken cancellationToken = default)
    {
        var refund = await _refundRepository.GetByIdAsync(
            command.RefundId,
            cancellationToken
        );

        if (refund is null)
        {
            throw new RefundNotFoundException(command.RefundId);
        }

        refund.MarkSucceeded(command.ProviderRefundId);

        await _refundRepository.SaveChangesAsync(cancellationToken);

        return new CompleteRefundResult(
            refund.Id,
            refund.Status.ToString(),
            refund.ProviderRefundId!,
            refund.RefundedAt,
            refund.UpdatedAt
        );
    }
}

