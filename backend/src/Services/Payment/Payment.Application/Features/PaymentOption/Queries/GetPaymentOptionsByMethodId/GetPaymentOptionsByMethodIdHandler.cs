using Payment.Application.Abstractions.Data;
using Payment.Application.Abstractions.Messaging;

namespace Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionsByMethodId;

public sealed class GetPaymentOptionsByMethodIdHandler : IQueryHandler<GetPaymentOptionsByMethodIdQuery, GetPaymentOptionsByMethodIdResult>
{
    private readonly IPaymentOptionRepository _paymentOptionRepository;

    public GetPaymentOptionsByMethodIdHandler(IPaymentOptionRepository paymentOptionRepository)
    {
        _paymentOptionRepository = paymentOptionRepository;
    }

    public async Task<GetPaymentOptionsByMethodIdResult> HandleAsync(
        GetPaymentOptionsByMethodIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var options = await _paymentOptionRepository.GetByMethodIdAsync(
            query.MethodId,
            cancellationToken
        );

        var items = options.Select(o => new PaymentOptionByMethodItemResult(
            o.Id,
            o.MethodId,
            o.ProviderId,
            o.Code,
            o.Name,
            o.Active
        )).ToList();

        return new GetPaymentOptionsByMethodIdResult(items);
    }
}

