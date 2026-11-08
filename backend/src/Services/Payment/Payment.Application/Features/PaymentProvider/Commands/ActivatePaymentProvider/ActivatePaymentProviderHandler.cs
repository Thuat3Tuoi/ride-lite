using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.PaymentProvider.Commands.ActivatePaymentProvider;

public sealed class ActivatePaymentProviderHandler
{
    private readonly IPaymentProviderRepository _paymentProviderRepository;

    public ActivatePaymentProviderHandler(IPaymentProviderRepository paymentProviderRepository)
    {
        _paymentProviderRepository = paymentProviderRepository;
    }

    public async Task<ActivatePaymentProviderResult> HandleAsync(
        ActivatePaymentProviderCommand command,
        CancellationToken cancellationToken = default)
    {
        var provider = await _paymentProviderRepository.GetByIdAsync(
            command.Id,
            cancellationToken
        );

        if (provider is null)
        {
            throw new PaymentProviderNotFoundException(command.Id);
        }

        provider.Activate();

        await _paymentProviderRepository.SaveChangesAsync(cancellationToken);

        return new ActivatePaymentProviderResult(provider.Id, provider.Active);
    }
}

