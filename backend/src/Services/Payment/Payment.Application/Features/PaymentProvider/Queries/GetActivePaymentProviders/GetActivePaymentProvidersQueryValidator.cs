using FluentValidation;

namespace Payment.Application.Features.PaymentProvider.Queries.GetActivePaymentProviders;

public sealed class GetActivePaymentProvidersQueryValidator : AbstractValidator<GetActivePaymentProvidersQuery>
{
    public GetActivePaymentProvidersQueryValidator()
    {
    }
}

