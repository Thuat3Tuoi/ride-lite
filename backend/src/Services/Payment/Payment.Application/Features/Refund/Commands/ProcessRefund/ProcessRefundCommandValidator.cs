using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Refund.Commands.ProcessRefund;

public sealed class ProcessRefundCommandValidator : AbstractValidator<ProcessRefundCommand>
{
    public ProcessRefundCommandValidator()
    {
        RuleFor(x => x.RefundId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidRefundId);
    }
}

