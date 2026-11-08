using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.Refund.Commands.ProcessRefund;

public sealed class ProcessRefundHandler
{
    private readonly IRefundRepository _refundRepository;

    public ProcessRefundHandler(IRefundRepository refundRepository)
    {
        _refundRepository = refundRepository;
    }

    public async Task<ProcessRefundResult> HandleAsync(
        ProcessRefundCommand command,
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

        refund.MarkProcessing();

        await _refundRepository.SaveChangesAsync(cancellationToken);

        return new ProcessRefundResult(
            refund.Id,
            refund.Status.ToString(),
            refund.UpdatedAt
        );
    }
}

