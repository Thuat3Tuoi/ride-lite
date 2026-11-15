using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Refund.Commands.CompleteRefund;

public sealed class CompleteRefundCommandValidator : AbstractValidator<CompleteRefundCommand>
{
    public CompleteRefundCommandValidator()
    {
        RuleFor(x => x.RefundId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidRefundId);

        RuleFor(x => x.ProviderRefundId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(_ => PaymentErrors.InvalidParameters)
            .MaximumLength(128)
            .WithState(_ => PaymentErrors.InvalidParameters);
    }
}

