using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.Payment.Commands.CreatePayment;

public sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidTripId);

        RuleFor(x => x.PaymentOptionId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidOptionId);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidAmount);

        RuleFor(x => x.IdempotencyKey)
            .Must(k => k == null || k.Trim().Length <= 128)
            .WithState(_ => PaymentErrors.InvalidParameters);

        RuleFor(x => x.ProviderId)
            .Must(p => !p.HasValue || p.Value > 0)
            .WithState(_ => PaymentErrors.InvalidProviderId);
    }
}

