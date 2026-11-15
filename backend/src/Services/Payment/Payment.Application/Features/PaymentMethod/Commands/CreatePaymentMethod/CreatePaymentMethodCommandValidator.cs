using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentMethod.Commands.CreatePaymentMethod;

public sealed class CreatePaymentMethodCommandValidator : AbstractValidator<CreatePaymentMethodCommand>
{
    public CreatePaymentMethodCommandValidator()
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
    }
}

