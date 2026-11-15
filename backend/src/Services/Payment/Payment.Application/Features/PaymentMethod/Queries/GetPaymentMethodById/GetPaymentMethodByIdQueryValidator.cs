using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentMethod.Queries.GetPaymentMethodById;

public sealed class GetPaymentMethodByIdQueryValidator : AbstractValidator<GetPaymentMethodByIdQuery>
{
    public GetPaymentMethodByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.PaymentMethodNotFound);
    }
}

