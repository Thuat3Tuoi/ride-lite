using Payment.Application.Abstractions.Data;
using Payment.Application.Common.Exceptions;

namespace Payment.Application.Features.PaymentOption.Commands.DeactivatePaymentOption;

public sealed class DeactivatePaymentOptionHandler
{
    private readonly IPaymentOptionRepository _paymentOptionRepository;

    public DeactivatePaymentOptionHandler(IPaymentOptionRepository paymentOptionRepository)
    {
        _paymentOptionRepository = paymentOptionRepository;
    }

    public async Task<DeactivatePaymentOptionResult> HandleAsync(
        DeactivatePaymentOptionCommand command,
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

        option.Deactivate();

        await _paymentOptionRepository.SaveChangesAsync(cancellationToken);

        return new DeactivatePaymentOptionResult(option.Id, option.Active);
    }
}

