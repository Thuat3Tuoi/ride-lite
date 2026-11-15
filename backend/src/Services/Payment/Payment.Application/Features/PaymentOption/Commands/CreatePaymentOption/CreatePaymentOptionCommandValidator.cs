using FluentValidation;
using Payment.Domain.Errors;

namespace Payment.Application.Features.PaymentOption.Commands.CreatePaymentOption;

public sealed class CreatePaymentOptionCommandValidator : AbstractValidator<CreatePaymentOptionCommand>
{
    public CreatePaymentOptionCommandValidator()
    {
        RuleFor(x => x.MethodId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidMethodId);

        RuleFor(x => x.ProviderId)
            .GreaterThan(0)
            .WithState(_ => PaymentErrors.InvalidProviderId);

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

