using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Payment.Commands.ProcessPayment;

public sealed class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidPaymentId);
    }
}

