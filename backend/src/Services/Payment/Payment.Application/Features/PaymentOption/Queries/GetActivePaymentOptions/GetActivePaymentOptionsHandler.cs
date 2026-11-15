using Payment.Application.Abstractions.Data;
using Payment.Application.Abstractions.Messaging;

namespace Payment.Application.Features.PaymentOption.Queries.GetActivePaymentOptions;

public sealed class GetActivePaymentOptionsHandler : IQueryHandler<GetActivePaymentOptionsQuery, GetActivePaymentOptionsResult>
{
    private readonly IPaymentOptionRepository _paymentOptionRepository;

    public GetActivePaymentOptionsHandler(IPaymentOptionRepository paymentOptionRepository)
    {
        _paymentOptionRepository = paymentOptionRepository;
    }

    public async Task<GetActivePaymentOptionsResult> HandleAsync(
        GetActivePaymentOptionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var options = await _paymentOptionRepository.GetAllActiveAsync(cancellationToken);

        var items = options.Select(o => new PaymentOptionItemResult(
            o.Id,
            o.MethodId,
            o.ProviderId,
            o.Code,
            o.Name,
            o.Active
        )).ToList();

        return new GetActivePaymentOptionsResult(items);
    }
}

