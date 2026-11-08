using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.PaymentMethod.Commands.DeactivatePaymentMethod;

public sealed class DeactivatePaymentMethodHandler
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public DeactivatePaymentMethodHandler(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<DeactivatePaymentMethodResult> HandleAsync(
        DeactivatePaymentMethodCommand command,
        CancellationToken cancellationToken = default)
    {
        var method = await _paymentMethodRepository.GetByIdAsync(
            command.Id,
            cancellationToken
        );

        if (method is null)
        {
            throw new PaymentMethodNotFoundException(command.Id);
        }

        method.Deactivate();

        await _paymentMethodRepository.SaveChangesAsync(cancellationToken);

        return new DeactivatePaymentMethodResult(method.Id, method.Active);
    }
}

