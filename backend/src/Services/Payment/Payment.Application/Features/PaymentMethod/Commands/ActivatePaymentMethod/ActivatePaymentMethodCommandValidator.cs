using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentMethod.Commands.ActivatePaymentMethod;

public sealed class ActivatePaymentMethodCommandValidator : AbstractValidator<ActivatePaymentMethodCommand>
{
    public ActivatePaymentMethodCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidMethodId);
    }
}

