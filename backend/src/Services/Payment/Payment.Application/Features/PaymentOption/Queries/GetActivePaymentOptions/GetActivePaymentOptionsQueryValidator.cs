using FluentValidation;

namespace Payment.Application.Features.PaymentOption.Queries.GetActivePaymentOptions;

public sealed class GetActivePaymentOptionsQueryValidator : AbstractValidator<GetActivePaymentOptionsQuery>
{
    public GetActivePaymentOptionsQueryValidator()
    {
    }
}

