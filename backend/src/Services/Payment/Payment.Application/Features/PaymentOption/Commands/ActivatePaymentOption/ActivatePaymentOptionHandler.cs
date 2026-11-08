using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

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
            throw new PaymentOptionNotFoundException(command.Id);
        }

        option.Activate();

        await _paymentOptionRepository.SaveChangesAsync(cancellationToken);

        return new ActivatePaymentOptionResult(option.Id, option.Active);
    }
}

