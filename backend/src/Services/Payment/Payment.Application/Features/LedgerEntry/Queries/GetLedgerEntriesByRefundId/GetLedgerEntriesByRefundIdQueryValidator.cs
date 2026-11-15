using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByRefundId;

public sealed class GetLedgerEntriesByRefundIdQueryValidator : AbstractValidator<GetLedgerEntriesByRefundIdQuery>
{
    public GetLedgerEntriesByRefundIdQueryValidator()
    {
        RuleFor(x => x.RefundId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidRefundId);
    }
}

