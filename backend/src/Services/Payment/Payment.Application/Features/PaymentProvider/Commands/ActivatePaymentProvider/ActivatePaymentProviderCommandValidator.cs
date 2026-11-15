using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentProvider.Commands.ActivatePaymentProvider;

public sealed class ActivatePaymentProviderCommandValidator : AbstractValidator<ActivatePaymentProviderCommand>
{
    public ActivatePaymentProviderCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidProviderId);
    }
}

