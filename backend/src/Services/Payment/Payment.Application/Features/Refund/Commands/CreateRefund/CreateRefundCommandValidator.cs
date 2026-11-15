using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Refund.Commands.CreateRefund;

public sealed class CreateRefundCommandValidator : AbstractValidator<CreateRefundCommand>
{
    public CreateRefundCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidPaymentId);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidAmount);

        RuleFor(x => x.IdempotencyKey)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(_ => PaymentErrors.InvalidParameters)
            .MaximumLength(128)
            .WithState(_ => PaymentErrors.InvalidParameters);

        RuleFor(x => x.Reason)
            .Cascade(CascadeMode.Stop)
            .Must(r => r == null || !string.IsNullOrWhiteSpace(r))
            .WithState(_ => PaymentErrors.InvalidReason)
            .Must(r => r == null || r.Trim().Length <= 500)
            .WithState(_ => PaymentErrors.InvalidReason);
    }
}

