using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentMethod.Commands.DeactivatePaymentMethod;

public sealed class DeactivatePaymentMethodCommandValidator : AbstractValidator<DeactivatePaymentMethodCommand>
{
    public DeactivatePaymentMethodCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidMethodId);
    }
}

