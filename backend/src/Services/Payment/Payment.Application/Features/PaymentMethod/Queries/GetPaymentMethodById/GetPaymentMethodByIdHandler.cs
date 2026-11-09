using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Errors;

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
            throw new AppException(PaymentErrors.PaymentMethodNotFound);
        }

        return new GetPaymentMethodByIdResult(
            method.Id,
            method.Code,
            method.Name,
            method.Active
        );
    }
}
