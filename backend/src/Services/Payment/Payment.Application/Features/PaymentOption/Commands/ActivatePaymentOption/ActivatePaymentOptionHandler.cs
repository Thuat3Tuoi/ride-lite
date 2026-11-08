using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentOption.Commands.ActivatePaymentOption;

public sealed class ActivatePaymentOptionHandler
{
    private readonly IPaymentOptionRepository _paymentOptionRepository;

    public ActivatePaymentOptionHandler(IPaymentOptionRepository paymentOptionRepository)
    {
        _paymentOptionRepository = paymentOptionRepository;
    }

    public async Task<ActivatePaymentOptionResult> HandleAsync(
        ActivatePaymentOptionCommand command,
        CancellationToken cancellationToken = default)
    {
        var option = await _paymentOptionRepository.GetByIdAsync(
            command.Id,
            cancellationToken
        );

        if (option is null)
        {
            throw new AppException(PaymentErrors.PaymentOptionNotFound);
        }

        option.Activate();

        await _paymentOptionRepository.SaveChangesAsync(cancellationToken);

        return new ActivatePaymentOptionResult(option.Id, option.Active);
    }
}
