using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Payment.Commands.CancelPayment;

public sealed class CancelPaymentHandler
{
    private readonly IPaymentRepository _paymentRepository;

    public CancelPaymentHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<CancelPaymentResult> HandleAsync(
        CancelPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(
            command.PaymentId,
            cancellationToken
        );

        if (payment is null)
        {
            throw new AppException(PaymentErrors.NotFound);
        }

        payment.Cancel();

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return new CancelPaymentResult(
            payment.Id,
            payment.Status.ToString(),
            payment.UpdatedAt
        );
    }
}
