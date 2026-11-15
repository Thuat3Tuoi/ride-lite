using FluentValidation;

namespace Payment.Application.Features.PaymentMethod.Queries.GetActivePaymentMethods;

public sealed class GetActivePaymentMethodsQueryValidator : AbstractValidator<GetActivePaymentMethodsQuery>
{
    public GetActivePaymentMethodsQueryValidator()
    {
    }
}

