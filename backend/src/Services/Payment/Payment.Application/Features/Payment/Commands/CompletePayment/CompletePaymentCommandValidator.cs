using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Payment.Commands.CompletePayment;

public sealed class CompletePaymentCommandValidator : AbstractValidator<CompletePaymentCommand>
{
    public CompletePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidPaymentId);

        RuleFor(x => x.ProviderTransactionId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(_ => PaymentErrors.InvalidParameters)
            .MaximumLength(128)
            .WithState(_ => PaymentErrors.InvalidParameters);

        RuleFor(x => x.ResolvedProviderId)
            .Must(p => !p.HasValue || p.Value > 0)
            .WithState(_ => PaymentErrors.InvalidProviderId);
    }
}

