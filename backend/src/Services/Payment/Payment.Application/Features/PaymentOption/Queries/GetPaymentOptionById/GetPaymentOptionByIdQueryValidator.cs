using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionById;

public sealed class GetPaymentOptionByIdQueryValidator : AbstractValidator<GetPaymentOptionByIdQuery>
{
    public GetPaymentOptionByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.PaymentOptionNotFound);
    }
}

