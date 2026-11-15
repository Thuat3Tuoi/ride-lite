using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByPaymentId;

public sealed class GetLedgerEntriesByPaymentIdQueryValidator : AbstractValidator<GetLedgerEntriesByPaymentIdQuery>
{
    public GetLedgerEntriesByPaymentIdQueryValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidPaymentId);
    }
}

