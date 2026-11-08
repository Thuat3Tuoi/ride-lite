using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.Payment.Commands.ProcessPayment;

public sealed class ProcessPaymentHandler
{
    private readonly IPaymentRepository _paymentRepository;

    public ProcessPaymentHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<ProcessPaymentResult> HandleAsync(
        ProcessPaymentCommand command,
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

        payment.MarkProcessing();

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return new ProcessPaymentResult(
            payment.Id,
            payment.Status.ToString(),
            payment.UpdatedAt
        );
    }
}

