using Payment.Application.Abstractions.Data;

namespace Payment.Application.Features.PaymentMethod.Queries.GetActivePaymentMethods;

public sealed class GetActivePaymentMethodsHandler
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public GetActivePaymentMethodsHandler(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<GetActivePaymentMethodsResult> HandleAsync(
        GetActivePaymentMethodsQuery query,
        CancellationToken cancellationToken = default)
    {
        var methods = await _paymentMethodRepository.GetAllActiveAsync(cancellationToken);

        var items = methods.Select(m => new PaymentMethodItemResult(
            m.Id,
            m.Code,
            m.Name,
            m.Active
        )).ToList();

        return new GetActivePaymentMethodsResult(items);
    }
}

