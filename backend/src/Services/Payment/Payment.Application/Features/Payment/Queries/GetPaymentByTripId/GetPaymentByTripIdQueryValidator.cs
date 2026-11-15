using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Payment.Queries.GetPaymentByTripId;

public sealed class GetPaymentByTripIdQueryValidator : AbstractValidator<GetPaymentByTripIdQuery>
{
    public GetPaymentByTripIdQueryValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidTripId);
    }
}

