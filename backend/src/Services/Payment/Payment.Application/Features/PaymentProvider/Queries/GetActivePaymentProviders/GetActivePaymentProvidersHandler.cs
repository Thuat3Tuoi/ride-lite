using Payment.Application.Abstractions.Data;

namespace Payment.Application.Features.PaymentProvider.Queries.GetActivePaymentProviders;

public sealed class GetActivePaymentProvidersHandler
{
    private readonly IPaymentProviderRepository _paymentProviderRepository;

    public GetActivePaymentProvidersHandler(IPaymentProviderRepository paymentProviderRepository)
    {
        _paymentProviderRepository = paymentProviderRepository;
    }

    public async Task<GetActivePaymentProvidersResult> HandleAsync(
        GetActivePaymentProvidersQuery query,
        CancellationToken cancellationToken = default)
    {
        var providers = await _paymentProviderRepository.GetAllActiveAsync(cancellationToken);

        var items = providers.Select(p => new PaymentProviderItemResult(
            p.Id,
            p.Code,
            p.Name,
            p.HandlerType,
            p.Active
        )).ToList();

        return new GetActivePaymentProvidersResult(items);
    }
}

