using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

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
            throw new PaymentNotFoundException(command.PaymentId);
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

