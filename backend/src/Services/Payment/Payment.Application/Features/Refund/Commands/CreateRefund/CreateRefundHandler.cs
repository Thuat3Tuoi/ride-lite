using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;
using Payment.Domain.Entities;

namespace Payment.Application.Features.Refund.Commands.CreateRefund;

public sealed class CreateRefundHandler
{
    private readonly IRefundRepository _refundRepository;

    public CreateRefundHandler(IRefundRepository refundRepository)
    {
        _refundRepository = refundRepository;
    }

    public async Task<CreateRefundResult> HandleAsync(
        CreateRefundCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Tạo Domain Entity và kiểm tra dữ liệu
        var refund = Domain.Entities.Refund.Create(
            command.PaymentId,
            command.Amount,
            command.IdempotencyKey,
            command.Reason
        );

        // 2. Kiểm tra trùng idempotency key
        var exists = await _refundRepository.ExistsByIdempotencyKeyAsync(
            refund.IdempotencyKey,
            cancellationToken
        );

        if (exists)
        {
            throw new DuplicateIdempotencyKeyException(refund.IdempotencyKey);
        }

        // 3. Lưu Refund
        await _refundRepository.AddAsync(
            refund,
            cancellationToken
        );

        await _refundRepository.SaveChangesAsync(
            cancellationToken
        );

        // 4. Trả kết quả
        return new CreateRefundResult(
            refund.Id,
            refund.PaymentId,
            refund.Amount,
            refund.Status.ToString(),
            refund.IdempotencyKey,
            refund.Reason,
            refund.CreatedAt
        );
    }
}

