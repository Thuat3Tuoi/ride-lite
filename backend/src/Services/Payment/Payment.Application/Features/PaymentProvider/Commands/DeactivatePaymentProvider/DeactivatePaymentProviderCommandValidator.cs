using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentProvider.Commands.DeactivatePaymentProvider;

public sealed class DeactivatePaymentProviderCommandValidator : AbstractValidator<DeactivatePaymentProviderCommand>
{
    public DeactivatePaymentProviderCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidProviderId);
    }
}

