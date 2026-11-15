using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Application.Abstractions.Messaging;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentMethod.Commands.DeactivatePaymentMethod;

public sealed class DeactivatePaymentMethodHandler : ICommandHandler<DeactivatePaymentMethodCommand, DeactivatePaymentMethodResult>
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
            throw new AppException(PaymentErrors.PaymentMethodNotFound);
        }

        method.Deactivate();

        await _paymentMethodRepository.SaveChangesAsync(cancellationToken);

        return new DeactivatePaymentMethodResult(method.Id, method.Active);
    }
}
