using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Refund.Queries.GetRefundsByPaymentId;

public sealed class GetRefundsByPaymentIdQueryValidator : AbstractValidator<GetRefundsByPaymentIdQuery>
{
    public GetRefundsByPaymentIdQueryValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidPaymentId);
    }
}

