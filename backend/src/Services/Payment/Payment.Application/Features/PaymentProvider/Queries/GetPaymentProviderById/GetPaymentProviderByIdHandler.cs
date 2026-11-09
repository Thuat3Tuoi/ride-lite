using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentProvider.Queries.GetPaymentProviderById;

public sealed class GetPaymentProviderByIdHandler
{
    private readonly IPaymentProviderRepository _paymentProviderRepository;

    public GetPaymentProviderByIdHandler(IPaymentProviderRepository paymentProviderRepository)
    {
        _paymentProviderRepository = paymentProviderRepository;
    }

    public async Task<GetPaymentProviderByIdResult> HandleAsync(
        GetPaymentProviderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var provider = await _paymentProviderRepository.GetByIdAsync(
            query.Id,
            cancellationToken
        );

        if (provider is null)
        {
            throw new AppException(PaymentErrors.PaymentProviderNotFound);
        }

        return new GetPaymentProviderByIdResult(
            provider.Id,
            provider.Code,
            provider.Name,
            provider.HandlerType,
            provider.Active
        );
    }
}
