using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Refund.Queries.GetRefundById;

public sealed class GetRefundByIdQueryValidator : AbstractValidator<GetRefundByIdQuery>
{
    public GetRefundByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.RefundNotFound);
    }
}

