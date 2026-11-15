using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentProvider.Queries.GetPaymentProviderById;

public sealed class GetPaymentProviderByIdQueryValidator : AbstractValidator<GetPaymentProviderByIdQuery>
{
    public GetPaymentProviderByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.PaymentProviderNotFound);
    }
}

