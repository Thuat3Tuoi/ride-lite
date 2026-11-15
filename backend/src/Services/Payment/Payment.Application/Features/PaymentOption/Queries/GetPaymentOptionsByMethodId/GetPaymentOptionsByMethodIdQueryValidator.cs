using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionsByMethodId;

public sealed class GetPaymentOptionsByMethodIdQueryValidator : AbstractValidator<GetPaymentOptionsByMethodIdQuery>
{
    public GetPaymentOptionsByMethodIdQueryValidator()
    {
        RuleFor(x => x.MethodId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidMethodId);
    }
}

