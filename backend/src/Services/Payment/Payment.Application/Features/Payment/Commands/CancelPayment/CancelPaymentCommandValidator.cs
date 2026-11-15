using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Payment.Commands.CancelPayment;

public sealed class CancelPaymentCommandValidator : AbstractValidator<CancelPaymentCommand>
{
    public CancelPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidPaymentId);
    }
}

