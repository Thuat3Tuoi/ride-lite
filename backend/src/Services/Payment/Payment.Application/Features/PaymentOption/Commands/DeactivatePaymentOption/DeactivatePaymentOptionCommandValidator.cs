using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentOption.Commands.DeactivatePaymentOption;

public sealed class DeactivatePaymentOptionCommandValidator : AbstractValidator<DeactivatePaymentOptionCommand>
{
    public DeactivatePaymentOptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidOptionId);
    }
}

