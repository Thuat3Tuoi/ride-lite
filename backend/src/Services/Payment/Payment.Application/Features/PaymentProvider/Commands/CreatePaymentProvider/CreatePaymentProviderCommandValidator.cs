using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentProvider.Commands.CreatePaymentProvider;

public sealed class CreatePaymentProviderCommandValidator : AbstractValidator<CreatePaymentProviderCommand>
{
    public CreatePaymentProviderCommandValidator()
    {
        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(_ => PaymentErrors.InvalidParameters)
            .MaximumLength(32)
            .WithState(_ => PaymentErrors.InvalidParameters);

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(_ => PaymentErrors.InvalidName)
            .MaximumLength(128)
            .WithState(_ => PaymentErrors.InvalidName);

        RuleFor(x => x.HandlerType)
            .IsInEnum()
            .WithState(_ => PaymentErrors.InvalidParameters);
    }
}

