using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionById;

public sealed class GetPaymentOptionByIdHandler
{
    private readonly IPaymentOptionRepository _paymentOptionRepository;

    public GetPaymentOptionByIdHandler(IPaymentOptionRepository paymentOptionRepository)
    {
        _paymentOptionRepository = paymentOptionRepository;
    }

    public async Task<GetPaymentOptionByIdResult> HandleAsync(
        GetPaymentOptionByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var option = await _paymentOptionRepository.GetByIdAsync(
            query.Id,
            cancellationToken
        );

        if (option is null)
        {
            throw new PaymentOptionNotFoundException(query.Id);
        }

        return new GetPaymentOptionByIdResult(
            option.Id,
            option.MethodId,
            option.ProviderId,
            option.Code,
            option.Name,
            option.Active
        );
    }
}

