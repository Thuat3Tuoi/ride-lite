using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentProvider.Commands.DeactivatePaymentProvider;

public sealed class DeactivatePaymentProviderHandler
{
    private readonly IPaymentProviderRepository _paymentProviderRepository;

    public DeactivatePaymentProviderHandler(IPaymentProviderRepository paymentProviderRepository)
    {
        _paymentProviderRepository = paymentProviderRepository;
    }

    public async Task<DeactivatePaymentProviderResult> HandleAsync(
        DeactivatePaymentProviderCommand command,
        CancellationToken cancellationToken = default)
    {
        var provider = await _paymentProviderRepository.GetByIdAsync(
            command.Id,
            cancellationToken
        );

        if (provider is null)
        {
            throw new AppException(PaymentErrors.PaymentProviderNotFound);
        }

        provider.Deactivate();

        await _paymentProviderRepository.SaveChangesAsync(cancellationToken);

        return new DeactivatePaymentProviderResult(provider.Id, provider.Active);
    }
}
