using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentMethod.Commands.ActivatePaymentMethod;

public sealed class ActivatePaymentMethodHandler
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public ActivatePaymentMethodHandler(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<ActivatePaymentMethodResult> HandleAsync(
        ActivatePaymentMethodCommand command,
        CancellationToken cancellationToken = default)
    {
        var method = await _paymentMethodRepository.GetByIdAsync(
            command.Id,
            cancellationToken
        );

        if (method is null)
        {
            throw new AppException(PaymentErrors.PaymentMethodNotFound);
        }

        method.Activate();

        await _paymentMethodRepository.SaveChangesAsync(cancellationToken);

        return new ActivatePaymentMethodResult(method.Id, method.Active);
    }
}
