using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Errors;
using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Application.Features.Payment.Commands.CreatePayment;

public sealed class CreatePaymentHandler
{
    private readonly IPaymentRepository _paymentRepository;

    public CreatePaymentHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<CreatePaymentResult> HandleAsync(
        CreatePaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Tạo Domain Entity và kiểm tra dữ liệu
        var payment = PaymentEntity.Create(
            command.TripId,
            command.PaymentOptionId,
            command.Amount,
            command.IdempotencyKey,
            command.ProviderId
        );

        // 2. Kiểm tra trùng idempotency key (nếu có)
        if (!string.IsNullOrWhiteSpace(payment.IdempotencyKey))
        {
            var exists = await _paymentRepository.ExistsByIdempotencyKeyAsync(
                payment.IdempotencyKey,
                cancellationToken
            );

            if (exists)
            {
                throw new AppException(PaymentErrors.DuplicateIdempotencyKey);
            }
        }

        // 3. Lưu Payment
        await _paymentRepository.AddAsync(
            payment,
            cancellationToken
        );

        await _paymentRepository.SaveChangesAsync(
            cancellationToken
        );

        // 4. Trả kết quả
        return new CreatePaymentResult(
            payment.Id,
            payment.TripId,
            payment.PaymentOptionId,
            payment.Amount,
            payment.Status.ToString(),
            payment.IdempotencyKey,
            payment.CreatedAt
        );
    }
}
