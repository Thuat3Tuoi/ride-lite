using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.PaymentMethod.Queries.GetPaymentMethodById;

public sealed class GetPaymentMethodByIdHandler
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public GetPaymentMethodByIdHandler(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<GetPaymentMethodByIdResult> HandleAsync(
        GetPaymentMethodByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var method = await _paymentMethodRepository.GetByIdAsync(
            query.Id,
            cancellationToken
        );

        if (method is null)
        {
            throw new PaymentMethodNotFoundException(query.Id);
        }

        return new GetPaymentMethodByIdResult(
            method.Id,
            method.Code,
            method.Name,
            method.Active
        );
    }
}

