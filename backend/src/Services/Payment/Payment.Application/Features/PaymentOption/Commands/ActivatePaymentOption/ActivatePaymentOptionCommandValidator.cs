using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentOption.Commands.ActivatePaymentOption;

public sealed class ActivatePaymentOptionCommandValidator : AbstractValidator<ActivatePaymentOptionCommand>
{
    public ActivatePaymentOptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidOptionId);
    }
}

