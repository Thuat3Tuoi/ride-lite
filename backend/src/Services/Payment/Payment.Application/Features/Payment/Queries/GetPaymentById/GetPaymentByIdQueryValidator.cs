using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Payment.Queries.GetPaymentById;

public sealed class GetPaymentByIdQueryValidator : AbstractValidator<GetPaymentByIdQuery>
{
    public GetPaymentByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.NotFound);
    }
}

