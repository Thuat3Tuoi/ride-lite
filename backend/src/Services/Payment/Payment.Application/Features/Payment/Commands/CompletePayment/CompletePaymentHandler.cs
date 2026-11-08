using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Payment.Commands.CompletePayment;

public sealed class CompletePaymentHandler
{
    private readonly IPaymentRepository _paymentRepository;

    public CompletePaymentHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<CompletePaymentResult> HandleAsync(
        CompletePaymentCommand command,
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

        payment.MarkSucceeded(
            command.ProviderTransactionId,
            command.GatewayToken,
            command.ResolvedProviderId
        );

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return new CompletePaymentResult(
            payment.Id,
            payment.Status.ToString(),
            payment.ProviderTransactionId!,
            payment.GatewayToken,
            payment.PaidAt,
            payment.UpdatedAt
        );
    }
}
