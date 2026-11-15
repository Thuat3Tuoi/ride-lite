using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.LedgerEntry.Commands.CreatePaymentLedgerEntry;

public sealed class CreatePaymentLedgerEntryCommandValidator : AbstractValidator<CreatePaymentLedgerEntryCommand>
{
    public CreatePaymentLedgerEntryCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidPaymentId);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidAmount);

        RuleFor(x => x.DebitAccount)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(_ => PaymentErrors.InvalidParameters)
            .MaximumLength(64)
            .WithState(_ => PaymentErrors.InvalidParameters);

        RuleFor(x => x.CreditAccount)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(_ => PaymentErrors.InvalidParameters)
            .MaximumLength(64)
            .WithState(_ => PaymentErrors.InvalidParameters);
    }
}

